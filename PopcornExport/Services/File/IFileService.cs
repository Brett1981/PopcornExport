using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PopcornExport.Models.Export;

namespace PopcornExport.Services.File
{
    /// <summary>
    /// File service
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Initialize file service
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        /// Upload a file to Azure Storage from a url
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="url">Url of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Uri of the uploaded file</returns>
        Task<string> UploadFileFromUrlToAzureStorage(string fileName, string url, ExportType type, bool forceReplace = false);

        /// <summary>
        /// Upload a file to Azure Storage from a file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="path">Path of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <param name="eraseSource">Erase source file</param>
        /// <returns>Uri of the uploaded file</returns>
        Task<string> UploadFileFromLocalToAzureStorage(string fileName, string path, ExportType type,
            bool forceReplace = false, bool eraseSource = false);

        /// <summary>
        /// Upload a file to Azure Storage from a stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="stream">Stream of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Uri of the uploaded file</returns>
        Task<string> UploadFileFromStreamToAzureStorage(string fileName, Stream stream, ExportType type,
            bool forceReplace = false);

        /// <summary>
        /// Check if a blob exists
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <returns>True if blob exists, false otherwise</returns>
        Task<bool> CheckIfBlobExists(string fileName, ExportType type);

        /// <summary>
        /// Get blob path
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <returns>Blob path</returns>
        Task<string> GetBlobPath(string fileName, ExportType type);

        /// <summary>
        /// Get a blob stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>Blob stream</returns>
        Task<Stream> DownloadBlobToStreamAsync(string fileName, ExportType type, Stream outputStream);
    }
}
