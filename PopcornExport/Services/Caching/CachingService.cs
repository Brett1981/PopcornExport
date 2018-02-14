using System.Threading.Tasks;
using StackExchange.Redis;
using Constants = PopcornExport.Helpers.Constants;

namespace PopcornExport.Services.Caching
{
    /// <summary>
    /// The caching service
    /// </summary>
    public class CachingService : ICachingService
    {
        /// <summary>
        /// The Redis connection multiplexer
        /// </summary>
        private readonly IConnectionMultiplexer _connection;

        /// <summary>
        /// Redis database
        /// </summary>
        private readonly IDatabase _redisDatabase;

        /// <summary>
        /// Create an instance of <see cref="CachingService"/>
        /// </summary>
        public CachingService(string redisConnectionString)
        {
            var redisConfig = ConfigurationOptions.Parse(redisConnectionString);
            redisConfig.SyncTimeout = 5000;
            redisConfig.AllowAdmin = true;
            _connection = ConnectionMultiplexer.Connect(redisConfig);
            _connection.IncludeDetailInExceptions = true;
            _redisDatabase = _connection.GetDatabase();
        }

        /// <summary>
        /// Cache
        /// </summary>
        public async Task Flush()
        {
            var server = _connection.GetServer(Constants.RedisHost);
            await server.FlushAllDatabasesAsync().ConfigureAwait(false);
        }
    }
}
