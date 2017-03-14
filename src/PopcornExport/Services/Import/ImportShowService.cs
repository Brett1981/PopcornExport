using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PopcornExport.Models.Show;
using PopcornExport.Services.Assets;
using PopcornExport.Database;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import shows
    /// </summary>
    public sealed class ImportShowService : IImportService
    {
        /// <summary>
        /// The logging service
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Assets service
        /// </summary>
        private readonly IAssetsService _assetsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetsService">Assets service</param>
        /// <param name="loggingService">Logging service</param>
        public ImportShowService(IAssetsService assetsService,
            ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _assetsService = assetsService;
        }

        /// <summary>
        /// Import shows to database
        /// </summary>
        /// <param name="docs">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> docs)
        {
            var documents = docs.ToList();
            var loggingTraceBegin =
                $@"Import {documents.Count} shows started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var updatedShows = 0;
            using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
            {
                foreach (var document in documents)
                {
                    try
                    {
                        var watch = new Stopwatch();
                        watch.Start();

                        // Deserialize a document to a show
                        var showJson =
                            BsonSerializer.Deserialize<ShowBson>(document);

                        await RetrieveAssets(showJson);

                        if (showJson.Year == null) continue;

                        var show = new Show
                        {
                            Rating = new Rating
                            {
                                Watching = showJson.Rating.Watching,
                                Hated = showJson.Rating.Hated,
                                Percentage = showJson.Rating.Percentage,
                                Votes = showJson.Rating.Votes,
                                Loved = showJson.Rating.Loved
                            },
                            Images = new ImageShow
                            {
                                Banner = showJson.Images.Banner,
                                Poster = showJson.Images.Poster,
                                Fanart = showJson.Images.Fanart
                            },
                            ImdbId = showJson.ImdbId,
                            Title = showJson.Title,
                            Year = int.Parse(showJson.Year),
                            Runtime = showJson.Runtime,
                            Genres = showJson.Genres.Select(genre => new Genre
                            {
                                Name = genre.AsString
                            }).ToList(),
                            Slug = showJson.Slug,
                            LastUpdated = showJson.LastUpdated,
                            TvdbId = showJson.TvdbId,
                            NumSeasons = showJson.NumSeasons,
                            Status = showJson.Status,
                            Synopsis = showJson.Synopsis,
                            Country = showJson.Country,
                            Episodes = showJson.Episodes.Select(episode => new EpisodeShow
                            {
                                Title = episode.Title,
                                DateBased = episode.DateBased,
                                TvdbId = episode.TvdbId,
                                Torrents = new TorrentNode
                                {
                                    Torrent0 = new Torrent
                                    {
                                        Url = episode.Torrents.Torrent_0?.Url,
                                        Peers = episode.Torrents.Torrent_0?.Peers,
                                        Seeds = episode.Torrents.Torrent_0?.Seeds,
                                        Provider = episode.Torrents.Torrent_0?.Provider
                                    },
                                    Torrent1080p = new Torrent
                                    {
                                        Url = episode.Torrents.Torrent_1080p?.Url,
                                        Peers = episode.Torrents.Torrent_1080p?.Peers,
                                        Seeds = episode.Torrents.Torrent_1080p?.Seeds,
                                        Provider = episode.Torrents.Torrent_1080p?.Provider
                                    },
                                    Torrent480p = new Torrent
                                    {
                                        Url = episode.Torrents.Torrent_480p?.Url,
                                        Peers = episode.Torrents.Torrent_480p?.Peers,
                                        Seeds = episode.Torrents.Torrent_480p?.Seeds,
                                        Provider = episode.Torrents.Torrent_480p?.Provider
                                    },
                                    Torrent720p = new Torrent
                                    {
                                        Url = episode.Torrents.Torrent_720p?.Url,
                                        Peers = episode.Torrents.Torrent_720p?.Peers,
                                        Seeds = episode.Torrents.Torrent_720p?.Seeds,
                                        Provider = episode.Torrents.Torrent_720p?.Provider
                                    }
                                },
                                EpisodeNumber = episode.EpisodeNumber,
                                Season = episode.Season,
                                Overview = episode.Overview,
                                FirstAired = episode.FirstAired
                            }).ToList(),
                            AirDay = showJson.AirDay,
                            AirTime = showJson.AirTime,
                            Network = showJson.Network
                        };

                        var existingEntity = await context.ShowSet.Include(a => a.Rating)
                            .Include(a => a.Episodes)
                            .ThenInclude(episode => episode.Torrents)
                            .ThenInclude(torrent => torrent.Torrent0)
                            .Include(a => a.Episodes)
                            .ThenInclude(episode => episode.Torrents)
                            .ThenInclude(torrent => torrent.Torrent1080p)
                            .Include(a => a.Episodes)
                            .ThenInclude(episode => episode.Torrents)
                            .ThenInclude(torrent => torrent.Torrent480p)
                            .Include(a => a.Episodes)
                            .ThenInclude(episode => episode.Torrents)
                            .ThenInclude(torrent => torrent.Torrent720p)
                            .Include(a => a.Genres)
                            .Include(a => a.Images).FirstOrDefaultAsync(a => a.ImdbId == show.ImdbId);

                        if (existingEntity == null)
                        {
                            context.ShowSet.Add(show);
                        }
                        else
                        {
                            existingEntity.Rating.Hated = show.Rating.Hated;
                            existingEntity.Rating.Loved = show.Rating.Loved;
                            existingEntity.Rating.Percentage = show.Rating.Percentage;
                            existingEntity.Rating.Votes = show.Rating.Votes;
                            existingEntity.Rating.Watching = show.Rating.Watching;
                            existingEntity.AirDay = show.AirDay;
                            existingEntity.AirTime = show.AirTime;
                            existingEntity.Status = show.Status;
                            existingEntity.NumSeasons = show.NumSeasons;
                            foreach (var episode in existingEntity.Episodes)
                            {
                                var updatedEpisode = show.Episodes.FirstOrDefault(a => a.TvdbId == episode.TvdbId);
                                if (updatedEpisode == null) continue;

                                if (episode.Torrents != null && episode.Torrents.Torrent0 != null &&
                                    updatedEpisode.Torrents.Torrent0 != null)
                                {
                                    episode.Torrents.Torrent0.Peers = updatedEpisode.Torrents.Torrent0.Peers;
                                    episode.Torrents.Torrent0.Seeds = updatedEpisode.Torrents.Torrent0.Seeds;
                                }

                                if (episode.Torrents != null && episode.Torrents.Torrent1080p != null &&
                                    updatedEpisode.Torrents.Torrent1080p != null)
                                {
                                    episode.Torrents.Torrent1080p.Peers = updatedEpisode.Torrents.Torrent1080p.Peers;
                                    episode.Torrents.Torrent1080p.Seeds = updatedEpisode.Torrents.Torrent1080p.Seeds;
                                }

                                if (episode.Torrents != null && episode.Torrents.Torrent720p != null &&
                                    updatedEpisode.Torrents.Torrent720p != null)
                                {
                                    episode.Torrents.Torrent720p.Peers = updatedEpisode.Torrents.Torrent720p.Peers;
                                    episode.Torrents.Torrent720p.Seeds = updatedEpisode.Torrents.Torrent720p.Seeds;
                                }

                                if (episode.Torrents != null && episode.Torrents.Torrent480p != null &&
                                    updatedEpisode.Torrents.Torrent480p != null)
                                {
                                    episode.Torrents.Torrent480p.Peers = updatedEpisode.Torrents.Torrent480p.Peers;
                                    episode.Torrents.Torrent480p.Seeds = updatedEpisode.Torrents.Torrent480p.Seeds;
                                }
                            }

                            var newEpisodes =
                                show.Episodes.Where(a => existingEntity.Episodes.All(b => b.TvdbId != a.TvdbId));
                            foreach (var newEpisode in newEpisodes.ToList())
                            {
                                show.Episodes.Add(newEpisode);
                            }
                        }

                        await context.SaveChangesAsync();

                        watch.Stop();
                        updatedShows++;
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(
                            $"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} UPDATED SHOW {show.Title} in {watch.ElapsedMilliseconds} ms. {updatedShows}/{documents.Count}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                    }
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing shows.");

            var loggingTraceEnd =
                $@"Import shows ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }

        /// <summary>
        /// Retrieve images for the provided show
        /// </summary>
        /// <param name="show">Show to process</param>
        /// <returns><see cref="Task"/></returns>
        private async Task RetrieveAssets(ShowBson show)
        {
            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Banner))
                        show.Images.Banner =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/banner/{show.Images.Banner.Split('/').Last()}",
                                show.Images.Banner);
                }),
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Fanart))
                        show.Images.Fanart =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/fanart/{show.Images.Fanart.Split('/').Last()}",
                                show.Images.Fanart);
                }),
                Task.Run(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(show.Images.Poster))
                        show.Images.Poster =
                            await _assetsService.UploadFile(
                                $@"images/{show.ImdbId}/poster/{show.Images.Poster.Split('/').Last()}",
                                show.Images.Poster);
                })
            };

            await Task.WhenAll(tasks);
        }
    }
}