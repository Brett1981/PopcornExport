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
                Uri result;
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out result))
                {
                    return await _fileService.UploadFileFromUrlToAzureStorage(fileName, fileUrl, ExportType.Anime, forceReplace);
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