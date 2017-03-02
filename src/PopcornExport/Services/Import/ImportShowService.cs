using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PopcornExport.Helpers;
using PopcornExport.Services.Database;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Models.Show;
using PopcornExport.Services.Assets;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

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
        /// DocumentDb service
        /// </summary>
        private readonly IDocumentDbService _documentDbService;

        /// <summary>
        /// Assets service
        /// </summary>
        private readonly IAssetsService _assetsService;

        /// <summary>
        /// Instanciate a <see cref="ImportShowService"/>
        /// </summary>
        /// <param name="documentDbService">MongoDb service</param>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportShowService(IDocumentDbService documentDbService, IAssetsService assetsService,
            ILoggingService loggingService)
        {
            _documentDbService = documentDbService;
            _assetsService = assetsService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Import shows to database
        /// </summary>
        /// <param name="docs">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> docs)
        {
            var documents = docs.ToList();
            var loggingTraceBegin =
                $@"Import {documents.Count} shows started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var updatedShows = 0;
            using (var client = _documentDbService.Client)
            {
                foreach (var document in documents)
                {
                    try
                    {
                        var watch = new Stopwatch();
                        watch.Start();

                        // Deserialize a document to a show
                        var show =
                            JsonConvert.DeserializeObject<ShowJson>(
                                BsonSerializer.Deserialize<ShowBson>(document).ToJson());

                        await RetrieveAssets(show);

                        await client.UpsertDocumentAsync(
                            UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.ShowsCollectionName),
                            show);

                        watch.Stop();
                        updatedShows++;
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(
                            $"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} UPDATED SHOW {show.Title} in {watch.ElapsedMilliseconds} ms. {updatedShows}/{documents.Count}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                    }
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing shows.");

            var loggingTraceEnd =
                $@"Import shows ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }

        /// <summary>
        /// Retrieve images for the provided show
        /// </summary>
        /// <param name="show">Show to process</param>
        /// <returns><see cref="Task"/></returns>
        private async Task RetrieveAssets(ShowJson show)
        {
            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Banner))
                        show.Images.Banner =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/banner/{show.Images.Banner.Split('/').Last()}",
                                show.Images.Banner);
                }),
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Fanart))
                        show.Images.Fanart =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/fanart/{show.Images.Fanart.Split('/').Last()}",
                                show.Images.Fanart);
                }),
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Poster))
                        show.Images.Poster =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/poster/{show.Images.Poster.Split('/').Last()}",
                                show.Images.Poster);
                })
            };

            await Task.WhenAll(tasks);
        }
    }
}