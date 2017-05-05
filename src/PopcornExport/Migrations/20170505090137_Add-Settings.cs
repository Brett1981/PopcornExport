using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PopcornExport.Migrations
{
    public partial class AddSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultHdQuality",
                table: "UserSet",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DefaultSubtitleLanguage",
                table: "UserSet",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultHdQuality",
                table: "UserSet");

            migrationBuilder.DropColumn(
                name: "DefaultSubtitleLanguage",
                table: "UserSet");
        }
    }
}
