namespace PopcornExport.Helpers
{
    public class Constants
    {
        /// <summary>
        /// Popcorn Api to fetch
        /// </summary>
        public const string PopcornApiFetchUrl = "https://tv-v2.api-fetch.website/exports";

        /// <summary>
        /// MongoDb Database to use as export
        /// </summary>
        public const string MongoDbName = "popcorn";

        /// <summary>
        /// Shows collection name
        /// </summary>
        public const string ShowCollectionName = "shows";

        /// <summary>
        /// Application Insights key used for logging
        /// </summary>
        public const string ApplicationInsightsKey = "SECRET";
    }
}
