using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PopcornExport.Extensions;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;

namespace PopcornExport.Services.File
{
    /// <summary>
    /// File service
    /// </summary>
    public sealed class FileService : IFileService
    {
        private readonly CloudStorageAccount _storageAccount;

        /// <summary>
        /// Create an instance of <see cref="FileService"/>
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="storageConnectionString"></param>
        public FileService(string accountName, string storageConnectionString)
        {
            _storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, storageConnectionString),
                false);
        }

        /// <summary>
        /// Upload a file to Azure Storage from a url
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="url">Url of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <returns>Uri of the uploaded file</returns>
        public async Task<string> UploadFileFromUrlToAzureStorage(string fileName, string url, ExportType type)
        {
            try
            {
                var blobClient = _storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(Constants.AssetsFolder);

                await container.CreateIfNotExistsAsync();

                var permissions = await container.GetPermissionsAsync();
                if (permissions.PublicAccess != BlobContainerPublicAccessType.Blob)
                {
                    permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    await container.SetPermissionsAsync(permissions);
                }

                var blob = container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (!await blob.ExistsAsync())
                {
                    using (var client = new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(2)
                    })
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        using (var response = await client.SendAsync(request))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var contentStream = await response.Content.ReadAsStreamAsync())
                            {
                                // Create a directory under the root directory 
                                var file = container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                                await file.UploadFromStreamAsync(contentStream);
                                return file.Uri.AbsoluteUri;
                            }
                        }
                    }
                }
                else
                {
                    return blob.Uri.AbsoluteUri;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
