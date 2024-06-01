using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class summon_pity_rates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pity",
                table: "PlayerBannerData");

            migrationBuilder.AddColumn<int>(
                name: "SummonCountSinceLastFiveStar",
                table: "PlayerBannerData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummonCountSinceLastFiveStar",
                table: "PlayerBannerData");

            migrationBuilder.AddColumn<byte>(
                name: "Pity",
                table: "PlayerBannerData",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
