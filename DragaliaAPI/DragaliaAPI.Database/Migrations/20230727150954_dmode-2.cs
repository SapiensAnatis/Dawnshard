using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class dmode2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerDmodeExpeditions",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CharaId1 = table.Column<int>(type: "integer", nullable: false),
                    CharaId2 = table.Column<int>(type: "integer", nullable: false),
                    CharaId3 = table.Column<int>(type: "integer", nullable: false),
                    CharaId4 = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TargetFloor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDmodeExpeditions", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerDmodeExpeditions_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDmodeServitorPassives",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PassiveId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDmodeServitorPassives", x => new { x.DeviceAccountId, x.PassiveId });
                    table.ForeignKey(
                        name: "FK_PlayerDmodeServitorPassives_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDmodeServitorPassives_DeviceAccountId",
                table: "PlayerDmodeServitorPassives",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerDmodeExpeditions");

            migrationBuilder.DropTable(
                name: "PlayerDmodeServitorPassives");
        }
    }
}
