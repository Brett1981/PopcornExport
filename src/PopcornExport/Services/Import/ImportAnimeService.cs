using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PopcornExport.Helpers;
using PopcornExport.Services.Database;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Models.Anime;
using PopcornExport.Services.Assets;
using System.Collections.Async;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import animes
    /// </summary>
    public sealed class ImportAnimeService : IImportService
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
        /// Instanciate a <see cref="ImportAnimeService"/>
        /// </summary>
        /// <param name="mongoDbService">MongoDb service</param>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportAnimeService(IMongoDbService<BsonDocument> mongoDbService, IAssetsService assetsService,
            ILoggingService loggingService)
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

            var updatedAnimes = 0;

            await documents.ParallelForEachAsync(async document => 
            {
                try
                {
                    var watch = new Stopwatch();
                    // Deserialize a document to an anime
                    var anime = BsonSerializer.Deserialize<AnimeBson>(document);

                    var tasks = new List<Task>
                    {
                        Task.Run(async () =>
                        {
                            if (!string.IsNullOrEmpty(anime.Images.Banner))
                                anime.Images.Banner =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/banner/{anime.Images.Banner.Split('/').Last()}",
                                        anime.Images.Banner);
                        }),
                        Task.Run(async () =>
                        {
                            if (!string.IsNullOrEmpty(anime.Images.Fanart))
                                anime.Images.Fanart =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/fanart/{anime.Images.Fanart.Split('/').Last()}",
                                        anime.Images.Fanart);
                        }),
                        Task.Run(async () =>
                        {
                            if (!string.IsNullOrEmpty(anime.Images.Poster))
                                anime.Images.Poster =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/{anime.Images.Poster.Split('/').Last()}",
                                        anime.Images.Poster);
                        })
                    };

                    await Task.WhenAll(tasks);

                    // Set filter to search an anime in database
                    var filter = Builders<BsonDocument>.Filter.Eq("mal_id", anime.MalId);

                    // Set udpate builder to update an anime
                    var update = Builders<BsonDocument>.Update.Set("mal_id", anime.MalId)
                        .Set("title", anime.Title)
                        .Set("year", anime.Year)
                        .Set("slug", anime.Slug)
                        .Set("synopsis", anime.Synopsis)
                        .Set("runtime", anime.Runtime)
                        .Set("status", anime.Status)
                        .Set("type", anime.Type)
                        .Set("last_updated", anime.LastUpdated)
                        .Set("__v", anime.V)
                        .Set("num_seasons", anime.NumSeasons)
                        .Set("episodes", anime.Episodes)
                        .Set("genres", anime.Genres)
                        .Set("images", anime.Images)
                        .Set("rating", anime.Rating);

                    // If an anime does not exist in database, create it
                    var upsert = new FindOneAndUpdateOptions<BsonDocument>
                    {
                        IsUpsert = true
                    };

                    // Retrieve animes from database
                    var collectionMovies = _mongoDbService.GetCollection(Constants.AnimeCollectionName);

                    watch.Start();

                    // Update anime
                    await collectionMovies.FindOneAndUpdateAsync(filter, update, upsert);
                    watch.Stop();
                    updatedAnimes++;
                    Console.WriteLine(Environment.NewLine);
                    Console.Write($"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
                    Console.Write("  UPDATED ANIME ");
                    Console.Write($"{anime.Title} in {watch.ElapsedMilliseconds} ms.");
                    Console.Write($"  {updatedAnimes}/{documents.Count}");
                }
                catch (Exception ex)
                {
                    _loggingService.Telemetry.TrackException(ex);
                }
            }, 10, false);

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing animes.");

            var loggingTraceEnd =
                $@"Import animes ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }
    }
}