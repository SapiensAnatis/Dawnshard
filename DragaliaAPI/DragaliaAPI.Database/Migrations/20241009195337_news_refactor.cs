using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class news_refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyImageAltText",
                table: "NewsItems",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderImageAltText",
                table: "NewsItems",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 20000,
                columns: new[] { "BodyImageAltText", "HeaderImageAltText" },
                values: new object[] { "Mercurial Gauntlet rewards infographic", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyImageAltText",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "HeaderImageAltText",
                table: "NewsItems");
        }
    }
}
