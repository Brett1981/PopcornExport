using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PopcornExport.Migrations
{
    public partial class GenreNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenreNames",
                table: "ShowSet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenreNames",
                table: "MovieSet",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenreNames",
                table: "ShowSet");

            migrationBuilder.DropColumn(
                name: "GenreNames",
                table: "MovieSet");
        }
    }
}
