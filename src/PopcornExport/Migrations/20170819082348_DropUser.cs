using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PopcornExport.Migrations
{
    public partial class DropUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieHistory");

            migrationBuilder.DropTable(
                name: "ShowHistory");

            migrationBuilder.DropTable(
                name: "UserSet");

            migrationBuilder.DropTable(
                name: "Language");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Culture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DefaultHdQuality = table.Column<bool>(nullable: false),
                    DefaultSubtitleLanguage = table.Column<string>(nullable: true),
                    DownloadLimit = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: true),
                    MachineGuid = table.Column<Guid>(nullable: false),
                    UploadLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSet_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Favorite = table.Column<bool>(nullable: false),
                    ImdbId = table.Column<string>(nullable: true),
                    Seen = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieHistory_UserSet_UserId",
                        column: x => x.UserId,
                        principalTable: "UserSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShowHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Favorite = table.Column<bool>(nullable: false),
                    ImdbId = table.Column<string>(nullable: true),
                    Seen = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowHistory_UserSet_UserId",
                        column: x => x.UserId,
                        principalTable: "UserSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieHistory_UserId",
                table: "MovieHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowHistory_UserId",
                table: "ShowHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSet_LanguageId",
                table: "UserSet",
                column: "LanguageId");
        }
    }
}
