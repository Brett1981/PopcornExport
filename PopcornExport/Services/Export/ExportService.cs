using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PopcornExport.Models.Movie;
using System.Collections.Async;
using System.Collections.Concurrent;
using Polly;
using PopcornExport.Models.Show;
using RestSharp;
using ShellProgressBar;
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
        /// <param name="pbar"><see cref="IProgressBar"/></param>
        /// <returns>Bson documents</returns>
        public async Task<IEnumerable<string>> LoadExport(ExportType exportType, IProgressBar pbar)
        {
            var export = new ConcurrentBag<string>();
            try
            {
                var workBarOptions = new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Yellow,
                    ProgressCharacter = '─',
                    BackgroundColor = ConsoleColor.DarkGray,
                };
                using (var childProgress = pbar?.Spawn(0, "step export progress", workBarOptions))
                {
                    if (exportType == ExportType.Shows)
                    {
                        var client = new RestClient(Constants.TvShowApi);
                        var request = new RestRequest("{segment}", Method.GET);
                        switch (exportType)
                        {
                            case ExportType.Shows:
                                request.AddUrlSegment("segment", "shows");
                                break;
                        }

                        // Execute request
                        var response = await client.ExecuteGetTaskAsync<IEnumerable<string>>(request);
                        if (childProgress != null)
                            childProgress.MaxTicks = response.Data.Count() * 50;

                        foreach (var line in response.Data)
                        {
                            var innerClient = new RestClient(Constants.TvShowApi);
                            var innerRequest = new RestRequest("{segment}/{subsegment}", Method.GET);
                            var args = line.Split('/');
                            innerRequest.AddUrlSegment("segment", args[0]);
                            innerRequest.AddUrlSegment("subsegment", args[1]);
                            var innerResponse =
                                await innerClient.ExecuteGetTaskAsync<IEnumerable<ShowJson>>(innerRequest);
                            if (innerResponse?.Data == null)
                            {
                                for (int i = 0; i < 50; i++)
                                    childProgress?.Tick();

                                continue;
                            }

                            var imdbIds = innerResponse.Data.Select(a => a.ImdbId);
                            await imdbIds.ParallelForEachAsync(async imdbId =>
                            {
                                var showClient = new RestClient(Constants.TvShowApi);
                                var showRequest = new RestRequest("{segment}/{subsegment}", Method.GET);
                                showRequest.AddUrlSegment("segment", "show");
                                showRequest.AddUrlSegment("subsegment", imdbId);
                                var showResponse = await showClient.ExecuteGetTaskAsync(showRequest);
                                export.Add(showResponse.Content);
                                childProgress?.Tick();
                            });
                        }
                    }
                    else if (exportType == ExportType.Movies)
                    {
                        var page = 1;
                        bool movieFound;
                        do
                        {
                            var client = new RestClient(Constants.YtsApiUrl);
                            var moviesByPageRequest = GetMoviesByPageRequest(page);
                            // Execute request
                            var movieShortResponse =
                                await client.ExecuteGetTaskAsync(moviesByPageRequest);
                            var movieNode =
                                JsonSerializer.Deserialize<MovieShortJsonNode>(movieShortResponse.RawBytes);
                            if (movieNode?.Data?.Movies == null || !movieNode.Data.Movies.Any())
                            {
                                movieFound = false;
                            }
                            else
                            {
                                if (childProgress != null)
                                    childProgress.MaxTicks = movieNode.Data.MovieCount;

                                movieFound = true;
                                page++;
                                await movieNode.Data.Movies.ParallelForEachAsync(async movie =>
                                {
                                    try
                                    {
                                        var innerClient = new RestClient(Constants.YtsApiUrl);
                                        var movieByIdRequest = GetMovieById(movie.Id);

                                        var retrySearchSubtitlesFromImdbPolicy = Policy
                                            .Handle<Exception>()
                                            .WaitAndRetryAsync(5, retryAttempt =>
                                                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                            );
                                        var movieFullResponse =
                                            await retrySearchSubtitlesFromImdbPolicy.ExecuteAsync(async () =>
                                                await innerClient.ExecuteGetTaskAsync(movieByIdRequest));
                                        var fullMovie =
                                            JsonSerializer.Deserialize<MovieFullJsonNode>(
                                                movieFullResponse.Content);
                                        export.Add(JsonSerializer.ToJsonString(fullMovie.Data.Movie));
                                        childProgress?.Tick();
                                    }
                                    catch (Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                    }
                                });
                            }
                        } while (movieFound);
                    }

                    pbar?.Tick();
                    return export;
                }

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
            request.AddQueryParameter("movie_id", movieId.ToString());
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
            request.AddQueryParameter("limit", 50.ToString());
            request.AddQueryParameter("page", page.ToString());
            return request;
        }
    }
}