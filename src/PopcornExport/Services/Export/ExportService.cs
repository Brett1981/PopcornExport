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
using PopcornExport.Models.Movie;
using System.Collections.Async;
using System.Collections.Concurrent;
using PopcornExport.Services.Caching;
using Utf8Json;

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
        public async Task<IEnumerable<BsonDocument>> LoadExport(ExportType exportType)
        {
            var export = new ConcurrentBag<BsonDocument>();
            try
            {
                var loggingTraceBegin =
                    $@"Export {exportType.ToFriendlyString()} started at {DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

                if (exportType == ExportType.Shows)
                {
                    using (var client = new RestClient(Constants.OriginalPopcornApi))
                    {
                        var request = new RestRequest("{segment}", Method.GET);
                        switch (exportType)
                        {
                            case ExportType.Shows:
                                request.AddUrlSegment("segment", "show");
                                break;
                        }

                        // Execute request
                        var response = await client.Execute(request);
                        // Load response into memory
                        using(var data = new MemoryStream(response.RawBytes))
                        using (var reader = new StreamReader(data, Encoding.UTF8))
                        {
                            string line;
                            // Read all response parts
                            while ((line = await reader.ReadLineAsync()) != null)
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
                            var movieShortResponse = await client.Execute(moviesByPageRequest);
                            var movieNode = JsonSerializer.Deserialize<MovieShortJsonNode>(movieShortResponse.RawBytes);
                            if (movieNode?.Data?.Movies == null || !movieNode.Data.Movies.Any())
                            {
                                movieFound = false;
                            }
                            else
                            {
                                movieFound = true;
                                page++;
                                await movieNode.Data.Movies.ParallelForEachAsync(async movie =>
                                {
                                    try
                                    {
                                        using (var innerClient = new RestClient(Constants.YtsApiUrl))
                                        {
                                            var movieByIdRequest = GetMovieById(movie.Id);
                                            var movieFullResponse =
                                                await innerClient.Execute(movieByIdRequest);
                                            var fullMovie =
                                                JsonSerializer.Deserialize<MovieFullJsonNode>(
                                                    movieFullResponse.RawBytes);
                                            ConvertJsonToBsonDocument(
                                                JsonSerializer.ToJsonString(fullMovie.Data.Movie),
                                                export);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                    }
                                });
                            }
                        }
                    } while (movieFound);
                }

                var loggingTraceEnd =
                    $@"Export {export.Count} {exportType.ToFriendlyString()} ended at {DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceEnd);

                return export;

            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return export;
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
        /// <param name="export">Bag of <see cref="BsonDocument"/> to update</param>
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