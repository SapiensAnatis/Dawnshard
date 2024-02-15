using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class questevent1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastWeeklyResetTime", table: "PlayerQuests");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastWeeklyResetTime",
                table: "PlayerQuests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTimeOffset.UnixEpoch
            );

            migrationBuilder.DropColumn(name: "LastDailyResetTime", table: "PlayerQuests");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastDailyResetTime",
                table: "PlayerQuests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTimeOffset.UnixEpoch
            );

            migrationBuilder.CreateTable(
                name: "QuestEvents",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    QuestEventId = table.Column<int>(type: "integer", nullable: false),
                    DailyPlayCount = table.Column<int>(type: "integer", nullable: false),
                    LastDailyResetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    WeeklyPlayCount = table.Column<int>(type: "integer", nullable: false),
                    LastWeeklyResetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    QuestBonusReceiveCount = table.Column<int>(type: "integer", nullable: false),
                    QuestBonusStackCount = table.Column<int>(type: "integer", nullable: false),
                    QuestBonusStackTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    QuestBonusReserveCount = table.Column<int>(type: "integer", nullable: false),
                    QuestBonusReserveTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestEvents", x => new { x.DeviceAccountId, x.QuestEventId });
                    table.ForeignKey(
                        name: "FK_QuestEvents_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestEvents_DeviceAccountId",
                table: "QuestEvents",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestEvents");

            migrationBuilder.DropColumn(name: "LastWeeklyResetTime", table: "PlayerQuests");

            migrationBuilder.AddColumn<int>(
                name: "LastWeeklyResetTime",
                table: "PlayerQuests",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.DropColumn(name: "LastDailyResetTime", table: "PlayerQuests");

            migrationBuilder.AddColumn<int>(
                name: "LastDailyResetTime",
                table: "PlayerQuests",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );
        }
    }
}
