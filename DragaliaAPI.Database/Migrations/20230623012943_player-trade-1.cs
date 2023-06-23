using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class playertrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerTrades",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    TradeId = table.Column<int>(type: "integer", nullable: false),
                    TradeType = table.Column<int>(type: "integer", nullable: false),
                    TradeCount = table.Column<int>(type: "integer", nullable: false),
                    LastTrade = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrades", x => new { x.DeviceAccountId, x.TradeId });
                    table.ForeignKey(
                        name: "FK_PlayerTrades_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrades_DeviceAccountId",
                table: "PlayerTrades",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrades_DeviceAccountId_TradeType",
                table: "PlayerTrades",
                columns: new[] { "DeviceAccountId", "TradeType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerTrades");
        }
    }
}
