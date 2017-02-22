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

namespace PopcornExport.Services.Import
{
    public sealed class ImportMoviesService : IImportService
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
        /// Instanciate a <see cref="ImportMoviesService"/>
        /// </summary>
        /// <param name="mongoDbService">MongoDb service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportMoviesService(IMongoDbService<BsonDocument> mongoDbService, ILoggingService loggingService)
        {
            _mongoDbService = mongoDbService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Import movies to database
        /// </summary>
        /// <param name="documents">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> documents)
        {
            var loggingTraceBegin = $@"Import {documents.Count()} movies started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var watch = new Stopwatch();
            var updatedmovies = 0;

            foreach (var document in documents)
            {
                try
                {
                    // Deserialize a document to a movie
                    var movie = BsonSerializer.Deserialize<MovieModel>(document);

                    // Set filter to search a movie in database
                    var filter = Builders<BsonDocument>.Filter.Eq("imdb_id", movie.ImdbId);

                    // Set udpate builder to update a movie
                    var update = Builders<BsonDocument>.Update.Set("imdb_id", movie.ImdbId)
                                                                .Set("title", movie.Title)
                                                                .Set("year", movie.Year)
                                                                .Set("synopsis", movie.Synopsis)
                                                                .Set("runtime", movie.Runtime)
                                                                .Set("released", movie.Released)
                                                                .Set("trailer", movie.Trailer)
                                                                .Set("certification", movie.Certification)
                                                                .Set("torrents", movie.Torrents)
                                                                .Set("genres", movie.Genres)
                                                                .Set("images", movie.Images)
                                                                .Set("rating", movie.Rating);

                    // If a movie does not exist in database, create it
                    var upsert = new FindOneAndUpdateOptions<BsonDocument>()
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
                    Console.Write($"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("  UPDATED  ");

                    // Sum up
                    Console.ResetColor();
                    Console.Write($"{movie.Title} in {watch.ElapsedMilliseconds} ms.");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"  {updatedmovies}/{documents.Count()}");
                    Console.ResetColor();
                    Console.WriteLine(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    _loggingService.Telemetry.TrackException(ex);
                }
            }

            // Finish
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done processing movies.");
            Console.ResetColor();

            var loggingTraceEnd = $@"Import movies ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }
    }
}
