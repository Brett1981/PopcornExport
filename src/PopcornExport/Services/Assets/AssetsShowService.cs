using System;
using System.Threading.Tasks;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;
using PopcornExport.Services.Logging;

namespace PopcornExport.Services.Assets
{
    /// <summary>
    /// Manage assets for shows
    /// </summary>
    public sealed class AssetsShowService : IAssetsService
    {
        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Create an instance of <see cref="AssetsShowService"/>
        /// </summary>
        public AssetsShowService(ILoggingService loggingService, IFileService fileService)
        {
            _loggingService = loggingService;
            _fileService = fileService;
        }

        /// <summary>
        /// Upload a file to Azure Storage
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="fileUrl">File url</param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Upload file path</returns>
        public async Task<string> UploadFile(string fileName, string fileUrl, bool forceReplace = false)
        {
            try
            {
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out _))
                {
                    return
                        await _fileService.UploadFileFromUrlToAzureStorage(fileName, fileUrl, ExportType.Shows, forceReplace);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }
    }
}