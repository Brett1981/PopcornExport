using PopcornExport.Helpers;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using PopcornExport.Models.Anime;
using PopcornExport.Services.Assets;
using Newtonsoft.Json;
using PopcornExport.Database;
using PopcornExport.Models.Image;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using Microsoft.EntityFrameworkCore;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import animes
    /// </summary>
    public sealed class ImportAnimeService : IImportService
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
        public ImportAnimeService(IAssetsService assetsService,
            ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _assetsService = assetsService;
        }

        /// <summary>
        /// Import movies to database
        /// </summary>
        /// <param name="docs">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        public async Task Import(IEnumerable<BsonDocument> docs)
        {
            var documents = docs.ToList();
            var loggingTraceBegin =
                $@"Import {documents.Count} movies started at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceBegin);

            var updatedAnimes = 0;
            using (var context = new PopcornContextFactory().Create(new DbContextFactoryOptions()))
            {
                foreach (var document in documents)
                {
                    try
                    {
                        var watch = new Stopwatch();
                        watch.Start();
                        // Deserialize a document to an anime
                        var animeJson =
                            BsonSerializer.Deserialize<AnimeBson>(document);

                        await RetrieveAssets(document, animeJson);

                        var anime = new Anime
                        {
                            Images = new CollectionImageAnime
                            {
                                Poster = new ImageAnime
                                {
                                    Medium = animeJson.Images.Poster?.Medium,
                                    Large = animeJson.Images.Poster?.Large,
                                    Tiny = animeJson.Images.Poster?.Tiny,
                                    Original = animeJson.Images.Poster?.Original,
                                    Small = animeJson.Images.Poster?.Small
                                },
                                Cover = new ImageAnime
                                {
                                    Medium = animeJson.Images.Cover?.Medium,
                                    Large = animeJson.Images.Cover?.Large,
                                    Tiny = animeJson.Images.Cover?.Tiny,
                                    Original = animeJson.Images.Cover?.Original,
                                    Small = animeJson.Images.Cover?.Small
                                }
                            },
                            Rating = new Rating
                            {
                                Hated = animeJson.Rating.Hated,
                                Loved = animeJson.Rating.Loved,
                                Percentage = animeJson.Rating.Percentage,
                                Votes = animeJson.Rating.Votes,
                                Watching = animeJson.Rating.Watching
                            },
                            Year = animeJson.Year,
                            Runtime = animeJson.Runtime,
                            Genres = animeJson.Genres.Select(genre => new Genre
                            {
                                Name = genre.AsString
                            }).ToList(),
                            Slug = animeJson.Slug,
                            LastUpdated = animeJson.LastUpdated,
                            Title = animeJson.Title,
                            MalId = animeJson.MalId,
                            Episodes = animeJson.Episodes.Select(episode => new EpisodeAnime
                            {
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
                                Title = episode.Title,
                                Overview = episode.Overview,
                                Season = episode.Season,
                                TvdbId = episode.TvdbId
                            }).ToList(),
                            NumSeasons = animeJson.NumSeasons,
                            Status = animeJson.Status,
                            Synopsis = animeJson.Synopsis,
                            Type = animeJson.Type
                        };

                        var existingEntity = await context.AnimeSet.Include(a => a.Rating)
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
                            .Include(a => a.Images).FirstOrDefaultAsync(a => a.MalId == anime.MalId);

                        if (existingEntity == null)
                        {
                            context.AnimeSet.Add(anime);
                        }
                        else
                        {
                            existingEntity.Rating.Hated = anime.Rating.Hated;
                            existingEntity.Rating.Loved = anime.Rating.Loved;
                            existingEntity.Rating.Percentage = anime.Rating.Percentage;
                            existingEntity.Rating.Votes = anime.Rating.Votes;
                            existingEntity.Rating.Watching = anime.Rating.Watching;
                            existingEntity.Status = anime.Status;
                            existingEntity.NumSeasons = anime.NumSeasons;
                            foreach (var episode in existingEntity.Episodes)
                            {
                                var updatedEpisode = anime.Episodes.FirstOrDefault(a => a.TvdbId == episode.TvdbId);
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
                                anime.Episodes.Where(a => existingEntity.Episodes.All(b => b.TvdbId != a.TvdbId));
                            foreach (var newEpisode in newEpisodes)
                            {
                                anime.Episodes.Add(newEpisode);
                            }
                        }

                        await context.SaveChangesAsync();

                        watch.Stop();
                        updatedAnimes++;
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(
                            $"{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)} UPDATED ANIME {anime.Title} in {watch.ElapsedMilliseconds} ms. {updatedAnimes}/{documents.Count}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Telemetry.TrackException(ex);
                    }
                }
            }

            // Finish
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done processing animes.");

            var loggingTraceEnd =
                $@"Import animes ended at {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)}";
            _loggingService.Telemetry.TrackTrace(loggingTraceEnd);
        }

        /// <summary>
        /// Retrieve assets for the provided anime
        /// </summary>
        /// <param name="document"><see cref="BsonDocument"/> to update</param>
        /// <param name="anime">Anime to process</param>
        /// <returns></returns>
        private async Task RetrieveAssets(BsonDocument document, AnimeBson anime)
        {
            using (var client = new RestClient(Constants.KitsuApiUrl))
            {
                var request = new RestRequest("{segment}", Method.GET);
                request.AddUrlSegment("segment", $"{document["_id"]}");
                var response = await client.Execute(request);
                var animeKitsu = JsonConvert.DeserializeObject<AnimeKitsuWrapperJson>(response.Content).Anime;
                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                    {
                        if (animeKitsu.Attributes.CoverImage != null)
                        {
                            anime.Images.Cover = new AnimeKitsuImage();
                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Tiny))
                            {
                                var tinyCover = string.Concat(animeKitsu.Attributes.CoverImage.Tiny
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Tiny =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/tiny/{tinyCover}",
                                        animeKitsu.Attributes.CoverImage.Tiny);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Small))
                            {
                                var smallCover = string.Concat(animeKitsu.Attributes.CoverImage.Small
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Small =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/small/{smallCover}",
                                        animeKitsu.Attributes.CoverImage.Small);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Medium))
                            {
                                var mediumCover = string.Concat(animeKitsu.Attributes.CoverImage.Medium
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Medium =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/medium/{mediumCover}",
                                        animeKitsu.Attributes.CoverImage.Medium);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Large))
                            {
                                var largeCover = string.Concat(animeKitsu.Attributes.CoverImage.Large
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Large =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/large/{largeCover}",
                                        animeKitsu.Attributes.CoverImage.Large);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.CoverImage.Original))
                            {
                                var originalCover = string.Concat(animeKitsu.Attributes.CoverImage.Original
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Cover.Original =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/cover/original/{originalCover}",
                                        animeKitsu.Attributes.CoverImage.Original);
                            }
                        }
                    }),
                    Task.Run(async () =>
                    {
                        if (animeKitsu.Attributes.PosterImage != null)
                        {
                            anime.Images.Poster = new AnimeKitsuImage();
                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Tiny))
                            {
                                var tinyPoster = string.Concat(animeKitsu.Attributes.PosterImage.Tiny
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Tiny =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/tiny/{tinyPoster}",
                                        animeKitsu.Attributes.PosterImage.Tiny);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Small))
                            {
                                var smallPoster = string.Concat(animeKitsu.Attributes.PosterImage.Small
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Small =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/small/{smallPoster}",
                                        animeKitsu.Attributes.PosterImage.Small);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Medium))
                            {
                                var mediumPoster = string.Concat(animeKitsu.Attributes.PosterImage.Medium
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Medium =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/medium/{mediumPoster}",
                                        animeKitsu.Attributes.PosterImage.Medium);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Large))
                            {
                                var largePoster = string.Concat(animeKitsu.Attributes.PosterImage.Large
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Large =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/large/{largePoster}",
                                        animeKitsu.Attributes.PosterImage.Large);
                            }

                            if (!string.IsNullOrWhiteSpace(animeKitsu.Attributes.PosterImage.Original))
                            {
                                var originalPoster = string.Concat(animeKitsu.Attributes.PosterImage.Original
                                    .Split(
                                        '/').Last().TakeWhile(a => a != '?'));

                                anime.Images.Poster.Original =
                                    await _assetsService.UploadFile(
                                        $@"images/{anime.MalId}/poster/original/{originalPoster}",
                                        animeKitsu.Attributes.PosterImage.Original);
                            }
                        }
                    })
                };

                await Task.WhenAll(tasks);
            }
        }
    }
}