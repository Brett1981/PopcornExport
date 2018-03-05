using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.EntityFrameworkCore;
using Polly;
using Popcorn.OSDB;
using PopcornExport.Database;
using PopcornExport.Extensions;
using PopcornExport.Helpers;
using PopcornExport.Models.Export;
using PopcornExport.Services.File;
using PopcornExport.Services.Language;
using PopcornExport.Services.Logging;

namespace PopcornExport.Services.Subtitle
{
    public class SubtitleService : ISubtitleService
    {
        private DateTimeOffset _lastOpenSubtitlesLimitReached = DateTimeOffset.MinValue;

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

        public async Task<string> DownloadSubtitleToPath(string subtitleId, string imdbId, string lang,
            string outputPath,
            string remoteSubtitlePath, ExportType type)
        {
            try
            {
                bool isOpusArchiveDownloaded;
                using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
                {
                    isOpusArchiveDownloaded =
                        await context.LanguageSet.AnyAsync(a => a.Iso639 == lang && a.OpusArchiveDownloaded);
                }

                if (!isOpusArchiveDownloaded)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                            "text/html,application/xhtml+xml,application/xml");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                            "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");

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
                                    try
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
                                                    var id = entry.Name.GetMatches("/", ".").Last().Split(new[] {'/'})
                                                        .Last();
                                                    var name =
                                                        $"{type.ToFriendlyString().ToLowerInvariant()}/{entry.Name.GetMatches("/", $"/{id}").Last().Split(new[] {'/'}).Last()}/{lang}/{id}.xml";
                                                    await _fileService.UploadFileFromStreamToAzureStorage(name,
                                                        blobStream, ExportType.Subtitles);
                                                    entry = tarStream.GetNextEntry();
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _loggingService.Telemetry.TrackException(ex);
                                    }
                                }
                            }
                        }
                    }

                    using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
                    {
                        var languageSet = await context.LanguageSet.FirstAsync(a => a.Iso639 == lang);
                        languageSet.OpusArchiveDownloaded = true;
                        await context.SaveChangesAsync();
                    }
                }

                if (!await _fileService.CheckIfBlobExists(outputPath, ExportType.Subtitles) &&
                    await _fileService.CheckIfBlobExists(outputPath.Replace("srt", "xml"), ExportType.Subtitles))
                {
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
                                var xElement = node as XElement;
                                if (string.IsNullOrEmpty(beginTime) && string.IsNullOrEmpty(endTime) &&
                                    xElement?.Name == "time")
                                    beginTime = xElement.LastAttribute?.Value;

                                if (string.IsNullOrEmpty(endTime) && !string.IsNullOrEmpty(beginTime) &&
                                    xElement?.Name == "time" &&
                                    xElement.LastAttribute?.Value != beginTime)
                                    endTime = xElement.LastAttribute?.Value;

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
                                else if (xElement != null && xElement.Value.Any(char.IsPunctuation) &&
                                         xElement.Value != "," ||
                                         nextElement != null &&
                                         (nextElement.Value == "?" || nextElement.Value == "!" ||
                                          nextElement.Value == "." || nextElement.Value == "'"))
                                {
                                    text += $"{xElement.Value}";
                                }
                                else if (xElement?.Name == "w")
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

                if (!await _fileService.CheckIfBlobExists(outputPath, ExportType.Subtitles) &&
                    _lastOpenSubtitlesLimitReached.AddDays(1) < DateTimeOffset.Now)
                {
                    var cookieContainer = new CookieContainer();
                    using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
                    using (var client = new HttpClient(handler)
                    {
                        Timeout = TimeSpan.FromSeconds(10)
                    })
                    using (var request = new HttpRequestMessage(HttpMethod.Get, remoteSubtitlePath))
                    {
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("__cfduid", "d89c398a4a225a96384a45172e9ad48d41518622455", "/",
                                ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("__qca", "P0-626973130-1518622458255", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("_gat", "1", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("_ga", "GA1.2.1978819766.1518622457", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("_gid", "GA1.2.206745261.1518622457", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("logged", "1", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("PHPSESSID", "Mq%2CxP8s4hD2WE%2CT0oAte77-kTh8", "/",
                                ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("remember_sid", "4SHIF6wndJ32qlAgQCBHe-HrHX2", "/",
                                ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("user", "bbougot", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("weblang", "en", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("OAID", "501bd61c02c291c50eb8647612c16839", "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("searchform",
                                "formname%3Dsearchform%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C1%7C%7C%7C1%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C%7C",
                                "/", ".opensubtitles.org"));
                        cookieContainer.Add(new Uri(remoteSubtitlePath),
                            new Cookie("show_iduserdnotrated", "1", "/", ".opensubtitles.org"));
                        using (var response = await client.SendAsync(request).ConfigureAwait(false))
                        {
                            if (response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired ||
                                response.StatusCode == HttpStatusCode.NotFound)
                            {
                                _lastOpenSubtitlesLimitReached = DateTimeOffset.Now;
                                return string.Empty;
                            }

                            using (var contentStream =
                                await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (var decompressedFileStream = new MemoryStream())
                                {
                                    using (var decompressionStream =
                                        new GZipStream(contentStream, CompressionMode.Decompress))
                                    {
                                        decompressionStream.CopyTo(decompressedFileStream);
                                        decompressedFileStream.Seek(0, SeekOrigin.Begin);
                                        return await _fileService.UploadFileFromStreamToAzureStorage(outputPath,
                                            decompressedFileStream,
                                            ExportType.Subtitles);
                                    }
                                }
                            }
                        }
                    }
                }

                return await _fileService.GetBlobPath(outputPath, ExportType.Subtitles);
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }
    }
}