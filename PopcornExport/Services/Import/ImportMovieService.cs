using PopcornExport.Helpers;
using PopcornExport.Models.Movie;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Database;
using PopcornExport.Services.Assets;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using Microsoft.EntityFrameworkCore;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;
using PopcornExport.Services.Language;
using PopcornExport.Services.Subtitle;
using ShellProgressBar;
using Utf8Json;
using Movie = PopcornExport.Database.Movie;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import movies
    /// </summary>
    public sealed class ImportMovieService : IImportService
    {
        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Assets service
        /// </summary>
        private readonly IAssetsService _assetsService;

        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// The subtitle service
        /// </summary>
        private readonly ISubtitleService _subtitleService;

        /// <summary>
        /// The language service
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        /// <param name="subtitleService">The subtitle service</param>
        /// <param name="languageService">The language service</param>
        /// <param name="fileService">The file service</param>
        public ImportMovieService(IAssetsService assetsService,
            ILoggingService loggingService, ISubtitleService subtitleService, ILanguageService languageService, IFileService fileService)
        {
            _fileService = fileService;
            _loggingService = loggingService;
            _assetsService = assetsService;
            _subtitleService = subtitleService;
            _languageService = languageService;

            TmdbClient = new TMDbClient(Constants.TmDbClientId)
            {
                MaxRetryCount = 10
            };

            try
            {
                TmdbClient.GetConfig();
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// TMDb client
        /// </summary>
        private TMDbClient TmdbClient { get; }

        /// <summary>
        /// Import movies to database
        /// </summary>
        /// <param name="rawImports">Documents to import</param>
        /// <param name="pbar"><see cref="IProgressBar"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<string> rawImports, IProgressBar pbar)
        {
            var imports = rawImports.ToList();
            var workBarOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                BackgroundColor = ConsoleColor.DarkGray,
            };
            using (var childProgress = pbar?.Spawn(imports.Count, "step import progress", workBarOptions))
            {
                using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
                {
                    foreach (var import in imports)
                    {
                        try
                        {
                            // Deserialize a document to a movie
                            var movieJson =
                                JsonSerializer.Deserialize<MovieJson>(import);

                            if (movieJson.Torrents == null || movieJson.Cast == null)
                                continue;

                            var movie = new Movie
                            {
                                ImdbCode = movieJson.ImdbCode,
                                Url = movieJson.Url,
                                Torrents = movieJson.Torrents.Select(torrent => new TorrentMovie
                                {
                                    Url = torrent.Url,
                                    DateUploaded = torrent.DateUploaded,
                                    DateUploadedUnix = torrent.DateUploadedUnix,
                                    Quality = torrent.Quality,
                                    Hash = torrent.Hash,
                                    Peers = torrent.Peers,
                                    Seeds = torrent.Seeds,
                                    Size = torrent.Size,
                                    SizeBytes = torrent.SizeBytes
                                }).ToList(),
                                DateUploaded = movieJson.DateUploaded,
                                DateUploadedUnix = movieJson.DateUploadedUnix,
                                DownloadCount = movieJson.DownloadCount,
                                MpaRating = movieJson.MpaRating,
                                Runtime = movieJson.Runtime,
                                YtTrailerCode = movieJson.YtTrailerCode,
                                DescriptionIntro = movieJson.DescriptionIntro,
                                TitleLong = movieJson.TitleLong,
                                Rating = movieJson.Rating,
                                Year = movieJson.Year,
                                LikeCount = movieJson.LikeCount,
                                DescriptionFull = movieJson.DescriptionFull,
                                Cast = movieJson.Cast?.Select(cast => new Database.Cast
                                {
                                    ImdbCode = cast?.ImdbCode,
                                    SmallImage = cast?.SmallImage,
                                    CharacterName = cast?.CharacterName,
                                    Name = cast?.Name
                                }).ToList(),
                                Genres = movieJson.Genres.Select(genre => new Genre
                                {
                                    Name = genre
                                }).ToList(),
                                GenreNames = string.Join(", ", movieJson.Genres.Select(FirstCharToUpper)),
                                Language = movieJson.Language,
                                Slug = movieJson.Slug,
                                Title = movieJson.Title,
                                BackgroundImage = movieJson.BackgroundImage,
                                PosterImage = movieJson.PosterImage
                            };

                            var languages = await _languageService.GetLanguages();
                            var subtitles = (await _subtitleService.SearchSubtitlesFromImdb(
                                languages.Select(lang => lang.SubLanguageId).Aggregate((a, b) => a + "," + b),
                                movieJson.ImdbCode.Replace("tt", ""), null, null)).GroupBy(x => x.LanguageName,
                                (k, g) =>
                                    g.Aggregate(
                                        (a, x) =>
                                            (Convert.ToDouble(x.Rating, CultureInfo.InvariantCulture) >=
                                             Convert.ToDouble(a.Rating, CultureInfo.InvariantCulture))
                                                ? x
                                                : a));
                            if (!context.MovieSet.Any(a => a.ImdbCode == movie.ImdbCode))
                            {
                                await RetrieveAssets(movie).ConfigureAwait(false);
                                movie.Subtitles = subtitles.Select(async subtitle => new Database.Subtitle
                                {
                                    ImdbId = subtitle.ImdbId,
                                    LanguageName = subtitle.LanguageName,
                                    Rating = Convert.ToDouble(subtitle.Rating, CultureInfo.InvariantCulture),
                                    Bad = Convert.ToDouble(subtitle.Bad, CultureInfo.InvariantCulture),
                                    Iso639 = subtitle.ISO639,
                                    LanguageId = subtitle.LanguageId,
                                    OsdbSubtitleId = subtitle.SubtitleId,
                                    SubtitleDownloadLink = await _fileService.UploadFileFromUrlToAzureStorage(
                                        $@"subtitles/movies/{movie.ImdbCode}/{subtitle.SubtitleId}" + ".srt",
                                        await _subtitleService.DownloadSubtitleToPath(subtitle.SubtitleId,
                                            subtitle.ISO639), ExportType.Subtitles),
                                    SubtitleFileName = subtitle.SubtitleId + "." +
                                                       subtitle.SubTitleDownloadLink.OriginalString.Split('.').Last()
                                }).Select(a => a.Result).ToList();
                                context.MovieSet.Add(movie);
                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }
                            else
                            {
                                var existingEntity =
                                    await context.MovieSet.Include(a => a.Torrents)
                                        .FirstOrDefaultAsync(a => a.ImdbCode == movie.ImdbCode).ConfigureAwait(false);

                                existingEntity.DownloadCount = movie.DownloadCount;
                                existingEntity.LikeCount = movie.LikeCount;
                                existingEntity.Rating = movie.Rating;
                                foreach (var torrent in existingEntity.Torrents)
                                {
                                    var updatedTorrent =
                                        movie.Torrents.FirstOrDefault(a => a.Quality == torrent.Quality);
                                    if (updatedTorrent == null) continue;
                                    torrent.Peers = updatedTorrent.Peers;
                                    torrent.Seeds = updatedTorrent.Seeds;
                                }

                                foreach (var subtitle in subtitles)
                                {
                                    if (existingEntity.Subtitles.All(a => a.OsdbSubtitleId != subtitle.SubtitleId) ||
                                        existingEntity.Subtitles.Any(a =>
                                            a.OsdbSubtitleId == subtitle.SubtitleId &&
                                            string.IsNullOrEmpty(a.SubtitleDownloadLink)))
                                    {
                                        existingEntity.Subtitles.Add(new Database.Subtitle
                                        {
                                            ImdbId = subtitle.ImdbId,
                                            LanguageName = subtitle.LanguageName,
                                            Rating = Convert.ToDouble(subtitle.Rating, CultureInfo.InvariantCulture),
                                            Bad = Convert.ToDouble(subtitle.Bad, CultureInfo.InvariantCulture),
                                            Iso639 = subtitle.ISO639,
                                            LanguageId = subtitle.LanguageId,
                                            OsdbSubtitleId = subtitle.SubtitleId,
                                            SubtitleDownloadLink = await _fileService.UploadFileFromUrlToAzureStorage(
                                                $@"subtitles/movies/{movie.ImdbCode}/{subtitle.SubtitleId}" + ".srt",
                                                await _subtitleService.DownloadSubtitleToPath(subtitle.SubtitleId,
                                                    subtitle.ISO639), ExportType.Subtitles),
                                            SubtitleFileName = subtitle.SubtitleId + "." +
                                                               subtitle.SubTitleDownloadLink.OriginalString.Split('.')
                                                                   .Last()
                                        });
                                    }
                                }

                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }

                            childProgress?.Tick();
                        }
                        catch (Exception ex)
                        {
                            _loggingService.Telemetry.TrackException(ex);
                        }
                    }
                }

                // Finish
                pbar?.Tick();
            }
        }

        private static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// Retrieve assets for the provided movie
        /// </summary>
        /// <param name="movie">Movie to update</param>
        /// <returns></returns>
        private async Task RetrieveAssets(Movie movie)
        {
            var tmdbMovie = await TmdbClient.GetMovieAsync(movie.ImdbCode, MovieMethods.Images | MovieMethods.Similar)
                .ConfigureAwait(false);
            if (tmdbMovie.Images?.Backdrops != null && tmdbMovie.Images.Backdrops.Any())
            {
                var backdrop = GetImagePathFromTmdb(TmdbClient,
                    tmdbMovie.Images.Backdrops.Aggregate((image1, image2) =>
                        image1 != null && image2 != null && image1.VoteCount < image2.VoteCount
                            ? image2
                            : image1).FilePath);
                movie.BackgroundImage =
                    await _assetsService.UploadFile(
                        $@"images/{movie.ImdbCode}/background/{backdrop.Split('/').Last()}",
                        backdrop).ConfigureAwait(false);
            }

            if (tmdbMovie.Images?.Posters != null && tmdbMovie.Images.Posters.Any())
            {
                var poster = GetImagePathFromTmdb(TmdbClient,
                    tmdbMovie.Images.Posters.Aggregate((image1, image2) =>
                        image1 != null && image2 != null && image1.VoteCount < image2.VoteCount
                            ? image2
                            : image1).FilePath);
                movie.PosterImage =
                    await _assetsService.UploadFile(
                        $@"images/{movie.ImdbCode}/poster/{poster.Split('/').Last()}",
                        poster).ConfigureAwait(false);
            }

            if (movie.Torrents != null)
            {
                foreach (var torrent in movie.Torrents)
                {
                    torrent.Url =
                        await _assetsService.UploadFile(
                            $@"torrents/{movie.ImdbCode}/{torrent.Quality}/{movie.ImdbCode}.torrent",
                            torrent.Url).ConfigureAwait(false);
                }
            }

            if (movie.Cast != null)
            {
                foreach (var cast in movie.Cast)
                {
                    if (!string.IsNullOrWhiteSpace(cast.SmallImage))
                    {
                        cast.SmallImage = await _assetsService.UploadFile(
                            $@"images/{movie.ImdbCode}/cast/{cast.ImdbCode}/{
                                    cast.SmallImage.Split
                                        ('/')
                                        .Last()
                                }", cast.SmallImage).ConfigureAwait(false);
                    }
                }
            }

            if (!movie.Similars.Any() && tmdbMovie.Similar != null && tmdbMovie.Similar.TotalResults != 0)
            {
                movie.Similars = new List<Similar>();
                foreach (var id in tmdbMovie.Similar.Results.Select(a => a.Id))
                {
                    try
                    {
                        var res = await TmdbClient.GetMovieAsync(id).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(res?.ImdbId))
                        {
                            movie.Similars.Add(new Similar
                            {
                                TmdbId = res.ImdbId
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve an image from Tmdb
        /// </summary>
        /// <param name="client"><see cref="TMDbClient"/></param>
        /// <param name="path">Path to the image to retrieve</param>
        /// <returns></returns>
        private string GetImagePathFromTmdb(TMDbClient client, string path)
        {
            return client.GetImageUrl("original", path, true).AbsoluteUri;
        }
    }
}