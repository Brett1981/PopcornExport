﻿using System;
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
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Processing.Transforms.Resamplers;
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
                if (forceReplace || !await blob.ExistsAsync())
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
                        using (var response = await client.SendAsync(request))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var contentStream =
                                await response.Content.ReadAsStreamAsync())
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
                                            await file.UploadFromStreamAsync(stream);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                        await file.UploadFromStreamAsync(contentStream);
                                    }
                                }
                                else
                                {
                                    await file.UploadFromStreamAsync(contentStream);
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
    }
}