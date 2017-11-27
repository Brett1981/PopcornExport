using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PopcornExport.Helpers;
using PopcornExport.Models.Movie;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Database;
using PopcornExport.Services.Assets;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using Microsoft.EntityFrameworkCore;
using System.Collections.Async;
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
        /// Constructor
        /// </summary>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportMovieService(IAssetsService assetsService,
            ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _assetsService = assetsService;

            TmdbClient = new TMDbClient(Constants.TmDbClientId)
            {
                MaxRetryCount = 10
            };

            try
            {
                TmdbClient.GetConfig();
            }
            catch (Exception)
            {
                //TODO
            }
        }

        /// <summary>
        /// TMDb client
        /// </summary>
        private TMDbClient TmdbClient { get; set; }

        /// <summary>
        /// Import movies to database
        /// </summary>
        /// <param name="docs">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> docs)
        {
            var documents = docs.ToList();
            var loggingTraceBegin =
                $@"Import {documents.Count} movies started at {
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff",
                            CultureInfo.InvariantCulture)
                    }";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var updatedMovies = 0;
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                foreach (var document in documents)
                {
                    try
                    {
                        var watch = new Stopwatch();
                        watch.Start();

                        // Deserialize a document to a movie
                        var movieJson =
                            BsonSerializer.Deserialize<MovieBson>(document);

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

                        if (!context.MovieSet.Any(a => a.ImdbCode == movie.ImdbCode))
                        {
                            await RetrieveAssets(TmdbClient, movie);
                            context.MovieSet.Add(movie);
                        }
                        else
                        {
                            var existingEntity =
                                await context.MovieSet.Include(a => a.Torrents)
                                    .Include(a => a.Cast)
                                    .Include(a => a.Genres).Include(a => a.Similars)
                                    .FirstOrDefaultAsync(a => a.ImdbCode == movie.ImdbCode);

                            foreach (var torrent in existingEntity.Torrents)
                            {
                                var updatedTorrent = movie.Torrents.FirstOrDefault(a => a.Quality == torrent.Quality);
                                if (updatedTorrent == null) continue;
                                torrent.Peers = updatedTorrent.Peers;
                                torrent.Seeds = updatedTorrent.Seeds;
                                if (string.IsNullOrEmpty(torrent.Url))
                                {
                                    torrent.Url = updatedTorrent.Url;
                                }
                            }

                            existingEntity.Cast = movieJson.Cast?.Select(cast => new Database.Cast
                            {
                                ImdbCode = cast?.ImdbCode,
                                SmallImage = cast?.SmallImage,
                                CharacterName = cast?.CharacterName,
                                Name = cast?.Name
                            }).ToList();
                            await RetrieveAssets(TmdbClient, existingEntity);
                        }

                        await context.SaveChangesAsync();
                        watch.Stop();
                        updatedMovies++;
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(
                            $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} UPDATED MOVIE {movie.Title} in {watch.ElapsedMilliseconds} ms. {updatedMovies}/{documents.Count}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                    }
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing movies.");

            var loggingTraceEnd =
                $@"Import movies ended at {
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff",
                            CultureInfo.InvariantCulture)
                    }";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
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
        /// <param name="tmdbClient"><see cref="TMDbClient"/></param>
        /// <param name="movie">Movie to update</param>
        /// <returns></returns>
        private async Task RetrieveAssets(TMDbClient tmdbClient, Movie movie)
        {
            var tmdbMovie = await tmdbClient.GetMovieAsync(movie.ImdbCode, MovieMethods.Images | MovieMethods.Similar);
            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
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
                                backdrop);
                    }
                }),
                Task.Run(async () =>
                {
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
                                poster);
                    }
                }),
                Task.Run(async () =>
                {
                    if (movie.Torrents != null)
                    {
                        foreach (var torrent in movie.Torrents)
                        {
                            torrent.Url =
                                await _assetsService.UploadFile(
                                    $@"torrents/{movie.ImdbCode}/{torrent.Quality}/{movie.ImdbCode}.torrent",
                                    torrent.Url);
                        }
                    }
                }),
                Task.Run(async () =>
                {
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
                                        }", cast.SmallImage);
                            }
                        }
                    }
                })
            };

            await Task.WhenAll(tasks);

            if (tmdbMovie.Similar.TotalResults != 0)
            {
                movie.Similars = new List<Similar>();
                await tmdbMovie.Similar.Results.Select(a => a.Id)
                    .ParallelForEachAsync(async id =>
                    {
                        try
                        {
                            var res = await TmdbClient.GetMovieAsync(id);
                            if (!string.IsNullOrEmpty(res?.ImdbId))
                            {
                                movie.Similars.Add(new Similar
                                {
                                    TmdbId = res.ImdbId
                                });
                            }
                        }
                        catch (Exception)
                        {

                        }
                    });
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