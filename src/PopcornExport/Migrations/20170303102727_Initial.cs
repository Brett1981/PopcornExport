using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PopcornExport.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageAnime",
                columns: table => new
                {
                    ImageAnimeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Large = table.Column<string>(nullable: true),
                    Medium = table.Column<string>(nullable: true),
                    Original = table.Column<string>(nullable: true),
                    Small = table.Column<string>(nullable: true),
                    Tiny = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnime", x => x.ImageAnimeId);
                });

            migrationBuilder.CreateTable(
                name: "ImageShow",
                columns: table => new
                {
                    ImageShowId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Banner = table.Column<string>(nullable: true),
                    Fanart = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageShow", x => x.ImageShowId);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BackdropImage = table.Column<string>(nullable: true),
                    BackgroundImage = table.Column<string>(nullable: true),
                    DateUploaded = table.Column<string>(nullable: true),
                    DateUploadedUnix = table.Column<int>(nullable: false),
                    DescriptionFull = table.Column<string>(nullable: true),
                    DescriptionIntro = table.Column<string>(nullable: true),
                    DownloadCount = table.Column<string>(nullable: true),
                    ImdbCode = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    LargeCoverImage = table.Column<string>(nullable: true),
                    LargeScreenshotImage1 = table.Column<string>(nullable: true),
                    LargeScreenshotImage2 = table.Column<string>(nullable: true),
                    LargeScreenshotImage3 = table.Column<string>(nullable: true),
                    LikeCount = table.Column<string>(nullable: true),
                    MediumCoverImage = table.Column<string>(nullable: true),
                    MediumScreenshotImage1 = table.Column<string>(nullable: true),
                    MediumScreenshotImage2 = table.Column<string>(nullable: true),
                    MediumScreenshotImage3 = table.Column<string>(nullable: true),
                    MpaRating = table.Column<string>(nullable: true),
                    PosterImage = table.Column<string>(nullable: true),
                    Rating = table.Column<double>(nullable: false),
                    Runtime = table.Column<int>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    SmallCoverImage = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TitleLong = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    YtTrailerCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Hated = table.Column<int>(nullable: false),
                    Loved = table.Column<int>(nullable: false),
                    Percentage = table.Column<int>(nullable: false),
                    Votes = table.Column<int>(nullable: false),
                    Watching = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.RatingId);
                });

            migrationBuilder.CreateTable(
                name: "Torrent",
                columns: table => new
                {
                    TorrentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Peers = table.Column<int>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    Seeds = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torrent", x => x.TorrentId);
                });

            migrationBuilder.CreateTable(
                name: "CollectionImageAnime",
                columns: table => new
                {
                    CollectionImageAnimeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CoverImageAnimeId = table.Column<int>(nullable: true),
                    PosterImageAnimeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionImageAnime", x => x.CollectionImageAnimeId);
                    table.ForeignKey(
                        name: "FK_CollectionImageAnime_ImageAnime_CoverImageAnimeId",
                        column: x => x.CoverImageAnimeId,
                        principalTable: "ImageAnime",
                        principalColumn: "ImageAnimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectionImageAnime_ImageAnime_PosterImageAnimeId",
                        column: x => x.PosterImageAnimeId,
                        principalTable: "ImageAnime",
                        principalColumn: "ImageAnimeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cast",
                columns: table => new
                {
                    CastId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CharacterName = table.Column<string>(nullable: true),
                    ImdbCode = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SmallImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cast", x => x.CastId);
                    table.ForeignKey(
                        name: "FK_Cast_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentMovie",
                columns: table => new
                {
                    TorrentMovieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateUploaded = table.Column<string>(nullable: true),
                    DateUploadedUnix = table.Column<int>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: true),
                    Peers = table.Column<int>(nullable: false),
                    Quality = table.Column<string>(nullable: true),
                    Seeds = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    SizeBytes = table.Column<long>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentMovie", x => x.TorrentMovieId);
                    table.ForeignKey(
                        name: "FK_TorrentMovie_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    ShowId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AirDay = table.Column<string>(nullable: true),
                    AirTime = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    ImageShowId = table.Column<int>(nullable: true),
                    ImdbId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<long>(nullable: false),
                    Network = table.Column<string>(nullable: true),
                    NumSeasons = table.Column<int>(nullable: false),
                    RatingId = table.Column<int>(nullable: true),
                    Runtime = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Synopsis = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TvdbId = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.ShowId);
                    table.ForeignKey(
                        name: "FK_Shows_ImageShow_ImageShowId",
                        column: x => x.ImageShowId,
                        principalTable: "ImageShow",
                        principalColumn: "ImageShowId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shows_Rating_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Rating",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentNode",
                columns: table => new
                {
                    TorrentNodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Torrent0TorrentId = table.Column<int>(nullable: true),
                    Torrent1080PTorrentId = table.Column<int>(nullable: true),
                    Torrent480PTorrentId = table.Column<int>(nullable: true),
                    Torrent720PTorrentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentNode", x => x.TorrentNodeId);
                    table.ForeignKey(
                        name: "FK_TorrentNode_Torrent_Torrent0TorrentId",
                        column: x => x.Torrent0TorrentId,
                        principalTable: "Torrent",
                        principalColumn: "TorrentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNode_Torrent_Torrent1080PTorrentId",
                        column: x => x.Torrent1080PTorrentId,
                        principalTable: "Torrent",
                        principalColumn: "TorrentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNode_Torrent_Torrent480PTorrentId",
                        column: x => x.Torrent480PTorrentId,
                        principalTable: "Torrent",
                        principalColumn: "TorrentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNode_Torrent_Torrent720PTorrentId",
                        column: x => x.Torrent720PTorrentId,
                        principalTable: "Torrent",
                        principalColumn: "TorrentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Animes",
                columns: table => new
                {
                    AnimeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImagesCollectionImageAnimeId = table.Column<int>(nullable: true),
                    LastUpdated = table.Column<long>(nullable: false),
                    MalId = table.Column<string>(nullable: true),
                    NumSeasons = table.Column<int>(nullable: false),
                    RatingId = table.Column<int>(nullable: true),
                    Runtime = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Synopsis = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animes", x => x.AnimeId);
                    table.ForeignKey(
                        name: "FK_Animes_CollectionImageAnime_ImagesCollectionImageAnimeId",
                        column: x => x.ImagesCollectionImageAnimeId,
                        principalTable: "CollectionImageAnime",
                        principalColumn: "CollectionImageAnimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Animes_Rating_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Rating",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeShow",
                columns: table => new
                {
                    EpisodeShowId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateBased = table.Column<bool>(nullable: false),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    FirstAired = table.Column<long>(nullable: false),
                    Overview = table.Column<string>(nullable: true),
                    Season = table.Column<int>(nullable: false),
                    ShowId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TorrentsTorrentNodeId = table.Column<int>(nullable: true),
                    TvdbId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeShow", x => x.EpisodeShowId);
                    table.ForeignKey(
                        name: "FK_EpisodeShow_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpisodeShow_TorrentNode_TorrentsTorrentNodeId",
                        column: x => x.TorrentsTorrentNodeId,
                        principalTable: "TorrentNode",
                        principalColumn: "TorrentNodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeAnime",
                columns: table => new
                {
                    EpisodeAnimeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnimeId = table.Column<int>(nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    Overview = table.Column<string>(nullable: true),
                    Season = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TorrentsTorrentNodeId = table.Column<int>(nullable: true),
                    TvdbId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeAnime", x => x.EpisodeAnimeId);
                    table.ForeignKey(
                        name: "FK_EpisodeAnime_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "AnimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpisodeAnime_TorrentNode_TorrentsTorrentNodeId",
                        column: x => x.TorrentsTorrentNodeId,
                        principalTable: "TorrentNode",
                        principalColumn: "TorrentNodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    GenreId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnimeId = table.Column<int>(nullable: true),
                    MovieId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ShowId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.GenreId);
                    table.ForeignKey(
                        name: "FK_Genre_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "AnimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Genre_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Genre_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animes_ImagesCollectionImageAnimeId",
                table: "Animes",
                column: "ImagesCollectionImageAnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_RatingId",
                table: "Animes",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cast_MovieId",
                table: "Cast",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionImageAnime_CoverImageAnimeId",
                table: "CollectionImageAnime",
                column: "CoverImageAnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionImageAnime_PosterImageAnimeId",
                table: "CollectionImageAnime",
                column: "PosterImageAnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeAnime_AnimeId",
                table: "EpisodeAnime",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeAnime_TorrentsTorrentNodeId",
                table: "EpisodeAnime",
                column: "TorrentsTorrentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeShow_ShowId",
                table: "EpisodeShow",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeShow_TorrentsTorrentNodeId",
                table: "EpisodeShow",
                column: "TorrentsTorrentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Genre_AnimeId",
                table: "Genre",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Genre_MovieId",
                table: "Genre",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Genre_ShowId",
                table: "Genre",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ImageShowId",
                table: "Shows",
                column: "ImageShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_RatingId",
                table: "Shows",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentMovie_MovieId",
                table: "TorrentMovie",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNode_Torrent0TorrentId",
                table: "TorrentNode",
                column: "Torrent0TorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNode_Torrent1080PTorrentId",
                table: "TorrentNode",
                column: "Torrent1080PTorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNode_Torrent480PTorrentId",
                table: "TorrentNode",
                column: "Torrent480PTorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNode_Torrent720PTorrentId",
                table: "TorrentNode",
                column: "Torrent720PTorrentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cast");

            migrationBuilder.DropTable(
                name: "EpisodeAnime");

            migrationBuilder.DropTable(
                name: "EpisodeShow");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "TorrentMovie");

            migrationBuilder.DropTable(
                name: "TorrentNode");

            migrationBuilder.DropTable(
                name: "Animes");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Torrent");

            migrationBuilder.DropTable(
                name: "CollectionImageAnime");

            migrationBuilder.DropTable(
                name: "ImageShow");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "ImageAnime");
        }
    }
}
