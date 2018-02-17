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
using PopcornExport.Services.Logging;

namespace PopcornExport.Services.Subtitle
{
    public class SubtitleService : ISubtitleService
    {
        private readonly ILoggingService _loggingService;

        public SubtitleService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
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

        public async Task<string> DownloadSubtitleToPath(string subtitleId, string lang)
        {
            try
            {
                var directory =
                    new DirectoryInfo(
                        $@"{Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName}\Subtitles\{lang}");
                if (!directory.Exists)
                {
                    directory.Create();
                }

                if (!Directory.Exists($@"{directory.FullName}\OpenSubtitles2018"))
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
                            using (Stream sourceStream = new GZipStream(streamToReadFrom, CompressionMode.Decompress))
                            {
                                using (var tarArchive =
                                    TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                                {
                                    tarArchive.ExtractContents(directory.FullName);
                                }
                            }
                        }

                        Parallel.ForEach(directory.GetFiles("*.xml.gz", SearchOption.AllDirectories),
                            file => { ExtractGZipSample(file.FullName, Directory.GetParent(file.FullName).FullName); });
                    }
                }

                var files = directory.GetFiles("*.xml", SearchOption.AllDirectories);
                var match = files.First(file => file.FullName.Contains(subtitleId));
                var xmldoc = new XmlDocument();
                xmldoc.Load(match.FullName);
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
                            (node as XElement).Name == "time" && (node as XElement)?.LastAttribute?.Value != beginTime)
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
                        var nextNextElement = nextElement?.NextNode as XElement;
                        if (xElement?.Name == "w" && xElement == descendant.LastNode || nextElement?.Name == "time")
                        {
                            text += $"{xElement.Value}";
                            if (xElement?.Name == "w" && xElement == descendant.LastNode)
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

                var outputPath = $@"{Directory.GetParent(match.FullName).FullName}\{subtitleId}.srt";
                System.IO.File.WriteAllText(outputPath,
                    sb.ToString(), Encoding.UTF8);
                return outputPath;
            }
            catch (Exception ex)
            {
                _loggingService.Telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        private static void ExtractGZipSample(string gzipFileName, string targetDir)
        {
            // Use a 4K buffer. Any larger is a waste.    
            var dataBuffer = new byte[4096];

            using (Stream fs = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read))
            {
                using (var gzipStream = new GZipInputStream(fs))
                {
                    // Change this to your needs
                    var fnOut = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(gzipFileName));

                    using (var fsOut = System.IO.File.Create(fnOut))
                    {
                        StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                    }
                }
            }
        }
    }
}