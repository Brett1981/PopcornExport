using System;
using System.Threading.Tasks;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;

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
        /// Create an instance of <see cref="AssetsAnimeService"/>
        /// </summary>
        public AssetsShowService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<string> UploadFile(string fileName, string fileUrl)
        {
            try
            {
                Uri result;
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out result))
                {
                    return
                        await _fileService.UploadFileFromUrlToAzureStorage(fileName, fileUrl, ExportType.Shows);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}