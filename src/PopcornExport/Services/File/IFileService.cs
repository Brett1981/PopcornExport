using System;
using System.Collections.Generic;
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
        /// <returns>Uri of the uploaded file</returns>
        Task<string> UploadFileFromUrlToAzureStorage(string fileName, string url, ExportType type);
    }
}
