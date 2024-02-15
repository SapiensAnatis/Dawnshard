using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class events1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerEventData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    CustomEventFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerEventData", x => new { x.DeviceAccountId, x.EventId });
                    table.ForeignKey(
                        name: "FK_PlayerEventData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerEventItems",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerEventItems", x => new { x.DeviceAccountId, x.Id });
                    table.ForeignKey(
                        name: "FK_PlayerEventItems_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerEventRewards",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    RewardId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerEventRewards", x => new { x.DeviceAccountId, x.EventId, x.RewardId });
                    table.ForeignKey(
                        name: "FK_PlayerEventRewards_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventData_DeviceAccountId",
                table: "PlayerEventData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventItems_DeviceAccountId_EventId",
                table: "PlayerEventItems",
                columns: new[] { "DeviceAccountId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventRewards_DeviceAccountId_EventId",
                table: "PlayerEventRewards",
                columns: new[] { "DeviceAccountId", "EventId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerEventData");

            migrationBuilder.DropTable(
                name: "PlayerEventItems");

            migrationBuilder.DropTable(
                name: "PlayerEventRewards");
        }
    }
}
