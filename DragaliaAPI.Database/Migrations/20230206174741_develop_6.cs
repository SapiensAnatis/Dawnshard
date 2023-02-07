using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class develop6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddMaxLevel",
                table: "PlayerCharaData");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "PlayerStoryState",
                newName: "StoryState");

            migrationBuilder.CreateTable(
                name: "PlayerDragonGift",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    DragonGiftId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OwnerAccountId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonGift", x => new { x.DeviceAccountId, x.DragonGiftId });
                    table.ForeignKey(
                        name: "FK_PlayerDragonGift_Players_OwnerAccountId",
                        column: x => x.OwnerAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_OwnerAccountId",
                table: "PlayerDragonGift",
                column: "OwnerAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerDragonGift");

            migrationBuilder.RenameColumn(
                name: "StoryState",
                table: "PlayerStoryState",
                newName: "State");

            migrationBuilder.AddColumn<byte>(
                name: "AddMaxLevel",
                table: "PlayerCharaData",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
