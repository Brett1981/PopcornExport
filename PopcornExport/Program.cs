using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PopcornExport.Models.Export;
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
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var container = new Container();
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
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                });
                config.Populate(services);
            });
            
            var coreService = container.GetInstance<ICoreService>();
            await coreService.Export().ConfigureAwait(false);
        }
    }
}