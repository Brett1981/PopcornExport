using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PopcornExport.Migrations
{
    public partial class DropAnime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenreSet_AnimeSet_AnimeId",
                table: "GenreSet");

            migrationBuilder.DropTable(
                name: "EpisodeAnimeSet");

            migrationBuilder.DropTable(
                name: "AnimeSet");

            migrationBuilder.DropTable(
                name: "CollectionImageAnimeSet");

            migrationBuilder.DropTable(
                name: "ImageAnimeSet");

            migrationBuilder.DropIndex(
                name: "IX_GenreSet_AnimeId",
                table: "GenreSet");

            migrationBuilder.DropColumn(
                name: "AnimeId",
                table: "GenreSet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnimeId",
                table: "GenreSet",
                nullable: true);

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
                    Year = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_GenreSet_AnimeId",
                table: "GenreSet",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeSet_ImagesId",
                table: "AnimeSet",
                column: "ImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeSet_RatingId",
                table: "AnimeSet",
                column: "RatingId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_GenreSet_AnimeSet_AnimeId",
                table: "GenreSet",
                column: "AnimeId",
                principalTable: "AnimeSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
