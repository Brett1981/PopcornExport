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
using Newtonsoft.Json;
using PopcornExport.Models.Image;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

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
            foreach(var document in documents)
            {
                try
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    // Deserialize a document to an anime
                    var anime = BsonSerializer.Deserialize<AnimeBson>(document);

                    await RetrieveAssets(document, anime);

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

                    // Update anime
                    await collectionMovies.FindOneAndUpdateAsync(filter, update, upsert);
                    watch.Stop();
                    updatedAnimes++;
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(
                        $"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} UPDATED ANIME {anime.Title} in {watch.ElapsedMilliseconds} ms. {updatedAnimes}/{documents.Count}");
                }
                catch (Exception ex)
                {
                    _loggingService.Telemetry.TrackException(ex);
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing animes.");

            var loggingTraceEnd =
                $@"Import animes ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }

        /// <summary>
        /// Retrieve assets for the provided anime
        /// </summary>
        /// <param name="document"><see cref="BsonDocument"/> to update</param>
        /// <param name="anime">Anime to process</param>
        /// <returns></returns>
        private async Task RetrieveAssets(BsonDocument document, AnimeBson anime)
        {
            using (var client = new RestClient(Constants.KitsuApiUrl))
            {
                var request = new RestRequest("{segment}", Method.GET);
                request.AddUrlSegment("segment", $"{document["_id"]}");
                var response = await client.Execute(request);
                var animeKitsu = JsonConvert.DeserializeObject<AnimeKitsuWrapperJson>(response.Content).Anime;
                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                    {
                        if (animeKitsu.Attributes.CoverImage != null)
                        {
                            anime.Images.Cover = new AnimeKitsuImage();
                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Tiny))
                            {
                                var tinyCover = string.Concat(animeKitsu.Attributes.CoverImage.Tiny
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Tiny =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/tiny/{tinyCover}",
                                        animeKitsu.Attributes.CoverImage.Tiny);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Small))
                            {
                                var smallCover = string.Concat(animeKitsu.Attributes.CoverImage.Small
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Small =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/small/{smallCover}",
                                        animeKitsu.Attributes.CoverImage.Small);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Medium))
                            {
                                var mediumCover = string.Concat(animeKitsu.Attributes.CoverImage.Medium
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Medium =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/medium/{mediumCover}",
                                        animeKitsu.Attributes.CoverImage.Medium);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Large))
                            {
                                var largeCover = string.Concat(animeKitsu.Attributes.CoverImage.Large
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Large =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/large/{largeCover}",
                                        animeKitsu.Attributes.CoverImage.Large);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Original))
                            {
                                var originalCover = string.Concat(animeKitsu.Attributes.CoverImage.Original
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Original =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/original/{originalCover}",
                                        animeKitsu.Attributes.CoverImage.Original);
                            }
                        }
                    }),
                    Task.Run(async () =>
                    {
                        if (animeKitsu.Attributes.PosterImage != null)
                        {
                            anime.Images.Poster = new AnimeKitsuImage();
                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Tiny))
                            {
                                var tinyPoster = string.Concat(animeKitsu.Attributes.PosterImage.Tiny
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Tiny =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/tiny/{tinyPoster}",
                                        animeKitsu.Attributes.PosterImage.Tiny);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Small))
                            {
                                var smallPoster = string.Concat(animeKitsu.Attributes.PosterImage.Small
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Small =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/small/{smallPoster}",
                                        animeKitsu.Attributes.PosterImage.Small);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Medium))
                            {
                                var mediumPoster = string.Concat(animeKitsu.Attributes.PosterImage.Medium
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Medium =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/medium/{mediumPoster}",
                                        animeKitsu.Attributes.PosterImage.Medium);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Large))
                            {
                                var largePoster = string.Concat(animeKitsu.Attributes.PosterImage.Large
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Large =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/large/{largePoster}",
                                        animeKitsu.Attributes.PosterImage.Large);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Original))
                            {
                                var originalPoster = string.Concat(animeKitsu.Attributes.PosterImage.Original
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Original =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/original/{originalPoster}",
                                        animeKitsu.Attributes.PosterImage.Original);
                            }
                        }
                    })
                };

                await Task.WhenAll(tasks);
            }
        }
    }
}