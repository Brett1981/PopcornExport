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
using TMDbLib.Client;
using PopcornExport.Helpers;
using TMDbLib.Objects.TvShows;
using TMDbLib.Objects.General;

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

            TmdbClient = new TMDbClient(Constants.TmDbClientId)
            {
                MaxRetryCount = 10
            };

            try
            {
                TmdbClient.GetConfig();
            }
            catch (Exception)
            {
                //TODO
            }
        }

        /// <summary>
        /// TMDb client
        /// </summary>
        private TMDbClient TmdbClient { get; set; }

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
                            Genres = showJson.Genres.Select(genre => new Database.Genre
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
                            .Include(a => a.Images)
                            .Include(a => a.Similars).FirstOrDefaultAsync(a => a.ImdbId == show.ImdbId);

                        if (existingEntity == null)
                        {
                            await UpdateImagesAndSimilarShow(show);
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
        /// Update images and similar shows for a show
        /// </summary>
        /// <param name="show">Show to update</param>
        /// <returns></returns>
        private async Task UpdateImagesAndSimilarShow(Show show)
        {
            int tvId = 0;
            if (int.TryParse(show.TvdbId, out tvId))
            {
                var search = await TmdbClient.SearchTvShowAsync(show.Title);
                if (search.TotalResults != 0)
                {
                    var result = search.Results.FirstOrDefault();
                    var tmdbShow = await TmdbClient.GetTvShowAsync(result.Id, TvShowMethods.Images | TvShowMethods.Similar);
                    var tasks = new List<Task>
                    {
                        Task.Run(async () =>
                        {
                            if (tmdbShow.Images?.Backdrops != null && tmdbShow.Images.Backdrops.Any())
                            {
                                var backdrop = GetImagePathFromTmdb(TmdbClient,
                                    tmdbShow.Images.Backdrops.Aggregate(
                                        (image1, image2) =>
                                            image1 != null && image2 != null && image1.Width < image2.Width
                                                ? image2
                                                : image1));
                                show.Images.Banner =
                                    await _assetsService.UploadFile(
                                        $@"images/{show.ImdbId}/banner/{backdrop.Split('/').Last()}",
                                        backdrop);
                            }
                        }),
                        Task.Run(async () =>
                        {
                            if (tmdbShow.Images?.Posters != null && tmdbShow.Images.Posters.Any())
                            {
                                var poster = GetImagePathFromTmdb(TmdbClient,
                                    tmdbShow.Images.Posters.Aggregate(
                                        (image1, image2) =>
                                            image1 != null && image2 != null && image1.Width < image2.Width
                                                ? image2
                                                : image1));
                                show.Images.Poster =
                                    await _assetsService.UploadFile(
                                        $@"images/{show.ImdbId}/poster/{poster.Split('/').Last()}",
                                        poster);
                            }
                        }),
                        Task.Run(async () =>
                        {
                            if (tmdbShow.Images?.Backdrops != null && tmdbShow.Images.Backdrops.Any())
                            {
                                var fanart = GetImagePathFromTmdb(TmdbClient,
                                    tmdbShow.Images.Backdrops.Aggregate(
                                        (image1, image2) =>
                                            image1 != null && image2 != null && image1.VoteAverage < image2.VoteAverage
                                                ? image2
                                                : image1));
                                show.Images.Fanart =
                                    await _assetsService.UploadFile(
                                        $@"images/{show.ImdbId}/fanart/{fanart.Split('/').Last()}",
                                        fanart);
                            }
                        })
                    };

                    await Task.WhenAll(tasks);

                    if (tmdbShow.Similar.Results.Any())
                    {
                        show.Similars = new List<Similar>();
                        await tmdbShow.Similar.Results.Select(a => a.Id).ParallelForEachAsync(async id =>
                        {
                            try
                            {
                                var externalIds = await TmdbClient.GetTvShowExternalIdsAsync(id);
                                if (externalIds != null)
                                {
                                    show.Similars.Add(new Similar
                                    {
                                        TmdbId = externalIds.ImdbId
                                    });
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve an image from Tmdb
        /// </summary>
        /// <param name="client"><see cref="TMDbClient"/></param>
        /// <param name="image">Image to retrieve</param>
        /// <returns></returns>
        private string GetImagePathFromTmdb(TMDbClient client, ImageData image)
        {
            return client.GetImageUrl("original", image.FilePath).AbsoluteUri;
        }
    }
}