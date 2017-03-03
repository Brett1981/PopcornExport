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
                name: "ImageAnimeSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Large = table.Column<string>(nullable: true),
                    Medium = table.Column<string>(nullable: true),
                    Original = table.Column<string>(nullable: true),
                    Small = table.Column<string>(nullable: true),
                    Tiny = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAnimeSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageShowSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Banner = table.Column<string>(nullable: true),
                    Fanart = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageShowSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BackdropImage = table.Column<string>(nullable: true),
                    BackgroundImage = table.Column<string>(nullable: true),
                    DateUploaded = table.Column<string>(nullable: true),
                    DateUploadedUnix = table.Column<int>(nullable: false),
                    DescriptionFull = table.Column<string>(nullable: true),
                    DescriptionIntro = table.Column<string>(nullable: true),
                    DownloadCount = table.Column<int>(nullable: false),
                    ImdbCode = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    LargeCoverImage = table.Column<string>(nullable: true),
                    LargeScreenshotImage1 = table.Column<string>(nullable: true),
                    LargeScreenshotImage2 = table.Column<string>(nullable: true),
                    LargeScreenshotImage3 = table.Column<string>(nullable: true),
                    LikeCount = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_MovieSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatingSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Hated = table.Column<int>(nullable: false),
                    Loved = table.Column<int>(nullable: false),
                    Percentage = table.Column<int>(nullable: false),
                    Votes = table.Column<int>(nullable: false),
                    Watching = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Peers = table.Column<int>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    Seeds = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionImageAnimeSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CoverId = table.Column<int>(nullable: true),
                    PosterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionImageAnimeSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionImageAnimeSet_ImageAnimeSet_CoverId",
                        column: x => x.CoverId,
                        principalTable: "ImageAnimeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectionImageAnimeSet_ImageAnimeSet_PosterId",
                        column: x => x.PosterId,
                        principalTable: "ImageAnimeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CastSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CharacterName = table.Column<string>(nullable: true),
                    ImdbCode = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SmallImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastSet_MovieSet_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MovieSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentMovieSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateUploaded = table.Column<string>(nullable: true),
                    DateUploadedUnix = table.Column<int>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: false),
                    Peers = table.Column<int>(nullable: false),
                    Quality = table.Column<string>(nullable: true),
                    Seeds = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    SizeBytes = table.Column<long>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentMovieSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentMovieSet_MovieSet_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MovieSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AirDay = table.Column<string>(nullable: true),
                    AirTime = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    ImagesId = table.Column<int>(nullable: true),
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
                    table.PrimaryKey("PK_ShowSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowSet_ImageShowSet_ImagesId",
                        column: x => x.ImagesId,
                        principalTable: "ImageShowSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShowSet_RatingSet_RatingId",
                        column: x => x.RatingId,
                        principalTable: "RatingSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TorrentNodeSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Torrent0Id = table.Column<int>(nullable: true),
                    Torrent1080pId = table.Column<int>(nullable: true),
                    Torrent480pId = table.Column<int>(nullable: true),
                    Torrent720pId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentNodeSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentNodeSet_TorrentSet_Torrent0Id",
                        column: x => x.Torrent0Id,
                        principalTable: "TorrentSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNodeSet_TorrentSet_Torrent1080pId",
                        column: x => x.Torrent1080pId,
                        principalTable: "TorrentSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNodeSet_TorrentSet_Torrent480pId",
                        column: x => x.Torrent480pId,
                        principalTable: "TorrentSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TorrentNodeSet_TorrentSet_Torrent720pId",
                        column: x => x.Torrent720pId,
                        principalTable: "TorrentSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimeSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImagesId = table.Column<int>(nullable: true),
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
                    table.PrimaryKey("PK_AnimeSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimeSet_CollectionImageAnimeSet_ImagesId",
                        column: x => x.ImagesId,
                        principalTable: "CollectionImageAnimeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimeSet_RatingSet_RatingId",
                        column: x => x.RatingId,
                        principalTable: "RatingSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeShowSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateBased = table.Column<bool>(nullable: false),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    FirstAired = table.Column<long>(nullable: false),
                    Overview = table.Column<string>(nullable: true),
                    Season = table.Column<int>(nullable: false),
                    ShowId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TorrentsId = table.Column<int>(nullable: true),
                    TvdbId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeShowSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeShowSet_ShowSet_ShowId",
                        column: x => x.ShowId,
                        principalTable: "ShowSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpisodeShowSet_TorrentNodeSet_TorrentsId",
                        column: x => x.TorrentsId,
                        principalTable: "TorrentNodeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeAnimeSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnimeId = table.Column<int>(nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    Overview = table.Column<string>(nullable: true),
                    Season = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TorrentsId = table.Column<int>(nullable: true),
                    TvdbId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeAnimeSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpisodeAnimeSet_AnimeSet_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "AnimeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpisodeAnimeSet_TorrentNodeSet_TorrentsId",
                        column: x => x.TorrentsId,
                        principalTable: "TorrentNodeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GenreSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnimeId = table.Column<int>(nullable: true),
                    MovieId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ShowId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenreSet_AnimeSet_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "AnimeSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenreSet_MovieSet_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MovieSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenreSet_ShowSet_ShowId",
                        column: x => x.ShowId,
                        principalTable: "ShowSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimeSet_ImagesId",
                table: "AnimeSet",
                column: "ImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeSet_RatingId",
                table: "AnimeSet",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_CastSet_MovieId",
                table: "CastSet",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionImageAnimeSet_CoverId",
                table: "CollectionImageAnimeSet",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionImageAnimeSet_PosterId",
                table: "CollectionImageAnimeSet",
                column: "PosterId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeAnimeSet_AnimeId",
                table: "EpisodeAnimeSet",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeAnimeSet_TorrentsId",
                table: "EpisodeAnimeSet",
                column: "TorrentsId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeShowSet_ShowId",
                table: "EpisodeShowSet",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeShowSet_TorrentsId",
                table: "EpisodeShowSet",
                column: "TorrentsId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreSet_AnimeId",
                table: "GenreSet",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreSet_MovieId",
                table: "GenreSet",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreSet_ShowId",
                table: "GenreSet",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSet_ImagesId",
                table: "ShowSet",
                column: "ImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSet_RatingId",
                table: "ShowSet",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentMovieSet_MovieId",
                table: "TorrentMovieSet",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNodeSet_Torrent0Id",
                table: "TorrentNodeSet",
                column: "Torrent0Id");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNodeSet_Torrent1080pId",
                table: "TorrentNodeSet",
                column: "Torrent1080pId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNodeSet_Torrent480pId",
                table: "TorrentNodeSet",
                column: "Torrent480pId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentNodeSet_Torrent720pId",
                table: "TorrentNodeSet",
                column: "Torrent720pId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CastSet");

            migrationBuilder.DropTable(
                name: "EpisodeAnimeSet");

            migrationBuilder.DropTable(
                name: "EpisodeShowSet");

            migrationBuilder.DropTable(
                name: "GenreSet");

            migrationBuilder.DropTable(
                name: "TorrentMovieSet");

            migrationBuilder.DropTable(
                name: "TorrentNodeSet");

            migrationBuilder.DropTable(
                name: "AnimeSet");

            migrationBuilder.DropTable(
                name: "ShowSet");

            migrationBuilder.DropTable(
                name: "MovieSet");

            migrationBuilder.DropTable(
                name: "TorrentSet");

            migrationBuilder.DropTable(
                name: "CollectionImageAnimeSet");

            migrationBuilder.DropTable(
                name: "ImageShowSet");

            migrationBuilder.DropTable(
                name: "RatingSet");

            migrationBuilder.DropTable(
                name: "ImageAnimeSet");
        }
    }
}
