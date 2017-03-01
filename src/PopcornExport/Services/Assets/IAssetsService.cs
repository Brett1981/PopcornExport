using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Assets
{
    /// <summary>
    /// Assets service
    /// </summary>
    public interface IAssetsService
    {
        /// <summary>
        /// Upload a file to Azure Storage
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="fileUrl">File url</param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Upload file path</returns>
        Task<string> UploadFile(string fileName, string fileUrl, bool forceReplace = false);
    }
}
