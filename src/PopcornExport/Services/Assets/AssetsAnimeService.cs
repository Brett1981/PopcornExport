using System;
using System.Threading.Tasks;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;

namespace PopcornExport.Services.Assets
{
    /// <summary>
    /// Manage assets for anime
    /// </summary>
    public sealed class AssetsAnimeService : IAssetsService
    {
        /// <summary>
        /// The file service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Create an instance of <see cref="AssetsAnimeService"/>
        /// </summary>
        public AssetsAnimeService(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="fileName">File name to upload</param>
        /// <param name="fileUrl">File url to upload</param>
        /// <returns>Remote string</returns>
        public async Task<string> UploadFile(string fileName, string fileUrl)
        {
            try
            {
                Uri result;
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out result))
                {
                    return await _fileService.UploadFileFromUrlToAzureStorage(fileName, fileUrl, ExportType.Anime);
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