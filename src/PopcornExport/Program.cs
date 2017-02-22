using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using PopcornExport.Services.Core;
using PopcornExport.Services.Database;
using PopcornExport.Services.Export;
using StructureMap;

namespace PopcornExport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // add the framework services
            var services = new ServiceCollection()
                .AddSingleton<IExportService, ExportService>()
                .AddSingleton<IMongoDbService<BsonDocument>, MongoDbService<BsonDocument>>()
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
            coreService.Process().GetAwaiter().GetResult();
        }
    }
}