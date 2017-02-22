using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PopcornExport.Helpers;
using PopcornExport.Models.Show;
using PopcornExport.Services.Database;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
        /// MongoDb service
        /// </summary>
        private readonly IMongoDbService<BsonDocument> _mongoDbService;

        /// <summary>
        /// Instanciate a <see cref="ImportShowService"/>
        /// </summary>
        /// <param name="mongoDbService">MongoDb service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportShowService(IMongoDbService<BsonDocument> mongoDbService, ILoggingService loggingService)
        {
            _mongoDbService = mongoDbService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Import shows to database
        /// </summary>
        /// <param name="documents">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> documents)
        {
            var loggingTraceBegin = $@"Import {documents.Count()} shows started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var watch = new Stopwatch();
            var updatedshows = 0;

            foreach (var document in documents)
            {
                try
                {
                    // Deserialize a document to a show
                    var show = BsonSerializer.Deserialize<ShowModel>(document);

                    // Set filter to search a show in database
                    var filter = Builders<BsonDocument>.Filter.Eq("imdb_id", show.ImdbId);

                    // Set udpate builder to update a show
                    var update = Builders<BsonDocument>.Update.Set("imdb_id", show.ImdbId)
                                                                .Set("tvdb_id", show.TvdbId)
                                                                .Set("title", show.Title)
                                                                .Set("year", show.Year)
                                                                .Set("slug", show.Slug)
                                                                .Set("synopsis", show.Synopsis)
                                                                .Set("runtime", show.Runtime)
                                                                .Set("country", show.Country)
                                                                .Set("network", show.Network)
                                                                .Set("air_day", show.AirDay)
                                                                .Set("air_time", show.AirTime)
                                                                .Set("status", show.Status)
                                                                .Set("num_seasons", show.NumSeasons)
                                                                .Set("last_updated", show.LastUpdated)
                                                                .Set("__v", show.V)
                                                                .Set("episodes", show.Episodes)
                                                                .Set("genres", show.Genres)
                                                                .Set("images", show.Images)
                                                                .Set("rating", show.Rating);

                    // If a show does not exist in database, create it
                    var upsert = new FindOneAndUpdateOptions<BsonDocument>()
                    {
                        IsUpsert = true
                    };

                    // Retrieve shows from database
                    var collectionShows = _mongoDbService.GetCollection(Constants.ShowsCollectionName);

                    watch.Restart();

                    // Update show
                    await collectionShows.FindOneAndUpdateAsync(filter, update, upsert);
                    watch.Stop();
                    updatedshows++;
                    Console.Write($"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("  UPDATED  ");

                    // Sum up
                    Console.ResetColor();
                    Console.Write($"{show.Title} in {watch.ElapsedMilliseconds} ms.");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"  {updatedshows}/{documents.Count()}");
                    Console.ResetColor();
                    Console.WriteLine(Environment.NewLine);
                }
                catch(Exception ex)
                {
                    _loggingService.Telemetry.TrackException(ex);
                }
            }

            // Finish
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done processing shows.");
            Console.ResetColor();

            var loggingTraceEnd = $@"Import shows ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }
    }
}