using PopcornExport.Models.Export;
using PopcornExport.Services.Export;
using PopcornExport.Services.Import;
using PopcornExport.Services.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using PopcornExport.Extensions;
using PopcornExport.Services.Assets;
using PopcornExport.Services.Caching;
using PopcornExport.Services.File;
using ShellProgressBar;

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
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// The caching service
        /// </summary>
        private readonly ICachingService _cachingService;

        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Core service
        /// </summary>
        /// <param name="exportService">Export service</param>
        /// <param name="loggingService">Logging service</param>
        /// <param name="fileService">The file service</param>
        /// <param name="cachingService">The caching service</param>
        public CoreService(IExportService exportService, ILoggingService loggingService, IFileService fileService,
            ICachingService cachingService)
        {
            _exportService = exportService;
            _loggingService = loggingService;
            _fileService = fileService;
            _cachingService = cachingService;
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
                    $@"Export started at {
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff",
                                CultureInfo.InvariantCulture)
                        }";
                _loggingService.Telemetry.TrackTrace(loggingTraceBegin);
                Console.WriteLine(loggingTraceBegin);
                var exports = new[] {ExportType.Movies, ExportType.Shows};
                var overProgressOptions = new ProgressBarOptions
                {
                    BackgroundColor = ConsoleColor.DarkGray
                };
                using (var pbar = Schim.ProgressBar.Create(exports.Length * 2, "overall progress", overProgressOptions))
                {
                    foreach (var export in exports)
                    {
                        var stepBarOptions = new ProgressBarOptions
                        {
                            ForegroundColor = ConsoleColor.Cyan,
                            ForegroundColorDone = ConsoleColor.DarkGreen,
                            ProgressCharacter = '─',
                            BackgroundColor = ConsoleColor.DarkGray,
                        };
                        using (var childProgress = pbar.Spawn(2,
                            $"step {export.ToFriendlyString().ToLowerInvariant()} progress", stepBarOptions))
                        {
                            // Load export
                            var imports = await _exportService.LoadExport(export, childProgress)
                                ;
                            pbar.Tick();
                            IImportService importService;
                            // Import the documents according to export type
                            switch (export)
                            {
                                case ExportType.Shows:
                                    importService = new ImportShowService(
                                        new AssetsShowService(_loggingService, _fileService),
                                        _loggingService, _fileService);
                                    break;
                                case ExportType.Movies:
                                    importService = new ImportMovieService(
                                        new AssetsMovieService(_loggingService, _fileService),
                                        _loggingService, _fileService);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                            await importService.Import(imports, childProgress);
                            pbar.Tick();
                        }
                    }
                }

                await _cachingService.Flush();
                var loggingTraceEnd =
                    $@"Export ended at {
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)
                        }";
                _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                _loggingService.Telemetry.TrackException(ex);
            }
        }
    }
}