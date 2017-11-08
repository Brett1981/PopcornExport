using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PopcornExport.Database;
using PopcornExport.Helpers;
using PopcornExport.Services.Assets;
using PopcornExport.Services.File;
using PopcornExport.Services.Logging;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace PopcornExport.Services.Integrity
{
    public class IntegrityService : IIntegrityService
    {
        private readonly ILoggingService _loggingService;

        private readonly IAssetsService _assetsMovieService;

        private readonly IAssetsService _assetsShowService;

        public IntegrityService(ILoggingService loggingService, IFileService fileService)
        {
            _loggingService = loggingService;
            _assetsMovieService = new AssetsMovieService(fileService);
            _assetsShowService = new AssetsShowService(fileService);
        }

        public async Task Consolidate()
        {
            var loggingTraceBegin =
                $@"Consolidation started at {
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff",
                            CultureInfo.InvariantCulture)
                    }";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(loggingTraceBegin);
            var tmdbClient = new TMDbClient(Constants.TmdbClientApiKey);
            tmdbClient.GetConfig();
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                var watch = new Stopwatch();
                foreach (var movie in context.MovieSet.ToList())
                {
                    watch.Restart();
                    var tmdbMovie = await tmdbClient.GetMovieAsync(movie.ImdbCode, MovieMethods.Images);
                    if (!string.IsNullOrEmpty(tmdbMovie.BackdropPath))
                    {
                        var backdrop = GetImagePathFromTmdb(tmdbClient,
                            tmdbMovie.BackdropPath);
                        movie.BackdropImage =
                            await _assetsMovieService.UploadFile(
                                $@"images/{movie.ImdbCode}/backdrop/{backdrop.Split('/').Last()}",
                                backdrop, true);
                    }

                    if (!string.IsNullOrEmpty(tmdbMovie.PosterPath))
                    {
                        var poster = GetImagePathFromTmdb(tmdbClient, tmdbMovie.PosterPath);
                        movie.PosterImage =
                            await _assetsMovieService.UploadFile(
                                $@"images/{movie.ImdbCode}/poster/{poster.Split('/').Last()}",
                                poster, true);
                    }

                    await context.SaveChangesAsync();
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} CONSOLIDATED MOVIE {movie.Title} in {watch.ElapsedMilliseconds} ms.");
                }

                foreach (var show in context.ShowSet.ToList())
                {
                    watch.Restart();
                    var search = await tmdbClient.SearchTvShowAsync(show.Title);
                    if (search.TotalResults != 0)
                    {
                        var result = search.Results.FirstOrDefault();
                        var tmdbShow =
                            await tmdbClient.GetTvShowAsync(result.Id, TvShowMethods.Images | TvShowMethods.Similar);

                        if (!string.IsNullOrEmpty(tmdbShow.BackdropPath))
                        {
                            var backdrop = GetImagePathFromTmdb(tmdbClient,
                                tmdbShow.BackdropPath);
                            show.Images.Banner =
                                await _assetsShowService.UploadFile(
                                    $@"images/{show.ImdbId}/banner/{backdrop.Split('/').Last()}",
                                    backdrop, true);

                            if (!string.IsNullOrEmpty(tmdbShow.PosterPath))
                            {
                                var poster = GetImagePathFromTmdb(tmdbClient,
                                    tmdbShow.PosterPath);
                                show.Images.Poster =
                                    await _assetsShowService.UploadFile(
                                        $@"images/{show.ImdbId}/poster/{poster.Split('/').Last()}",
                                        poster, true);
                            }

                            if (tmdbShow.Images?.Backdrops != null && tmdbShow.Images.Backdrops.Any())
                            {
                                var fanart = GetImagePathFromTmdb(tmdbClient,
                                    tmdbShow.Images.Backdrops.Aggregate(
                                        (image1, image2) =>
                                            image1 != null && image2 != null && image1.VoteAverage < image2.VoteAverage
                                                ? image2
                                                : image1).FilePath);
                                show.Images.Fanart =
                                    await _assetsShowService.UploadFile(
                                        $@"images/{show.ImdbId}/fanart/{fanart.Split('/').Last()}",
                                        fanart, true);
                            }
                        }

                        await context.SaveChangesAsync();
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(
                            $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} CONSOLIDATED SHOW {show.Title} in {watch.ElapsedMilliseconds} ms.");
                    }
                }

                var loggingTraceEnd =
                    $@"Consolidation ended at {
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff",
                                CultureInfo.InvariantCulture)
                        }";
                _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(loggingTraceEnd);
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