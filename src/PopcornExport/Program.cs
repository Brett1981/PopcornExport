using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PopcornExport.Database;
using PopcornExport.Services.Assets;
using PopcornExport.Services.Caching;
using PopcornExport.Services.Core;
using PopcornExport.Services.File;
using PopcornExport.Services.Logging;
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

            // add StructureMap
            var container = new Container();
            // add the framework services
            var services = new ServiceCollection()
                .AddTransient<IFileService>(
                    e =>
                    {
                        var fileService = new FileService(container.GetInstance<ILoggingService>(), configuration["AzureStorage:AccountName"],
                            configuration["AzureStorage:Key"]);
                        fileService.Initialize().GetAwaiter().GetResult();
                        return fileService;
                    }
                )
                .AddSingleton<ICachingService>(e => new CachingService(configuration["Redis:ConnectionString"]))
                .AddLogging();


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