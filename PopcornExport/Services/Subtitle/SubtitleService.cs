using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Polly;
using Popcorn.OSDB;
using PopcornExport.Extensions;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;
using PopcornExport.Services.Logging;

namespace PopcornExport.Services.Subtitle
{
    public class SubtitleService : ISubtitleService
    {
        private readonly ILoggingService _loggingService;

        private readonly IFileService _fileService;

        public SubtitleService(ILoggingService loggingService, IFileService fileService)
        {
            _loggingService = loggingService;
            _fileService = fileService;
        }

        /// <summary>
        /// Get subtitles languages
        /// </summary>
        /// <returns>Languages</returns>
        public async Task<IEnumerable<Popcorn.OSDB.Language>> GetSubLanguages()
        {
            var retryGetSubLanguagesPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            try
            {
                return await retryGetSubLanguagesPolicy.ExecuteAsync(async () =>
                {
                    using (var osdb = await new Osdb().Login(Constants.OsdbUa))
                    {
                        return await osdb.GetSubLanguages();
                    }
                });
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return new List<Popcorn.OSDB.Language>();
            }
        }

        /// <summary>
        /// Search subtitles by imdb code and languages
        /// </summary>
        /// <param name="languages">Languages</param>
        /// <param name="imdbId">Imdb code</param>
        /// <param name="season">Season number</param>
        /// <param name="episode">Episode number</param>
        /// <returns>Subtitles</returns>
        public async Task<IList<Popcorn.OSDB.Subtitle>> SearchSubtitlesFromImdb(string languages, string imdbId,
            int? season,
            int? episode)
        {
            var retrySearchSubtitlesFromImdbPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            try
            {
                return await retrySearchSubtitlesFromImdbPolicy.ExecuteAsync(async () =>
                {
                    using (var osdb = await new Osdb().Login(Constants.OsdbUa))
                    {
                        return await osdb.SearchSubtitlesFromImdb(languages, imdbId, season, episode);
                    }
                });
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return new List<Popcorn.OSDB.Subtitle>();
            }
        }

        public async Task<string> DownloadSubtitleToPath(string subtitleId, string lang, string outputPath)
        {
            try
            {
                if (!await _fileService.CheckIfBlobExists(outputPath, ExportType.Subtitles))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                            "text/html,application/xhtml+xml,application/xml");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                            "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

                        var url = $"http://opus.nlpl.eu/download.php?f=OpenSubtitles2018/{lang}.tar.gz";
                        using (var response =
                            await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream sourceStream =
                                new GZipStream(streamToReadFrom, CompressionMode.Decompress))
                            {
                                using (var tarStream =
                                    new TarInputStream(sourceStream, TarBuffer.DefaultBlockFactor))
                                {
                                    var entry = tarStream.GetNextEntry();
                                    while (entry != null)
                                    {
                                        using (var entryStream = new MemoryStream())
                                        {
                                            tarStream.CopyEntryContents(entryStream);
                                            var dataBuffer = new byte[4096];
                                            entryStream.Seek(0, SeekOrigin.Begin);
                                            using (var blobStream = new MemoryStream())
                                            using (var gzipStream = new GZipInputStream(entryStream))
                                            {
                                                StreamUtils.Copy(gzipStream, blobStream, dataBuffer);
                                                blobStream.Seek(0, SeekOrigin.Begin);
                                                await _fileService.UploadFileFromStreamToAzureStorage(
                                                    outputPath.Replace("srt", "xml"),
                                                    blobStream, ExportType.Subtitles);
                                                entry = tarStream.GetNextEntry();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    using (var blobStream = new MemoryStream())
                    {
                        await _fileService.DownloadBlobToStreamAsync(outputPath.Replace("srt", "xml"),
                            ExportType.Subtitles,
                            blobStream);
                        blobStream.Seek(0, SeekOrigin.Begin);
                        var xmldoc = new XmlDocument();
                        xmldoc.Load(blobStream);
                        var xdoc = xmldoc.ToXDocument();
                        var sb = new StringBuilder();
                        var count = 1;
                        var beginTime = string.Empty;
                        var endTime = string.Empty;
                        var text = string.Empty;
                        foreach (var descendant in xdoc.Descendants("s"))
                        {
                            var nodes = descendant.Nodes().ToList();
                            foreach (var node in nodes)
                            {
                                if (string.IsNullOrEmpty(beginTime) && string.IsNullOrEmpty(endTime) &&
                                    (node as XElement).Name == "time")
                                    beginTime = (node as XElement)?.LastAttribute?.Value;

                                if (string.IsNullOrEmpty(endTime) && !string.IsNullOrEmpty(beginTime) &&
                                    (node as XElement).Name == "time" &&
                                    (node as XElement)?.LastAttribute?.Value != beginTime)
                                    endTime = (node as XElement)?.LastAttribute?.Value;

                                var xElement = node as XElement;
                                if (xElement?.Name == "time")
                                {
                                    if (!string.IsNullOrEmpty(beginTime) && !string.IsNullOrEmpty(endTime))
                                    {
                                        sb.AppendLine(count.ToString());
                                        sb.AppendLine($"{beginTime} --> {endTime}");
                                        sb.AppendLine(text);
                                        sb.AppendLine();
                                        count++;
                                        beginTime = string.Empty;
                                        endTime = string.Empty;
                                        text = string.Empty;
                                    }

                                    continue;
                                }

                                var nextElement = xElement?.NextNode as XElement;
                                if (xElement?.Name == "w" && xElement == descendant.LastNode ||
                                    nextElement?.Name == "time")
                                {
                                    text += $"{xElement.Value}";
                                    if (xElement.Name == "w" && xElement == descendant.LastNode)
                                    {
                                        text += Environment.NewLine;
                                    }
                                }
                                else if (xElement.Value.Any(char.IsPunctuation) ||
                                         nextElement != null && nextElement.Value.Any(char.IsPunctuation))
                                {
                                    text += $"{xElement.Value}";
                                }
                                else if (xElement.Name == "w")
                                {
                                    text += $"{xElement.Value} ";
                                }
                            }
                        }

                        using (var ms = new MemoryStream())
                        {
                            using (var sw = new StreamWriter(ms, Encoding.UTF8))
                            {
                                sw.Write(sb.ToString());
                                ms.Seek(0, SeekOrigin.Begin);
                                return await _fileService.UploadFileFromStreamToAzureStorage(outputPath,
                                    ms,
                                    ExportType.Subtitles);
                            }
                        }
                    }
                }
                else
                {
                    return await _fileService.GetBlobPath(outputPath, ExportType.Subtitles);
                }
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }
    }
}