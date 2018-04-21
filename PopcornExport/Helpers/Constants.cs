namespace PopcornExport.Helpers
{
    /// <summary>
    /// Constants of the project
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Popcorn Api to import
        /// </summary>
        public const string TVShowAPI = "https://api-fetch.website/tv/";

        /// <summary>
        /// YTS Api
        /// </summary>
        public const string YtsApiUrl = "https://yts.am/api/v2/";

        /// <summary>
        /// Popcorn Api for movies
        /// </summary>
        public const string MovieV2ApiUrl = "https://tv-v2.api-fetch.website/movie/";

        /// <summary>
        /// Client ID for TMDb
        /// </summary>
        public const string TmDbClientId = "a21fe922d3bac6654e93450e9a18af1c";

        /// <summary>
        /// Application Insights key used for logging
        /// </summary>
        public const string ApplicationInsightsKey = "3705b885-bcf3-4189-a668-12055839640e";

        /// <summary>
        /// Azure Sotrage assets folder
        /// </summary>
        public const string AssetsFolder = "assets";

        /// <summary>
        /// Redis host
        /// </summary>
        public const string RedisHost = "popcorn.redis.cache.windows.net:6380";
    }
}
