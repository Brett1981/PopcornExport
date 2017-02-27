using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using PopcornExport.Services.Core;
using PopcornExport.Services.Database;
using PopcornExport.Services.File;
using StructureMap;

namespace PopcornExport
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            // add the framework services
            var services = new ServiceCollection()
                .AddTransient<IMongoDbService<BsonDocument>>(
                    e => new MongoDbService<BsonDocument>(configuration["MongoDb:ConnectionString"]))
                .AddTransient<IFileService>(
                    e =>
                        new FileService(configuration["AzureStorage:AccountName"], configuration["AzureStorage:Key"]))
                .AddLogging();

            // add StructureMap
            var container = new Container();
            container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                });
                // Populate the container using the service collection
                config.Populate(services);
            });

            var coreService = container.GetInstance<ICoreService>();

            // Start export
            coreService.Export().GetAwaiter().GetResult();
        }
    }
}