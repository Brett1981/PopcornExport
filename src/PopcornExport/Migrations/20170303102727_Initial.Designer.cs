using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PopcornExport.Database;

namespace PopcornExport.Migrations
{
    [DbContext(typeof(PopcornContext))]
    [Migration("20170303102727_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PopcornExport.Database.Anime", b =>
                {
                    b.Property<int>("AnimeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ImagesCollectionImageAnimeId");

                    b.Property<long>("LastUpdated");

                    b.Property<string>("MalId");

                    b.Property<int>("NumSeasons");

                    b.Property<int?>("RatingId");

                    b.Property<string>("Runtime");

                    b.Property<string>("Slug");

                    b.Property<string>("Status");

                    b.Property<string>("Synopsis");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.Property<string>("Year");

                    b.HasKey("AnimeId");

                    b.HasIndex("ImagesCollectionImageAnimeId");

                    b.HasIndex("RatingId");

                    b.ToTable("Animes");
                });

            modelBuilder.Entity("PopcornExport.Database.Cast", b =>
                {
                    b.Property<int>("CastId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CharacterName");

                    b.Property<string>("ImdbCode");

                    b.Property<int?>("MovieId");

                    b.Property<string>("Name");

                    b.Property<string>("SmallImage");

                    b.HasKey("CastId");

                    b.HasIndex("MovieId");

                    b.ToTable("Cast");
                });

            modelBuilder.Entity("PopcornExport.Database.CollectionImageAnime", b =>
                {
                    b.Property<int>("CollectionImageAnimeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CoverImageAnimeId");

                    b.Property<int?>("PosterImageAnimeId");

                    b.HasKey("CollectionImageAnimeId");

                    b.HasIndex("CoverImageAnimeId");

                    b.HasIndex("PosterImageAnimeId");

                    b.ToTable("CollectionImageAnime");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeAnime", b =>
                {
                    b.Property<int>("EpisodeAnimeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AnimeId");

                    b.Property<int>("EpisodeNumber");

                    b.Property<string>("Overview");

                    b.Property<int>("Season");

                    b.Property<string>("Title");

                    b.Property<int?>("TorrentsTorrentNodeId");

                    b.Property<string>("TvdbId");

                    b.HasKey("EpisodeAnimeId");

                    b.HasIndex("AnimeId");

                    b.HasIndex("TorrentsTorrentNodeId");

                    b.ToTable("EpisodeAnime");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeShow", b =>
                {
                    b.Property<int>("EpisodeShowId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("DateBased");

                    b.Property<int>("EpisodeNumber");

                    b.Property<long>("FirstAired");

                    b.Property<string>("Overview");

                    b.Property<int>("Season");

                    b.Property<int?>("ShowId");

                    b.Property<string>("Title");

                    b.Property<int?>("TorrentsTorrentNodeId");

                    b.Property<int?>("TvdbId");

                    b.HasKey("EpisodeShowId");

                    b.HasIndex("ShowId");

                    b.HasIndex("TorrentsTorrentNodeId");

                    b.ToTable("EpisodeShow");
                });

            modelBuilder.Entity("PopcornExport.Database.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AnimeId");

                    b.Property<int?>("MovieId");

                    b.Property<string>("Name");

                    b.Property<int?>("ShowId");

                    b.HasKey("GenreId");

                    b.HasIndex("AnimeId");

                    b.HasIndex("MovieId");

                    b.HasIndex("ShowId");

                    b.ToTable("Genre");
                });

            modelBuilder.Entity("PopcornExport.Database.ImageAnime", b =>
                {
                    b.Property<int>("ImageAnimeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Large");

                    b.Property<string>("Medium");

                    b.Property<string>("Original");

                    b.Property<string>("Small");

                    b.Property<string>("Tiny");

                    b.HasKey("ImageAnimeId");

                    b.ToTable("ImageAnime");
                });

            modelBuilder.Entity("PopcornExport.Database.ImageShow", b =>
                {
                    b.Property<int>("ImageShowId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<string>("Fanart");

                    b.Property<string>("Poster");

                    b.HasKey("ImageShowId");

                    b.ToTable("ImageShow");
                });

            modelBuilder.Entity("PopcornExport.Database.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BackdropImage");

                    b.Property<string>("BackgroundImage");

                    b.Property<string>("DateUploaded");

                    b.Property<int>("DateUploadedUnix");

                    b.Property<string>("DescriptionFull");

                    b.Property<string>("DescriptionIntro");

                    b.Property<string>("DownloadCount");

                    b.Property<string>("ImdbCode");

                    b.Property<string>("Language");

                    b.Property<string>("LargeCoverImage");

                    b.Property<string>("LargeScreenshotImage1");

                    b.Property<string>("LargeScreenshotImage2");

                    b.Property<string>("LargeScreenshotImage3");

                    b.Property<string>("LikeCount");

                    b.Property<string>("MediumCoverImage");

                    b.Property<string>("MediumScreenshotImage1");

                    b.Property<string>("MediumScreenshotImage2");

                    b.Property<string>("MediumScreenshotImage3");

                    b.Property<string>("MpaRating");

                    b.Property<string>("PosterImage");

                    b.Property<double>("Rating");

                    b.Property<int>("Runtime");

                    b.Property<string>("Slug");

                    b.Property<string>("SmallCoverImage");

                    b.Property<string>("Title");

                    b.Property<string>("TitleLong");

                    b.Property<string>("Url");

                    b.Property<int>("Year");

                    b.Property<string>("YtTrailerCode");

                    b.HasKey("MovieId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("PopcornExport.Database.Rating", b =>
                {
                    b.Property<int>("RatingId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Hated");

                    b.Property<int>("Loved");

                    b.Property<int>("Percentage");

                    b.Property<int>("Votes");

                    b.Property<int>("Watching");

                    b.HasKey("RatingId");

                    b.ToTable("Rating");
                });

            modelBuilder.Entity("PopcornExport.Database.Show", b =>
                {
                    b.Property<int>("ShowId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirDay");

                    b.Property<string>("AirTime");

                    b.Property<string>("Country");

                    b.Property<int?>("ImageShowId");

                    b.Property<string>("ImdbId");

                    b.Property<long>("LastUpdated");

                    b.Property<string>("Network");

                    b.Property<int>("NumSeasons");

                    b.Property<int?>("RatingId");

                    b.Property<string>("Runtime");

                    b.Property<string>("Slug");

                    b.Property<string>("Status");

                    b.Property<string>("Synopsis");

                    b.Property<string>("Title");

                    b.Property<string>("TvdbId");

                    b.Property<string>("Year");

                    b.HasKey("ShowId");

                    b.HasIndex("ImageShowId");

                    b.HasIndex("RatingId");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("PopcornExport.Database.Torrent", b =>
                {
                    b.Property<int>("TorrentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Peers");

                    b.Property<string>("Provider");

                    b.Property<int?>("Seeds");

                    b.Property<string>("Url");

                    b.HasKey("TorrentId");

                    b.ToTable("Torrent");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentMovie", b =>
                {
                    b.Property<int>("TorrentMovieId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateUploaded");

                    b.Property<int>("DateUploadedUnix");

                    b.Property<string>("Hash");

                    b.Property<int?>("MovieId");

                    b.Property<int>("Peers");

                    b.Property<string>("Quality");

                    b.Property<int>("Seeds");

                    b.Property<string>("Size");

                    b.Property<long?>("SizeBytes");

                    b.Property<string>("Url");

                    b.HasKey("TorrentMovieId");

                    b.HasIndex("MovieId");

                    b.ToTable("TorrentMovie");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentNode", b =>
                {
                    b.Property<int>("TorrentNodeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Torrent0TorrentId");

                    b.Property<int?>("Torrent1080PTorrentId");

                    b.Property<int?>("Torrent480PTorrentId");

                    b.Property<int?>("Torrent720PTorrentId");

                    b.HasKey("TorrentNodeId");

                    b.HasIndex("Torrent0TorrentId");

                    b.HasIndex("Torrent1080PTorrentId");

                    b.HasIndex("Torrent480PTorrentId");

                    b.HasIndex("Torrent720PTorrentId");

                    b.ToTable("TorrentNode");
                });

            modelBuilder.Entity("PopcornExport.Database.Anime", b =>
                {
                    b.HasOne("PopcornExport.Database.CollectionImageAnime", "Images")
                        .WithMany()
                        .HasForeignKey("ImagesCollectionImageAnimeId");

                    b.HasOne("PopcornExport.Database.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId");
                });

            modelBuilder.Entity("PopcornExport.Database.Cast", b =>
                {
                    b.HasOne("PopcornExport.Database.Movie")
                        .WithMany("Casts")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("PopcornExport.Database.CollectionImageAnime", b =>
                {
                    b.HasOne("PopcornExport.Database.ImageAnime", "Cover")
                        .WithMany()
                        .HasForeignKey("CoverImageAnimeId");

                    b.HasOne("PopcornExport.Database.ImageAnime", "Poster")
                        .WithMany()
                        .HasForeignKey("PosterImageAnimeId");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeAnime", b =>
                {
                    b.HasOne("PopcornExport.Database.Anime")
                        .WithMany("Episodes")
                        .HasForeignKey("AnimeId");

                    b.HasOne("PopcornExport.Database.TorrentNode", "Torrents")
                        .WithMany()
                        .HasForeignKey("TorrentsTorrentNodeId");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeShow", b =>
                {
                    b.HasOne("PopcornExport.Database.Show")
                        .WithMany("Episodes")
                        .HasForeignKey("ShowId");

                    b.HasOne("PopcornExport.Database.TorrentNode", "Torrents")
                        .WithMany()
                        .HasForeignKey("TorrentsTorrentNodeId");
                });

            modelBuilder.Entity("PopcornExport.Database.Genre", b =>
                {
                    b.HasOne("PopcornExport.Database.Anime")
                        .WithMany("Genres")
                        .HasForeignKey("AnimeId");

                    b.HasOne("PopcornExport.Database.Movie")
                        .WithMany("Genres")
                        .HasForeignKey("MovieId");

                    b.HasOne("PopcornExport.Database.Show")
                        .WithMany("Genres")
                        .HasForeignKey("ShowId");
                });

            modelBuilder.Entity("PopcornExport.Database.Show", b =>
                {
                    b.HasOne("PopcornExport.Database.ImageShow", "Images")
                        .WithMany()
                        .HasForeignKey("ImageShowId");

                    b.HasOne("PopcornExport.Database.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentMovie", b =>
                {
                    b.HasOne("PopcornExport.Database.Movie")
                        .WithMany("Torrents")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentNode", b =>
                {
                    b.HasOne("PopcornExport.Database.Torrent", "Torrent0")
                        .WithMany()
                        .HasForeignKey("Torrent0TorrentId");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent1080P")
                        .WithMany()
                        .HasForeignKey("Torrent1080PTorrentId");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent480P")
                        .WithMany()
                        .HasForeignKey("Torrent480PTorrentId");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent720P")
                        .WithMany()
                        .HasForeignKey("Torrent720PTorrentId");
                });
        }
    }
}
