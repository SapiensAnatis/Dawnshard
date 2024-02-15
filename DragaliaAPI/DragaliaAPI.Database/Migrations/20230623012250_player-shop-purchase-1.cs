using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class playershoppurchase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerPurchases",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    ShopType = table.Column<int>(type: "integer", nullable: false),
                    BuyCount = table.Column<int>(type: "integer", nullable: false),
                    LastBuyTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectStartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectEndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPurchases", x => new { x.DeviceAccountId, x.GoodsId });
                    table.ForeignKey(
                        name: "FK_PlayerPurchases_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPurchases_DeviceAccountId",
                table: "PlayerPurchases",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPurchases");
        }
    }
}
