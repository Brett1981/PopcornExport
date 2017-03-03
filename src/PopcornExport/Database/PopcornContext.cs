using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace PopcornExport.Database
{
    public class PopcornContext : DbContext
    {
        public PopcornContext(DbContextOptions<PopcornContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Show> Shows { get; set; }

        public DbSet<Anime> Animes { get; set; }
    }

    public class Movie
    {
        public int MovieId { get; set; }

        public string Url { get; set; }

        public string ImdbCode { get; set; }

        public string Title { get; set; }

        public string TitleLong { get; set; }

        public string Slug { get; set; }

        public int Year { get; set; }

        public double Rating { get; set; }

        public int Runtime { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

        public string Language { get; set; }

        public string MpaRating { get; set; }

        public string DownloadCount { get; set; }

        public string LikeCount { get; set; }

        public string DescriptionIntro { get; set; }

        public string DescriptionFull { get; set; }

        public string YtTrailerCode { get; set; }

        public virtual ICollection<Cast> Casts { get; set; }

        public virtual ICollection<TorrentMovie> Torrents { get; set; }

        public string DateUploaded { get; set; }

        public int DateUploadedUnix { get; set; }

        public string PosterImage { get; set; }

        public string BackdropImage { get; set; }

        public string BackgroundImage { get; set; }

        public string SmallCoverImage { get; set; }

        public string MediumCoverImage { get; set; }

        public string LargeCoverImage { get; set; }

        public string MediumScreenshotImage1 { get; set; }

        public string MediumScreenshotImage2 { get; set; }

        public string MediumScreenshotImage3 { get; set; }

        public string LargeScreenshotImage1 { get; set; }

        public string LargeScreenshotImage2 { get; set; }

        public string LargeScreenshotImage3 { get; set; }
    }

    public class Cast
    {
        public int CastId { get; set; }

        public string Name { get; set; }

        public string CharacterName { get; set; }

        public string SmallImage { get; set; }

        public string ImdbCode { get; set; }
    }

    public class TorrentMovie
    {
        public int TorrentMovieId { get; set; }

        public string Url { get; set; }

        public string Hash { get; set; }

        public string Quality { get; set; }

        public int Seeds { get; set; }

        public int Peers { get; set; }

        public string Size { get; set; }

        public long? SizeBytes { get; set; }

        public string DateUploaded { get; set; }

        public int DateUploadedUnix { get; set; }
    }

    public class Show
    {
        public int ShowId { get; set; }

        public string ImdbId { get; set; }

        public string TvdbId { get; set; }

        public string Title { get; set; }

        public string Year { get; set; }

        public string Slug { get; set; }

        public string Synopsis { get; set; }

        public string Runtime { get; set; }

        public string Country { get; set; }

        public string Network { get; set; }

        public string AirDay { get; set; }

        public string AirTime { get; set; }

        public string Status { get; set; }

        public int NumSeasons { get; set; }

        public long LastUpdated { get; set; }

        public virtual ICollection<EpisodeShow> Episodes { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

        public ImageShow Images { get; set; }

        public Rating Rating { get; set; }
    }

    public class EpisodeShow
    {
        public int EpisodeShowId { get; set; }

        public TorrentNode Torrents { get; set; }

        public long FirstAired { get; set; }

        public bool DateBased { get; set; }

        public string Overview { get; set; }

        public string Title { get; set; }

        public int EpisodeNumber { get; set; }

        public int Season { get; set; }

        public int? TvdbId { get; set; }
    }

    public class TorrentNode
    {
        public int TorrentNodeId { get; set; }

        public Torrent Torrent0 { get; set; }

        public Torrent Torrent480P { get; set; }

        public Torrent Torrent720P { get; set; }

        public Torrent Torrent1080P { get; set; }
    }

    public class Torrent
    {
        public int TorrentId { get; set; }

        public string Provider { get; set; }

        public int? Peers { get; set; }

        public int? Seeds { get; set; }

        public string Url { get; set; }
    }

    public class ImageShow
    {
        public int ImageShowId { get; set; }

        public string Poster { get; set; }

        public string Fanart { get; set; }

        public string Banner { get; set; }
    }

    public class Rating
    {
        public int RatingId { get; set; }

        public int Percentage { get; set; }

        public int Watching { get; set; }

        public int Votes { get; set; }

        public int Loved { get; set; }

        public int Hated { get; set; }
    }

    public class Anime
    {
        public int AnimeId { get; set; }

        public string MalId { get; set; }

        public string Title { get; set; }

        public string Year { get; set; }

        public string Slug { get; set; }

        public string Synopsis { get; set; }

        public string Runtime { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public long LastUpdated { get; set; }

        public int NumSeasons { get; set; }

        public virtual ICollection<EpisodeAnime> Episodes { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

        public CollectionImageAnime Images { get; set; }

        public Rating Rating { get; set; }
    }

    public class Genre
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

    }
    public class EpisodeAnime
    {
        public int EpisodeAnimeId { get; set; }

        public TorrentNode Torrents { get; set; }

        public string Overview { get; set; }

        public string Title { get; set; }

        public int EpisodeNumber { get; set; }

        public int Season { get; set; }

        public string TvdbId { get; set; }
    }

    public class CollectionImageAnime
    {
        public int CollectionImageAnimeId { get; set; }

        public ImageAnime Poster { get; set; }

        public ImageAnime Cover { get; set; }
    }

    public class ImageAnime
    {
        public int ImageAnimeId { get; set; }

        public string Tiny { get; set; }

        public string Small { get; set; }

        public string Medium { get; set; }

        public string Large { get; set; }

        public string Original { get; set; }
    }
}