using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class news_items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "NewsItems",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "BodyImagePath",
                table: "NewsItems",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderImagePath",
                table: "NewsItems",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "NewsItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "NewsItems",
                columns: new[] { "Id", "BodyImagePath", "Date", "Description", "HeaderImagePath", "Headline", "Hidden" },
                values: new object[] { 20000, "/dawnshard/news/mg-endeavours.webp", new DateTimeOffset(new DateTime(2024, 6, 2, 16, 7, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "The below infographic shows the endeavour rewards available for the progressing the Mercurial Gauntlet.", null, "Mercurial Gauntlet Endeavour Rewards", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 20000);

            migrationBuilder.DropColumn(
                name: "BodyImagePath",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "HeaderImagePath",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "NewsItems");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "NewsItems",
                newName: "Time");
        }
    }
}
