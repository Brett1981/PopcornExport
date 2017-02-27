using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PopcornExport.Helpers;
using PopcornExport.Models.Movie;
using PopcornExport.Services.Database;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Services.Assets;

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
        /// MongoDb service
        /// </summary>
        private readonly IMongoDbService<BsonDocument> _mongoDbService;

        /// <summary>
        /// Assets service
        /// </summary>
        private readonly IAssetsService _assetsService;

        /// <summary>
        /// Instanciate a <see cref="ImportMovieService"/>
        /// </summary>
        /// <param name="mongoDbService">MongoDb service</param>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportMovieService(IMongoDbService<BsonDocument> mongoDbService, IAssetsService assetsService, ILoggingService loggingService)
        {
            _mongoDbService = mongoDbService;
            _loggingService = loggingService;
            _assetsService = assetsService;
        }

        /// <summary>
        /// Import movies to database
        /// </summary>
        /// <param name="docs">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> docs)
        {
            var documents = docs.ToList();
            var loggingTraceBegin =
                $@"Import {documents.Count} movies started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var watch = new Stopwatch();
            var updatedmovies = 0;

            foreach (var document in documents)
            {
                try
                {
                    // Deserialize a document to a movie
                    var movie = BsonSerializer.Deserialize<MovieBson>(document);
                    if (!string.IsNullOrEmpty(movie.BackgroundImage))
                        movie.BackgroundImage = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/background/{movie.BackgroundImage.Split('/').Last()}", movie.BackgroundImage);

                    if (!string.IsNullOrEmpty(movie.SmallCoverImage))
                        movie.SmallCoverImage = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/cover/small/{movie.SmallCoverImage.Split('/').Last()}", movie.SmallCoverImage);

                    if (!string.IsNullOrEmpty(movie.MediumCoverImage))
                        movie.MediumCoverImage = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/cover/medium/{movie.MediumCoverImage.Split('/').Last()}", movie.MediumCoverImage);

                    if (!string.IsNullOrEmpty(movie.LargeCoverImage))
                        movie.LargeCoverImage = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/cover/large/{movie.LargeCoverImage.Split('/').Last()}", movie.LargeCoverImage);

                    if (!string.IsNullOrEmpty(movie.MediumScreenshotImage1))
                        movie.MediumScreenshotImage1 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/medium/1/{movie.MediumScreenshotImage1.Split('/').Last()}", movie.MediumScreenshotImage1);

                    if (!string.IsNullOrEmpty(movie.MediumScreenshotImage2))
                        movie.MediumScreenshotImage2 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/medium/2/{movie.MediumScreenshotImage2.Split('/').Last()}", movie.MediumScreenshotImage2);

                    if (!string.IsNullOrEmpty(movie.MediumScreenshotImage3))
                        movie.MediumScreenshotImage3 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/medium/3/{movie.MediumScreenshotImage3.Split('/').Last()}", movie.MediumScreenshotImage3);

                    if (!string.IsNullOrEmpty(movie.LargeScreenshotImage1))
                        movie.LargeScreenshotImage1 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/large/1/{movie.LargeScreenshotImage1.Split('/').Last()}", movie.LargeScreenshotImage1);

                    if (!string.IsNullOrEmpty(movie.LargeScreenshotImage2))
                        movie.LargeScreenshotImage2 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/large/2/{movie.LargeScreenshotImage2.Split('/').Last()}", movie.LargeScreenshotImage2);

                    if (!string.IsNullOrEmpty(movie.LargeScreenshotImage3))
                        movie.LargeScreenshotImage3 = await _assetsService.UploadFile($@"images/{movie.ImdbCode}/screenshot/large/3/{movie.LargeScreenshotImage3.Split('/').Last()}", movie.LargeScreenshotImage3);

                    foreach (var torrent in movie.Torrents)
                    {
                        torrent.Url =
                            await _assetsService.UploadFile($@"torrents/{movie.ImdbCode}/{movie.ImdbCode}.torrent",
                                torrent.Url);
                    }

                    // Set filter to search a movie in database
                    var filter = Builders<BsonDocument>.Filter.Eq("imdb_code", movie.ImdbCode);

                    // Set udpate builder to update a movie
                    var update = Builders<BsonDocument>.Update.Set("imdb_code", movie.ImdbCode)
                        .Set("url", movie.Url)
                        .Set("imdb_code", movie.ImdbCode)
                        .Set("title", movie.Title)
                        .Set("title_long", movie.TitleLong)
                        .Set("year", movie.Year)
                        .Set("slug", movie.Slug)
                        .Set("rating", movie.Rating)
                        .Set("runtime", movie.Runtime)
                        .Set("genres", movie.Genres)
                        .Set("language", movie.Language)
                        .Set("mpa_rating", movie.MpaRating)
                        .Set("download_count", movie.DownloadCount)
                        .Set("like_count", movie.LikeCount)
                        .Set("description_intro", movie.DescriptionIntro)
                        .Set("description_full", movie.DescriptionFull)
                        .Set("yt_trailer_code", movie.YtTrailerCode)
                        .Set("cast", movie.Cast)
                        .Set("torrents", movie.Torrents)
                        .Set("date_uploaded", movie.DateUploaded)
                        .Set("date_uploaded_unix", movie.DateUploadedUnix)
                        .Set("background_image", movie.BackgroundImage)
                        .Set("small_cover_image", movie.SmallCoverImage)
                        .Set("medium_cover_image", movie.MediumCoverImage)
                        .Set("large_cover_image", movie.LargeCoverImage)
                        .Set("medium_screenshot_image1", movie.MediumScreenshotImage1)
                        .Set("medium_screenshot_image2", movie.MediumScreenshotImage2)
                        .Set("medium_screenshot_image3", movie.MediumScreenshotImage3)
                        .Set("large_screenshot_image1", movie.LargeScreenshotImage1)
                        .Set("large_screenshot_image2", movie.LargeScreenshotImage2)
                        .Set("large_screenshot_image3", movie.LargeScreenshotImage3);

                    // If a movie does not exist in database, create it
                    var upsert = new FindOneAndUpdateOptions<BsonDocument>
                    {
                        IsUpsert = true
                    };

                    // Retrieve movies from database
                    var collectionMovies = _mongoDbService.GetCollection(Constants.MoviesCollectionName);

                    watch.Restart();

                    // Update movie
                    await collectionMovies.FindOneAndUpdateAsync(filter, update, upsert);
                    watch.Stop();
                    updatedmovies++;
                    Console.WriteLine(Environment.NewLine);
                    Console.Write($"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("  UPDATED MOVIE ");

                    // Sum up
                    Console.ResetColor();
                    Console.Write($"{movie.Title} in {watch.ElapsedMilliseconds} ms.");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"  {updatedmovies}/{documents.Count}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    _loggingService.Telemetry.TrackException(ex);
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done processing movies.");
            Console.ResetColor();

            var loggingTraceEnd =
                $@"Import movies ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }
    }
}