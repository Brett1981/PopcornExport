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
        public const string OriginalPopcornApi = "https://tv-v2.api-fetch.website/exports";

        /// <summary>
        /// YTS Api
        /// </summary>
        public const string YTSApiUrl = "https://yts.ag/api/v2/";

        /// <summary>
        /// Popcorn Api to export
        /// </summary>
        public const string AzurePopcornApi = "https://popcornapi.azurewebsites.net";

        /// <summary>
        /// MongoDb Database to use as export
        /// </summary>
        public const string MongoDbName = "popcorn";

        /// <summary>
        /// Shows collection name
        /// </summary>
        public const string ShowsCollectionName = "shows";

        /// <summary>
        /// Anime collection name
        /// </summary>
        public const string AnimeCollectionName = "anime";

        /// <summary>
        /// Movies collection name
        /// </summary>
        public const string MoviesCollectionName = "movies";

        /// <summary>
        /// Application Insights key used for logging
        /// </summary>
        public const string ApplicationInsightsKey = "SECRET";
    }
}
