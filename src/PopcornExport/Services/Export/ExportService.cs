using MongoDB.Bson;
using PopcornExport.Extensions;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.Logging;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Export
{
    /// <summary>
    /// Export service
    /// </summary>
    public sealed class ExportService : IExportService
    {
        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// The export service
        /// </summary>
        /// <param name="loggingService">The logging service</param>
        public ExportService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Load an export
        /// </summary>
        /// <param name="exportType">Export to load</param>
        /// <returns>Bson documents</returns>
        public async Task<List<BsonDocument>> LoadExport(ExportType exportType)
        {
            try
            {
                var loggingTraceBegin =
                    $@"Export {exportType.ToFriendlyString()} started at {DateTime.UtcNow.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

                using (var client = new RestClient(Constants.PopcornApiFetchUrl))
                {
                    var request = new RestRequest("{segment}", Method.GET);
                    switch (exportType)
                    {
                        case ExportType.Anime:
                            request.AddUrlSegment("segment", "anime");
                            break;
                        case ExportType.Movies:
                            request.AddUrlSegment("segment", "movie");
                            break;
                        case ExportType.Shows:
                            request.AddUrlSegment("segment", "show");
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    // Execute request
                    var response = await client.Execute(request);

                    // Load response into memory
                    using (var reader = new StreamReader(new MemoryStream(response.RawBytes), Encoding.UTF8))
                    {
                        string line;
                        var export = new List<BsonDocument>();

                        // Read all response parts
                        while ((line = reader.ReadLine()) != null)
                        {
                            BsonDocument document;
                            // Try to parse a document
                            if (BsonDocument.TryParse(line, out document))
                            {
                                export.Add(document);
                            }
                        }

                        var loggingTraceEnd =
                            $@"Export {export.Count} {exportType.ToFriendlyString()} ended at {DateTime.UtcNow.ToString(
                                "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                        _loggingService.Telemetry.TrackTrace(loggingTraceEnd);

                        return export;
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return null;
            }
        }
    }
}