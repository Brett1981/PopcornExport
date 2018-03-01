using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PopcornExport.Comparers;
using PopcornExport.Models.Show;
using PopcornExport.Services.Assets;
using PopcornExport.Database;
using TMDbLib.Client;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;
using PopcornExport.Services.Language;
using PopcornExport.Services.Subtitle;
using ShellProgressBar;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.TvShows;
using Utf8Json;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import shows
    /// </summary>
    public sealed class ImportShowService : IImportService
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
        public ImportShowService(IAssetsService assetsService,
            ILoggingService loggingService, ISubtitleService subtitleService, ILanguageService languageService,
            IFileService fileService)
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
        /// Import shows to database
        /// </summary>
        /// <param name="rawImports">Imports to import</param>
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
                            // Deserialize a document to a show
                            var showJson =
                                JsonSerializer.Deserialize<ShowJson>(import);

                            if (showJson.Year == null) continue;

                            var show = new Show
                            {
                                Rating = new Rating
                                {
                                    Hated = showJson.Rating.Hated,
                                    Percentage = Convert.ToInt32(showJson.Rating.Percentage),
                                    Votes = showJson.Rating.Votes,
                                    Loved = showJson.Rating.Loved,
                                    Watching = showJson.Rating.Watching
                                },
                                Images = new ImageShow
                                {
                                    Banner = showJson.Images?.Banner,
                                    Poster = showJson.Images?.Poster
                                },
                                ImdbId = showJson.ImdbId,
                                Title = WebUtility.HtmlDecode(showJson.Title),
                                Year = int.Parse(showJson.Year),
                                Runtime = showJson.Runtime,
                                Genres = showJson.Genres.Select(genre => new Genre
                                {
                                    Name = genre
                                }).ToList(),
                                GenreNames =
                                    string.Join(", ", showJson.Genres.Select(FirstCharToUpper)),
                                Slug = showJson.Slug,
                                LastUpdated = showJson.LastUpdated,
                                TvdbId = showJson.TvdbId,
                                NumSeasons = showJson.NumSeasons,
                                Status = showJson.Status,
                                Synopsis = showJson.Synopsis,
                                Country = showJson.Country,
                                Episodes = showJson.Episodes.Select(episode => new EpisodeShow
                                {
                                    Title = WebUtility.HtmlDecode(episode.Title),
                                    DateBased = episode.DateBased,
                                    TvdbId = episode.TvdbId != null &&
                                             int.TryParse(episode.TvdbId.ToString(), out var tvdbId)
                                        ? tvdbId
                                        : 0,
                                    Torrents = new TorrentNode
                                    {
                                        Torrent0 = new Torrent
                                        {
                                            Url = episode.Torrents.Torrent_0?.Url,
                                            Peers = episode.Torrents.Torrent_0?.Peers,
                                            Seeds = episode.Torrents.Torrent_0?.Seeds
                                        },
                                        Torrent1080p = new Torrent
                                        {
                                            Url = episode.Torrents.Torrent_1080p?.Url,
                                            Peers = episode.Torrents.Torrent_1080p?.Peers,
                                            Seeds = episode.Torrents.Torrent_1080p?.Seeds
                                        },
                                        Torrent480p = new Torrent
                                        {
                                            Url = episode.Torrents.Torrent_480p?.Url,
                                            Peers = episode.Torrents.Torrent_480p?.Peers,
                                            Seeds = episode.Torrents.Torrent_480p?.Seeds
                                        },
                                        Torrent720p = new Torrent
                                        {
                                            Url = episode.Torrents.Torrent_720p?.Url,
                                            Peers = episode.Torrents.Torrent_720p?.Peers,
                                            Seeds = episode.Torrents.Torrent_720p?.Seeds
                                        }
                                    },
                                    EpisodeNumber =
                                        int.TryParse(episode.EpisodeNumber.ToString(), out var episodeNumber)
                                            ? episodeNumber
                                            : 0,
                                    Season = int.TryParse(episode.Season.ToString(), out var season) ? season : 0,
                                    Overview = episode.Overview,
                                    FirstAired = episode.FirstAired
                                }).ToList(),
                                AirDay = showJson.AirDay,
                                AirTime = showJson.AirTime,
                                Network = showJson.Network
                            };

                            var languages = (await _languageService.GetLanguages()).ToList();
                            if (!context.ShowSet.Any(a => a.ImdbId == show.ImdbId))
                            {
                                await UpdateImagesAndSimilarShow(show).ConfigureAwait(false);
                                foreach (var episode in show.Episodes)
                                {
                                    var subtitles = (await _subtitleService.SearchSubtitlesFromImdb(
                                            languages.Select(lang => lang.SubLanguageId)
                                                .Aggregate((a, b) => a + "," + b),
                                            showJson.ImdbId.Replace("tt", ""), episode.Season,
                                            episode.EpisodeNumber))
                                        .GroupBy(
                                            x => x.LanguageName,
                                            (k, g) =>
                                                g.Aggregate(
                                                    (a, x) =>
                                                        (Convert.ToDouble(x.Rating, CultureInfo.InvariantCulture) >=
                                                         Convert.ToDouble(a.Rating, CultureInfo.InvariantCulture))
                                                            ? x
                                                            : a));
                                    episode.Subtitles = subtitles.Select(async subtitle => new Database.Subtitle
                                    {
                                        ImdbId = subtitle.ImdbId,
                                        LanguageName = subtitle.LanguageName,
                                        Rating = Convert.ToDouble(subtitle.Rating, CultureInfo.InvariantCulture),
                                        Bad = Convert.ToDouble(subtitle.Bad, CultureInfo.InvariantCulture),
                                        Iso639 = subtitle.ISO639,
                                        LanguageId = subtitle.LanguageId,
                                        OsdbSubtitleId = subtitle.SubtitleId,
                                        SubtitleDownloadLink = await _subtitleService.DownloadSubtitleToPath(
                                            subtitle.SubtitleId, subtitle.ImdbId,
                                            subtitle.ISO639,
                                            $@"shows/{subtitle.ImdbId}/{subtitle.ISO639}/{subtitle.SubtitleId}" +
                                            ".srt", subtitle.SubTitleDownloadLink.OriginalString, ExportType.Shows),
                                        SubtitleFileName = subtitle.SubtitleId + "." +
                                                           subtitle.SubTitleDownloadLink.OriginalString.Split('.')
                                                               .Last()
                                    }).Select(a => a.Result).ToList();
                                }

                                context.ShowSet.Add(show);
                            }
                            else
                            {
                                var existingEntity = await context.ShowSet.Include(a => a.Rating)
                                    .Include(a => a.Episodes)
                                    .ThenInclude(episode => episode.Torrents)
                                    .ThenInclude(torrent => torrent.Torrent0)
                                    .Include(a => a.Episodes)
                                    .ThenInclude(episode => episode.Torrents)
                                    .ThenInclude(torrent => torrent.Torrent1080p)
                                    .Include(a => a.Episodes)
                                    .ThenInclude(episode => episode.Torrents)
                                    .ThenInclude(torrent => torrent.Torrent480p)
                                    .Include(a => a.Episodes)
                                    .ThenInclude(episode => episode.Torrents)
                                    .ThenInclude(torrent => torrent.Torrent720p)
                                    .Include(a => a.Genres)
                                    .Include(a => a.Images)
                                    .Include(a => a.Similars).FirstOrDefaultAsync(a => a.ImdbId == show.ImdbId)
                                    .ConfigureAwait(false);

                                existingEntity.Rating.Hated = show.Rating.Hated;
                                existingEntity.Rating.Loved = show.Rating.Loved;
                                existingEntity.Rating.Percentage = show.Rating.Percentage;
                                existingEntity.Rating.Votes = show.Rating.Votes;
                                existingEntity.Rating.Watching = show.Rating.Watching;
                                existingEntity.AirDay = show.AirDay;
                                existingEntity.AirTime = show.AirTime;
                                existingEntity.Status = show.Status;
                                existingEntity.NumSeasons = show.NumSeasons;
                                foreach (var episode in existingEntity.Episodes)
                                {
                                    var updatedEpisode = show.Episodes.FirstOrDefault(a => a.TvdbId == episode.TvdbId);
                                    if (updatedEpisode == null) continue;
                                    if (episode.Torrents?.Torrent0 != null && updatedEpisode.Torrents.Torrent0 != null)
                                    {
                                        episode.Torrents.Torrent0.Peers = updatedEpisode.Torrents.Torrent0.Peers;
                                        episode.Torrents.Torrent0.Seeds = updatedEpisode.Torrents.Torrent0.Seeds;
                                        if (!string.IsNullOrWhiteSpace(updatedEpisode.Torrents.Torrent0.Url))
                                            episode.Torrents.Torrent0.Url = updatedEpisode.Torrents.Torrent0.Url;
                                    }

                                    if (episode.Torrents?.Torrent1080p != null &&
                                        updatedEpisode.Torrents.Torrent1080p != null)
                                    {
                                        episode.Torrents.Torrent1080p.Peers =
                                            updatedEpisode.Torrents.Torrent1080p.Peers;
                                        episode.Torrents.Torrent1080p.Seeds =
                                            updatedEpisode.Torrents.Torrent1080p.Seeds;
                                        if (!string.IsNullOrWhiteSpace(updatedEpisode.Torrents.Torrent1080p.Url))
                                            episode.Torrents.Torrent1080p.Url =
                                                updatedEpisode.Torrents.Torrent1080p.Url;
                                    }

                                    if (episode.Torrents?.Torrent720p != null &&
                                        updatedEpisode.Torrents.Torrent720p != null)
                                    {
                                        episode.Torrents.Torrent720p.Peers = updatedEpisode.Torrents.Torrent720p.Peers;
                                        episode.Torrents.Torrent720p.Seeds = updatedEpisode.Torrents.Torrent720p.Seeds;
                                        if (!string.IsNullOrWhiteSpace(updatedEpisode.Torrents.Torrent720p.Url))
                                            episode.Torrents.Torrent720p.Url = updatedEpisode.Torrents.Torrent720p.Url;
                                    }

                                    if (episode.Torrents?.Torrent480p != null &&
                                        updatedEpisode.Torrents.Torrent480p != null)
                                    {
                                        episode.Torrents.Torrent480p.Peers = updatedEpisode.Torrents.Torrent480p.Peers;
                                        episode.Torrents.Torrent480p.Seeds = updatedEpisode.Torrents.Torrent480p.Seeds;
                                        if (!string.IsNullOrWhiteSpace(updatedEpisode.Torrents.Torrent480p.Url))
                                            episode.Torrents.Torrent480p.Url = updatedEpisode.Torrents.Torrent480p.Url;
                                    }

                                    var subtitles = (await _subtitleService.SearchSubtitlesFromImdb(
                                            languages.Select(lang => lang.SubLanguageId)
                                                .Aggregate((a, b) => a + "," + b),
                                            showJson.ImdbId.Replace("tt", ""), episode.Season,
                                            episode.EpisodeNumber))
                                        .GroupBy(
                                            x => x.LanguageName,
                                            (k, g) =>
                                                g.Aggregate(
                                                    (a, x) =>
                                                        (Convert.ToDouble(x.Rating, CultureInfo.InvariantCulture) >=
                                                         Convert.ToDouble(a.Rating, CultureInfo.InvariantCulture))
                                                            ? x
                                                            : a));
                                    foreach (var subtitle in subtitles)
                                    {
                                        if (episode.Subtitles.All(a => a.OsdbSubtitleId != subtitle.SubtitleId) ||
                                            episode.Subtitles.Any(a =>
                                                a.OsdbSubtitleId == subtitle.SubtitleId &&
                                                string.IsNullOrEmpty(a.SubtitleDownloadLink)))
                                        {
                                            episode.Subtitles.Add(new Database.Subtitle
                                            {
                                                ImdbId = subtitle.ImdbId,
                                                LanguageName = subtitle.LanguageName,
                                                Rating =
                                                    Convert.ToDouble(subtitle.Rating, CultureInfo.InvariantCulture),
                                                Bad = Convert.ToDouble(subtitle.Bad, CultureInfo.InvariantCulture),
                                                Iso639 = subtitle.ISO639,
                                                LanguageId = subtitle.LanguageId,
                                                OsdbSubtitleId = subtitle.SubtitleId,
                                                SubtitleDownloadLink =
                                                    await _subtitleService.DownloadSubtitleToPath(
                                                        subtitle.SubtitleId, subtitle.ImdbId,
                                                        subtitle.ISO639,
                                                        $@"shows/{subtitle.ImdbId}/{subtitle.ISO639}/{
                                                                subtitle.SubtitleId
                                                            }" + ".srt",
                                                        subtitle.SubTitleDownloadLink.OriginalString,
                                                        ExportType.Shows),
                                                SubtitleFileName = subtitle.SubtitleId + "." +
                                                                   subtitle.SubTitleDownloadLink.OriginalString
                                                                       .Split('.').Last()
                                            });
                                        }
                                    }

                                }

                                var newEpisodes =
                                    show.Episodes.Except(existingEntity.Episodes, new EpisodeComparer());
                                foreach (var newEpisode in newEpisodes.ToList())
                                {
                                    existingEntity.Episodes.Add(newEpisode);
                                    var subtitles = (await _subtitleService.SearchSubtitlesFromImdb(
                                        languages.Select(lang => lang.SubLanguageId)
                                            .Aggregate((a, b) => a + "," + b),
                                        showJson.ImdbId.Replace("tt", ""), newEpisode.Season,
                                        newEpisode.EpisodeNumber)).GroupBy(
                                        x => x.LanguageName,
                                        (k, g) =>
                                            g.Aggregate(
                                                (a, x) =>
                                                    (Convert.ToDouble(x.Rating, CultureInfo.InvariantCulture) >=
                                                     Convert.ToDouble(a.Rating, CultureInfo.InvariantCulture))
                                                        ? x
                                                        : a));
                                    newEpisode.Subtitles = subtitles.Select(async subtitle => new Database.Subtitle
                                    {
                                        ImdbId = subtitle.ImdbId,
                                        LanguageName = subtitle.LanguageName,
                                        Rating = Convert.ToDouble(subtitle.Rating, CultureInfo.InvariantCulture),
                                        Bad = Convert.ToDouble(subtitle.Bad, CultureInfo.InvariantCulture),
                                        Iso639 = subtitle.ISO639,
                                        LanguageId = subtitle.LanguageId,
                                        OsdbSubtitleId = subtitle.SubtitleId,
                                        SubtitleDownloadLink = await _subtitleService.DownloadSubtitleToPath(
                                            subtitle.SubtitleId, subtitle.ImdbId,
                                            subtitle.ISO639,
                                            $@"shows/{subtitle.ImdbId}/{subtitle.ISO639}/{subtitle.SubtitleId}" +
                                            ".srt", subtitle.SubTitleDownloadLink.OriginalString, ExportType.Shows),
                                        SubtitleFileName = subtitle.SubtitleId + "." +
                                                           subtitle.SubTitleDownloadLink.OriginalString.Split('.')
                                                               .Last()
                                    }).Select(a => a.Result).ToList();
                                }

                                if (existingEntity.Episodes.Any())
                                {
                                    var lastEpisode = existingEntity.Episodes.OrderBy(a => a.FirstAired).Last();
                                    existingEntity.LastUpdated = lastEpisode.FirstAired;
                                }
                            }

                            await context.SaveChangesAsync().ConfigureAwait(false);
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
        /// Update images and similar shows for a show
        /// </summary>
        /// <param name="show">Show to update</param>
        /// <returns></returns>
        private async Task UpdateImagesAndSimilarShow(Show show)
        {
            if (int.TryParse(show.TvdbId, out _))
            {
                var search = await TmdbClient.SearchTvShowAsync(show.Title).ConfigureAwait(false);
                if (search.TotalResults != 0)
                {
                    var result = search.Results.FirstOrDefault();
                    if (result == null) return;
                    var tmdbShow =
                        await TmdbClient.GetTvShowAsync(result.Id, TvShowMethods.Images | TvShowMethods.Similar)
                            .ConfigureAwait(false);
                    if (tmdbShow.Images?.Backdrops != null && tmdbShow.Images.Backdrops.Any())
                    {
                        var backdrop = GetImagePathFromTmdb(TmdbClient,
                            tmdbShow.Images.Backdrops.Aggregate((image1, image2) =>
                                image1 != null && image2 != null && image1.VoteCount < image2.VoteCount
                                    ? image2
                                    : image1).FilePath);
                        show.Images.Banner =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/banner/{backdrop.Split('/').Last()}",
                                backdrop).ConfigureAwait(false);
                    }

                    if (tmdbShow.Images?.Posters != null && tmdbShow.Images.Posters.Any())
                    {
                        var poster = GetImagePathFromTmdb(TmdbClient,
                            tmdbShow.Images.Posters.Aggregate((image1, image2) =>
                                image1 != null && image2 != null && image1.VoteCount < image2.VoteCount
                                    ? image2
                                    : image1).FilePath);
                        show.Images.Poster =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/poster/{poster.Split('/').Last()}",
                                poster).ConfigureAwait(false);
                    }

                    if (tmdbShow.Similar.Results.Any())
                    {
                        show.Similars = new List<Similar>();
                        foreach (var id in tmdbShow.Similar.Results.Select(a => a.Id))
                        {
                            try
                            {
                                var externalIds = await TmdbClient.GetTvShowExternalIdsAsync(id).ConfigureAwait(false);
                                if (externalIds != null)
                                {
                                    show.Similars.Add(new Similar
                                    {
                                        TmdbId = externalIds.ImdbId
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