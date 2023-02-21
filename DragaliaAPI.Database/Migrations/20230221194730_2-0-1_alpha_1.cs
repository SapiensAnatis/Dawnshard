using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class _201alpha1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddMaxLevel",
                table: "PlayerCharaData");

            migrationBuilder.CreateTable(
                name: "PlayerDragonGift",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    DragonGiftId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonGift", x => new { x.DeviceAccountId, x.DragonGiftId });
                    table.ForeignKey(
                        name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerDragonGift");

            migrationBuilder.AddColumn<byte>(
                name: "AddMaxLevel",
                table: "PlayerCharaData",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
