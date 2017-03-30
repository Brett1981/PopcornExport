using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PopcornExport.Migrations
{
    public partial class TvImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "Similar",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Similar_ShowId",
                table: "Similar",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Similar_ShowSet_ShowId",
                table: "Similar",
                column: "ShowId",
                principalTable: "ShowSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Similar_ShowSet_ShowId",
                table: "Similar");

            migrationBuilder.DropIndex(
                name: "IX_Similar_ShowId",
                table: "Similar");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "Similar");
        }
    }
}
