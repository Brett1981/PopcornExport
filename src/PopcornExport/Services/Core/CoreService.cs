using PopcornExport.Models.Export;
using PopcornExport.Services.Database;
using PopcornExport.Services.Export;
using PopcornExport.Services.Import;
using PopcornExport.Services.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using PopcornExport.Services.Assets;
using PopcornExport.Services.File;

namespace PopcornExport.Services.Core
{
    /// <summary>
    /// Core service
    /// </summary>
    public sealed class CoreService : ICoreService
    {
        /// <summary>
        /// Export service
        /// </summary>
        private readonly IExportService _exportService;

        /// <summary>
        /// MongoDb service
        /// </summary>
        private readonly IDocumentDbService _documentDbService;

        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Core service
        /// </summary>
        /// <param name="exportService">Export service</param>
        /// <param name="documentDbService">MongoDb service</param>
        /// <param name="loggingService">Logging service</param>
        /// <param name="fileService">The file service</param>
        public CoreService(IExportService exportService, IDocumentDbService documentDbService,
            ILoggingService loggingService, IFileService fileService)
        {
            _exportService = exportService;
            _loggingService = loggingService;
            _documentDbService = documentDbService;
            _fileService = fileService;
        }

        /// <summary>
        /// Process the exportation
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task Export()
        {
            try
            {
                var loggingTraceBegin =
                    $@"Export started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                        CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

                Console.WriteLine(loggingTraceBegin);

                await _documentDbService.Init();
                var exports = new[] {ExportType.Shows, ExportType.Anime, ExportType.Movies};
                foreach (var export in exports)
                {
                    // Load export
                    var documents = await _exportService.LoadExport(export);

                    IImportService importService;
                    // Import the documents according to export type
                    switch (export)
                    {
                        case ExportType.Anime:
                            importService = new ImportAnimeService(_documentDbService,
                                new AssetsAnimeService(_fileService),
                                _loggingService);
                            await importService.Import(documents);
                            break;
                        case ExportType.Shows:
                            importService = new ImportShowService(_documentDbService,
                                new AssetsShowService(_fileService),
                                _loggingService);
                            await importService.Import(documents);
                            break;
                        case ExportType.Movies:
                            importService = new ImportMovieService(_documentDbService,
                                new AssetsMovieService(_fileService),
                                _loggingService);
                            await importService.Import(documents);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                var loggingTraceEnd =
                    $@"Export ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
                _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
            }
        }
    }
}