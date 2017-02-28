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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PopcornExport.Models.Movie;
using System.Collections.Async;
using System.Collections.Concurrent;

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

                var export = new ConcurrentBag<BsonDocument>();
                if (exportType == ExportType.Anime || exportType == ExportType.Shows)
                {
                    using (var client = new RestClient(Constants.OriginalPopcornApi))
                    {
                        var request = new RestRequest("{segment}", Method.GET);
                        switch (exportType)
                        {
                            case ExportType.Anime:
                                request.AddUrlSegment("segment", "anime");
                                break;
                            case ExportType.Shows:
                                request.AddUrlSegment("segment", "show");
                                break;
                        }

                        // Execute request
                        var response = await client.Execute(request);
                        // Load response into memory
                        using (var reader = new StreamReader(new MemoryStream(response.RawBytes), Encoding.UTF8))
                        {
                            string line;
                            // Read all response parts
                            while ((line = reader.ReadLine()) != null)
                            {
                                ConvertJsonToBsonDocument(line, export);
                            }
                        }
                    }
                }
                else if (exportType == ExportType.Movies)
                {
                    var page = 1;
                    bool movieFound;
                    do
                    {
                        using (var client = new RestClient(Constants.YtsApiUrl))
                        {
                            var moviesByPageRequest = GetMoviesByPageRequest(page);
                            // Execute request
                            var response = await client.Execute<MovieShortJsonNode>(moviesByPageRequest);
                            if (response?.Data?.Data?.Movies == null || !response.Data.Data.Movies.Any())
                            {
                                movieFound = false;
                            }
                            else
                            {
                                movieFound = true;
                                page++;
                                await response.Data.Data.Movies.ParallelForEachAsync(async movie => 
                                {
                                    try 
                                    {
                                        var movieByIdRequest = GetMovieById(movie.Id);
                                        var fullMovie = await client.Execute<MovieFullJsonNode>(movieByIdRequest);
                                        ConvertJsonToBsonDocument(JsonConvert.SerializeObject(fullMovie.Data.Data.Movie),
                                            export);
                                    }
                                    catch(Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                    }
                                }, 50, false);
                            }
                        }
                    } while (movieFound);
                }

                var loggingTraceEnd =
                    $@"Export {export.Count} {exportType.ToFriendlyString()} ended at {DateTime.UtcNow.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceEnd);

                return export.ToList();

            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get movie by Id
        /// </summary>
        /// <param name="movieId">Movie Id to get</param>
        /// <returns><see cref="RestRequest"/></returns>
        private RestRequest GetMovieById(int movieId)
        {
            var request = new RestRequest("{segment}", Method.GET);
            request.AddUrlSegment("segment", "movie_details.json");
            request.AddQueryParameter("movie_id", movieId);
            request.AddQueryParameter("with_images", "true");
            request.AddQueryParameter("with_cast", "true");
            return request;
        }

        /// <summary>
        /// Get movies by page
        /// </summary>
        /// <param name="page">Page to fetch</param>
        /// <returns><see cref="RestRequest"/></returns>
        private RestRequest GetMoviesByPageRequest(int page)
        {
            var request = new RestRequest("{segment}", Method.GET);
            request.AddUrlSegment("segment", "list_movies.json");
            request.AddQueryParameter("limit", 50);
            request.AddQueryParameter("page", page);
            return request;
        }

        /// <summary>
        /// Convert json to BsonDocument
        /// </summary>
        /// <param name="json">Json to convert</param>
        /// <param name="export">BsonDocument to update</param>
        private void ConvertJsonToBsonDocument(string json, ConcurrentBag<BsonDocument> export)
        {
            BsonDocument document;
            // Try to parse a document
            if (BsonDocument.TryParse(json, out document))
            {
                export.Add(document);
            }
        }
    }
}