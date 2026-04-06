using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class GatherItemRestrictedDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGatherItem_Players_ViewerId",
                table: "PlayerGatherItem");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGatherItem_Players_ViewerId",
                table: "PlayerGatherItem",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGatherItem_Players_ViewerId",
                table: "PlayerGatherItem");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGatherItem_Players_ViewerId",
                table: "PlayerGatherItem",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
