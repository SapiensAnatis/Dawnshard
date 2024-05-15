using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class wall_rewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WallRewardDates",
                columns: table => new
                {
                    ViewerId = table.Column<long>(type: "bigint", nullable: false),
                    LastClaimDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallRewardDates", x => x.ViewerId);
                    table.ForeignKey(
                        name: "FK_WallRewardDates_Players_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WallRewardDates");
        }
    }
}
