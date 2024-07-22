using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class event_summon_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerEventSummonData",
                columns: table => new
                {
                    ViewerId = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    BoxSummonCount = table.Column<int>(type: "integer", nullable: false),
                    BoxNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerEventSummonData", x => new { x.ViewerId, x.EventId });
                    table.ForeignKey(
                        name: "FK_PlayerEventSummonData_Players_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerEventSummonItems",
                columns: table => new
                {
                    ViewerId = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TimesSummoned = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerEventSummonItems", x => new { x.ViewerId, x.EventId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_PlayerEventSummonItems_PlayerEventSummonData_ViewerId_Event~",
                        columns: x => new { x.ViewerId, x.EventId },
                        principalTable: "PlayerEventSummonData",
                        principalColumns: new[] { "ViewerId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerEventSummonItems");

            migrationBuilder.DropTable(
                name: "PlayerEventSummonData");
        }
    }
}
