using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PopcornExport.Database;

namespace PopcornExport.Migrations
{
    [DbContext(typeof(PopcornContext))]
    partial class PopcornContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PopcornExport.Database.Anime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ImagesId");

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

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("ImagesId");

                    b.HasIndex("RatingId");

                    b.ToTable("AnimeSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Cast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CharacterName");

                    b.Property<string>("ImdbCode");

                    b.Property<int?>("MovieId");

                    b.Property<string>("Name");

                    b.Property<string>("SmallImage");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("CastSet");
                });

            modelBuilder.Entity("PopcornExport.Database.CollectionImageAnime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CoverId");

                    b.Property<int?>("PosterId");

                    b.HasKey("Id");

                    b.HasIndex("CoverId");

                    b.HasIndex("PosterId");

                    b.ToTable("CollectionImageAnimeSet");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeAnime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AnimeId");

                    b.Property<int>("EpisodeNumber");

                    b.Property<string>("Overview");

                    b.Property<int>("Season");

                    b.Property<string>("Title");

                    b.Property<int?>("TorrentsId");

                    b.Property<string>("TvdbId");

                    b.HasKey("Id");

                    b.HasIndex("AnimeId");

                    b.HasIndex("TorrentsId");

                    b.ToTable("EpisodeAnimeSet");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeShow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("DateBased");

                    b.Property<int>("EpisodeNumber");

                    b.Property<long>("FirstAired");

                    b.Property<string>("Overview");

                    b.Property<int>("Season");

                    b.Property<int?>("ShowId");

                    b.Property<string>("Title");

                    b.Property<int?>("TorrentsId");

                    b.Property<int?>("TvdbId");

                    b.HasKey("Id");

                    b.HasIndex("ShowId");

                    b.HasIndex("TorrentsId");

                    b.ToTable("EpisodeShowSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AnimeId");

                    b.Property<int?>("MovieId");

                    b.Property<string>("Name");

                    b.Property<int?>("ShowId");

                    b.HasKey("Id");

                    b.HasIndex("AnimeId");

                    b.HasIndex("MovieId");

                    b.HasIndex("ShowId");

                    b.ToTable("GenreSet");
                });

            modelBuilder.Entity("PopcornExport.Database.ImageAnime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Large");

                    b.Property<string>("Medium");

                    b.Property<string>("Original");

                    b.Property<string>("Small");

                    b.Property<string>("Tiny");

                    b.HasKey("Id");

                    b.ToTable("ImageAnimeSet");
                });

            modelBuilder.Entity("PopcornExport.Database.ImageShow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<string>("Fanart");

                    b.Property<string>("Poster");

                    b.HasKey("Id");

                    b.ToTable("ImageShowSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BackdropImage");

                    b.Property<string>("BackgroundImage");

                    b.Property<string>("DateUploaded");

                    b.Property<int>("DateUploadedUnix");

                    b.Property<string>("DescriptionFull");

                    b.Property<string>("DescriptionIntro");

                    b.Property<int>("DownloadCount");

                    b.Property<string>("ImdbCode");

                    b.Property<string>("Language");

                    b.Property<string>("LargeCoverImage");

                    b.Property<string>("LargeScreenshotImage1");

                    b.Property<string>("LargeScreenshotImage2");

                    b.Property<string>("LargeScreenshotImage3");

                    b.Property<int>("LikeCount");

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

                    b.HasKey("Id");

                    b.ToTable("MovieSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Hated");

                    b.Property<int>("Loved");

                    b.Property<int>("Percentage");

                    b.Property<int>("Votes");

                    b.Property<int>("Watching");

                    b.HasKey("Id");

                    b.ToTable("RatingSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Show", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirDay");

                    b.Property<string>("AirTime");

                    b.Property<string>("Country");

                    b.Property<int?>("ImagesId");

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

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("ImagesId");

                    b.HasIndex("RatingId");

                    b.ToTable("ShowSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Torrent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Peers");

                    b.Property<string>("Provider");

                    b.Property<int?>("Seeds");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("TorrentSet");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentMovie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateUploaded");

                    b.Property<int>("DateUploadedUnix");

                    b.Property<string>("Hash");

                    b.Property<int>("MovieId");

                    b.Property<int>("Peers");

                    b.Property<string>("Quality");

                    b.Property<int>("Seeds");

                    b.Property<string>("Size");

                    b.Property<long?>("SizeBytes");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("TorrentMovieSet");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Torrent0Id");

                    b.Property<int?>("Torrent1080pId");

                    b.Property<int?>("Torrent480pId");

                    b.Property<int?>("Torrent720pId");

                    b.HasKey("Id");

                    b.HasIndex("Torrent0Id");

                    b.HasIndex("Torrent1080pId");

                    b.HasIndex("Torrent480pId");

                    b.HasIndex("Torrent720pId");

                    b.ToTable("TorrentNodeSet");
                });

            modelBuilder.Entity("PopcornExport.Database.Anime", b =>
                {
                    b.HasOne("PopcornExport.Database.CollectionImageAnime", "Images")
                        .WithMany()
                        .HasForeignKey("ImagesId");

                    b.HasOne("PopcornExport.Database.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId");
                });

            modelBuilder.Entity("PopcornExport.Database.Cast", b =>
                {
                    b.HasOne("PopcornExport.Database.Movie")
                        .WithMany("Cast")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("PopcornExport.Database.CollectionImageAnime", b =>
                {
                    b.HasOne("PopcornExport.Database.ImageAnime", "Cover")
                        .WithMany()
                        .HasForeignKey("CoverId");

                    b.HasOne("PopcornExport.Database.ImageAnime", "Poster")
                        .WithMany()
                        .HasForeignKey("PosterId");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeAnime", b =>
                {
                    b.HasOne("PopcornExport.Database.Anime")
                        .WithMany("Episodes")
                        .HasForeignKey("AnimeId");

                    b.HasOne("PopcornExport.Database.TorrentNode", "Torrents")
                        .WithMany()
                        .HasForeignKey("TorrentsId");
                });

            modelBuilder.Entity("PopcornExport.Database.EpisodeShow", b =>
                {
                    b.HasOne("PopcornExport.Database.Show")
                        .WithMany("Episodes")
                        .HasForeignKey("ShowId");

                    b.HasOne("PopcornExport.Database.TorrentNode", "Torrents")
                        .WithMany()
                        .HasForeignKey("TorrentsId");
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
                        .HasForeignKey("ImagesId");

                    b.HasOne("PopcornExport.Database.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId");
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentMovie", b =>
                {
                    b.HasOne("PopcornExport.Database.Movie")
                        .WithMany("Torrents")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PopcornExport.Database.TorrentNode", b =>
                {
                    b.HasOne("PopcornExport.Database.Torrent", "Torrent0")
                        .WithMany()
                        .HasForeignKey("Torrent0Id");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent1080p")
                        .WithMany()
                        .HasForeignKey("Torrent1080pId");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent480p")
                        .WithMany()
                        .HasForeignKey("Torrent480pId");

                    b.HasOne("PopcornExport.Database.Torrent", "Torrent720p")
                        .WithMany()
                        .HasForeignKey("Torrent720pId");
                });
        }
    }
}
