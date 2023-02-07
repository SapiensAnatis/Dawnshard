using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class develop7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonGift_Players_OwnerAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonGift_OwnerAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.DropColumn(
                name: "OwnerAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.RenameColumn(
                name: "StoryState",
                table: "PlayerStoryState",
                newName: "State");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift",
                column: "DeviceAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                table: "PlayerDragonGift",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "PlayerStoryState",
                newName: "StoryState");

            migrationBuilder.AddColumn<string>(
                name: "OwnerAccountId",
                table: "PlayerDragonGift",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_OwnerAccountId",
                table: "PlayerDragonGift",
                column: "OwnerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonGift_Players_OwnerAccountId",
                table: "PlayerDragonGift",
                column: "OwnerAccountId",
                principalTable: "Players",
                principalColumn: "AccountId");
        }
    }
}
