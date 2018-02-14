using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Popcorn.OSDB;
using PopcornExport.Helpers;
using PopcornExport.Services.Logging;

namespace PopcornExport.Services.Subtitle
{
    public class SubtitleService : ISubtitleService
    {
        private readonly ILoggingService _loggingService;

        public SubtitleService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get subtitles languages
        /// </summary>
        /// <returns>Languages</returns>
        public async Task<IEnumerable<Popcorn.OSDB.Language>> GetSubLanguages()
        {
            var retryGetSubLanguagesPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            try
            {
                return await retryGetSubLanguagesPolicy.ExecuteAsync(async () =>
                {
                    using (var osdb = await new Osdb().Login(Constants.OsdbUa))
                    {
                        return await osdb.GetSubLanguages();
                    }
                });
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return new List<Popcorn.OSDB.Language>();
            }
        }

        /// <summary>
        /// Search subtitles by imdb code and languages
        /// </summary>
        /// <param name="languages">Languages</param>
        /// <param name="imdbId">Imdb code</param>
        /// <param name="season">Season number</param>
        /// <param name="episode">Episode number</param>
        /// <returns>Subtitles</returns>
        public async Task<IList<Popcorn.OSDB.Subtitle>> SearchSubtitlesFromImdb(string languages, string imdbId, int? season,
            int? episode)
        {
            var retrySearchSubtitlesFromImdbPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            return await retrySearchSubtitlesFromImdbPolicy.ExecuteAsync(async () =>
            {
                using (var osdb = await new Osdb().Login(Constants.OsdbUa))
                {
                    try
                    {
                        return await osdb.SearchSubtitlesFromImdb(languages, imdbId, season, episode);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                        return new List<Popcorn.OSDB.Subtitle>();
                    }
                }
            });
        }

        /// <summary>
        /// Download a subtitle to a path
        /// </summary>
        /// <param name="path">Path to download</param>
        /// <param name="subtitle">Subtitle to download</param>
        /// <param name="remote">Is remote download path</param>
        /// <returns>Downloaded subtitle path</returns>
        public async Task<string> DownloadSubtitleToPath(string path, Popcorn.OSDB.Subtitle subtitle, bool remote = true)
        {
            var retryDownloadSubtitleToPathPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            return await retryDownloadSubtitleToPathPolicy.ExecuteAsync(async () =>
            {
                using (var osdb = await new Osdb().Login(Constants.OsdbUa))
                {
                    try
                    {
                        return await osdb.DownloadSubtitleToPath(path, subtitle, remote);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                        return null;
                    }
                }
            });
        }
    }
}