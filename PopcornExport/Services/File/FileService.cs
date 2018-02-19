using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PopcornExport.Extensions;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace PopcornExport.Services.File
{
    /// <summary>
    /// File service
    /// </summary>
    public sealed class FileService : IFileService
    {
        /// <summary>
        /// True if service is initialized
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// <see cref="CloudStorageAccount"/>
        /// </summary>
        private readonly CloudStorageAccount _storageAccount;

        /// <summary>
        /// <see cref="ILoggingService"/>
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Container
        /// </summary>
        private CloudBlobContainer _container;

        /// <summary>
        /// Create an instance of <see cref="FileService"/>
        /// </summary>
        /// <param name="loggingService"></param>
        /// <param name="accountName"></param>
        /// <param name="storageConnectionString"></param>
        public FileService(ILoggingService loggingService, string accountName, string storageConnectionString)
        {
            _loggingService = loggingService;
            _storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, storageConnectionString),
                true);
        }

        /// <summary>
        /// Initialize the file service
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(Constants.AssetsFolder);

            await _container.CreateIfNotExistsAsync();

            var permissions = await _container.GetPermissionsAsync();
            if (permissions.PublicAccess != BlobContainerPublicAccessType.Blob)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await _container.SetPermissionsAsync(permissions);
            }

            _initialized = true;
        }

        /// <summary>
        /// Upload a file to Azure Storage from a url
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="url">Url of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Uri of the uploaded file</returns>
        public async Task<string> UploadFileFromUrlToAzureStorage(string fileName, string url, ExportType type,
            bool forceReplace = false)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");
                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (forceReplace || !await blob.ExistsAsync().ConfigureAwait(false))
                {
                    var file = _container.GetBlockBlobReference(
                        $@"{type.ToFriendlyString()}/{fileName}");
                    var cookieContainer = new CookieContainer();
                    using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
                    using (var client = new HttpClient(handler)
                    {
                        Timeout = TimeSpan.FromSeconds(10)
                    })
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        if (url.Contains("yts"))
                        {
                            // Sometimes YTS requires an authentication to access movies. Here is an already authenticated cookie valid until December 2017
                            cookieContainer.Add(new Uri(url),
                                new Cookie("PHPSESSID", "9kp14954qk7pfa6kconsocg2a3", "/", "yts.am"));
                            cookieContainer.Add(new Uri(url),
                                new Cookie("__atuvs", "5a1b42230c1ac9da002", "/", "yts.am"));
                            cookieContainer.Add(new Uri(url),
                                new Cookie("__cfduid", "da06ef1392f710522307088b060b5826a1511735838", "/",
                                    ".yts.am"));
                            cookieContainer.Add(new Uri(url),
                                new Cookie("uhh", "5703d275ec7d989652c497b9f921dfcf", "/", ".yts.am"));
                            cookieContainer.Add(new Uri(url), new Cookie("uid", "3788520", "/", ".yts.am"));
                        }
                        using (var response = await client.SendAsync(request).ConfigureAwait(false))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var contentStream =
                                await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {

                                if (blob.Name.Contains("background") ||
                                    blob.Name.Contains("banner") ||
                                    blob.Name.Contains("poster"))
                                {
                                    try
                                    {
                                        using (var stream = new MemoryStream())
                                        using (var image = Image.Load(contentStream))
                                        {
                                            if (blob.Name.Contains("background") || blob.Name.Contains("banner"))
                                                image.Mutate(x => x
                                                    .Resize(new ResizeOptions
                                                    {
                                                        Mode = ResizeMode.Stretch,
                                                        Size = new Size(1280, 720),
                                                        Sampler = new BicubicResampler()
                                                    }));

                                            if (blob.Name.Contains("poster"))
                                                image.Mutate(x => x
                                                    .Resize(new ResizeOptions
                                                    {
                                                        Mode = ResizeMode.Stretch,
                                                        Size = new Size(400, 600),
                                                        Sampler = new BicubicResampler()
                                                    }));

                                            image.SaveAsJpeg(stream, new JpegEncoder
                                            {
                                                Quality = 90
                                            });
                                            stream.Seek(0, SeekOrigin.Begin);
                                            await file.UploadFromStreamAsync(stream).ConfigureAwait(false);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                        await file.UploadFromStreamAsync(contentStream).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await file.UploadFromStreamAsync(contentStream).ConfigureAwait(false);
                                }

                                return file.Uri.AbsoluteUri;
                            }
                        }
                    }
                }

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Upload a file to Azure Storage from a file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="path">Path of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <param name="eraseSource">Erase source file</param>
        /// <returns>Uri of the uploaded file</returns>
        public async Task<string> UploadFileFromLocalToAzureStorage(string fileName, string path, ExportType type,
            bool forceReplace = false, bool eraseSource = false)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");

                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (forceReplace || !await blob.ExistsAsync().ConfigureAwait(false))
                {
                    var file = _container.GetBlockBlobReference(
                        $@"{type.ToFriendlyString()}/{fileName}");
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        await file.UploadFromStreamAsync(stream).ConfigureAwait(false);
                        if (eraseSource)
                        {
                            try
                            {
                                System.IO.File.Delete(path);
                            }
                            catch (Exception ex)
                            {
                                _loggingService.Telemetry.TrackException(ex);
                            }
                        }
                    }
                }

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Upload a file to Azure Storage from a stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="stream">Stream of the file</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="forceReplace">Force replacing an existing file</param>
        /// <returns>Uri of the uploaded file</returns>
        public async Task<string> UploadFileFromStreamToAzureStorage(string fileName, Stream stream, ExportType type,
            bool forceReplace = false)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");

                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (forceReplace || !await blob.ExistsAsync().ConfigureAwait(false))
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                return blob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if a blob exists
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <returns>True if blob exists, false otherwise</returns>
        public async Task<bool> CheckIfBlobExists(string fileName, ExportType type)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");

                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (await blob.ExistsAsync().ConfigureAwait(false))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        /// Get blob path
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <returns>Blob path</returns>
        public async Task<string> GetBlobPath(string fileName, ExportType type)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");

                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (await blob.ExistsAsync().ConfigureAwait(false))
                {
                    return blob.Uri.OriginalString;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get a blob stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="type"><see cref="ExportType"/></param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>Blob stream</returns>
        public async Task<Stream> DownloadBlobToStreamAsync(string fileName, ExportType type, Stream outputStream)
        {
            try
            {
                if (!_initialized) throw new Exception("Service is not initialized");

                var blob = _container.GetBlockBlobReference($@"{type.ToFriendlyString()}/{fileName}");
                if (await blob.ExistsAsync().ConfigureAwait(false))
                {
                    await blob.DownloadToStreamAsync(outputStream);
                }

                return null;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Check if a directory exists
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <returns></returns>
        public bool CheckIfDirectoryExists(string path)
        {
            return _container.ListBlobs(path).Any();
        }
    }
}