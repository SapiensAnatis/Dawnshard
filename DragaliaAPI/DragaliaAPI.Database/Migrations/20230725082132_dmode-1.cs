using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class dmode1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerDmodeCharas",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    MaxFloor = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    SelectedServitorId = table.Column<int>(type: "integer", nullable: false),
                    SelectEditSkillCharaId1 = table.Column<int>(type: "integer", nullable: false),
                    SelectEditSkillCharaId2 = table.Column<int>(type: "integer", nullable: false),
                    SelectEditSkillCharaId3 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDmodeCharas", x => new { x.DeviceAccountId, x.CharaId });
                    table.ForeignKey(
                        name: "FK_PlayerDmodeCharas_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDmodeDungeons",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    QuestTime = table.Column<int>(type: "integer", nullable: false),
                    DungeonScore = table.Column<int>(type: "integer", nullable: false),
                    IsPlayEnd = table.Column<bool>(type: "boolean", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDmodeDungeons", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerDmodeDungeons_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDmodeInfos",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    RecoveryCount = table.Column<int>(type: "integer", nullable: false),
                    RecoveryTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FloorSkipCount = table.Column<int>(type: "integer", nullable: false),
                    FloorSkipTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Point1Quantity = table.Column<int>(type: "integer", nullable: false),
                    Point2Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDmodeInfos", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerDmodeInfos_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDmodeCharas_DeviceAccountId",
                table: "PlayerDmodeCharas",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerDmodeCharas");

            migrationBuilder.DropTable(
                name: "PlayerDmodeDungeons");

            migrationBuilder.DropTable(
                name: "PlayerDmodeInfos");
        }
    }
}
