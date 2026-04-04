using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGatherItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerGatherItem",
                columns: table => new
                {
                    ViewerId = table.Column<long>(type: "bigint", nullable: false),
                    GatherItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    QuestTakeWeeklyQuantity = table.Column<int>(type: "integer", nullable: false),
                    QuestLastWeeklyResetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGatherItem", x => new { x.ViewerId, x.GatherItemId });
                    table.ForeignKey(
                        name: "FK_PlayerGatherItem_Players_ViewerId",
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
                name: "PlayerGatherItem");
        }
    }
}
