using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class HelperRethink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastHelperUseDate",
                table: "PlayerFriendshipPlayers");

            migrationBuilder.CreateTable(
                name: "PlayerHelperUseDates",
                columns: table => new
                {
                    HelperViewerId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerViewerId = table.Column<long>(type: "bigint", nullable: false),
                    UseDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHelperUseDates", x => new { x.HelperViewerId, x.PlayerViewerId });
                    table.ForeignKey(
                        name: "FK_PlayerHelperUseDates_PlayerHelpers_HelperViewerId",
                        column: x => x.HelperViewerId,
                        principalTable: "PlayerHelpers",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelperUseDates_Players_PlayerViewerId",
                        column: x => x.PlayerViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelperUseDates_PlayerViewerId",
                table: "PlayerHelperUseDates",
                column: "PlayerViewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerHelperUseDates");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastHelperUseDate",
                table: "PlayerFriendshipPlayers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
