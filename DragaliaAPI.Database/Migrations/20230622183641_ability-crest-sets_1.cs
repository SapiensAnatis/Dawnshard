using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class abilitycrestsets_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerAbilityCrestSets",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    AbilityCrestSetNo = table.Column<int>(type: "integer", nullable: false),
                    AbilityCrestSetName = table.Column<string>(type: "text", nullable: false),
                    CrestSlotType1CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType1CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType1CrestId3 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType2CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType2CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType3CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    CrestSlotType3CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    TalismanKeyId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAbilityCrestSets", x => new { x.DeviceAccountId, x.AbilityCrestSetNo });
                    table.ForeignKey(
                        name: "FK_PlayerAbilityCrestSets_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAbilityCrestSets_DeviceAccountId",
                table: "PlayerAbilityCrestSets",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAbilityCrestSets");
        }
    }
}
