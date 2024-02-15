using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class time_attack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceivedRankingTierRewards",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    RewardId = table.Column<int>(type: "integer", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivedRankingTierRewards", x => new { x.DeviceAccountId, x.RewardId });
                    table.ForeignKey(
                        name: "FK_ReceivedRankingTierRewards_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAttackClears",
                columns: table => new
                {
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAttackClears", x => x.RoomName);
                });

            migrationBuilder.CreateTable(
                name: "TimeAttackPlayers",
                columns: table => new
                {
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PartyInfo = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAttackPlayers", x => new { x.RoomName, x.DeviceAccountId });
                    table.ForeignKey(
                        name: "FK_TimeAttackPlayers_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeAttackPlayers_TimeAttackClears_RoomName",
                        column: x => x.RoomName,
                        principalTable: "TimeAttackClears",
                        principalColumn: "RoomName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAttackClearUnits",
                columns: table => new
                {
                    UnitNo = table.Column<int>(type: "integer", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    EquippedDragonEntityId = table.Column<int>(type: "integer", nullable: false),
                    EquippedTalismanEntityId = table.Column<int>(type: "integer", nullable: false),
                    TalismanAbility1 = table.Column<int>(type: "integer", nullable: false),
                    TalismanAbility2 = table.Column<int>(type: "integer", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    EquipDragonKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EquipWeaponBodyId = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipTalismanKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EquipWeaponSkinId = table.Column<int>(type: "integer", nullable: false),
                    EditSkill1CharaId = table.Column<int>(type: "integer", nullable: false),
                    EditSkill2CharaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAttackClearUnits", x => new { x.RoomName, x.DeviceAccountId, x.UnitNo });
                    table.ForeignKey(
                        name: "FK_TimeAttackClearUnits_TimeAttackPlayers_RoomName_DeviceAccou~",
                        columns: x => new { x.RoomName, x.DeviceAccountId },
                        principalTable: "TimeAttackPlayers",
                        principalColumns: new[] { "RoomName", "DeviceAccountId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedRankingTierRewards_DeviceAccountId",
                table: "ReceivedRankingTierRewards",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedRankingTierRewards_QuestId",
                table: "ReceivedRankingTierRewards",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAttackClears_QuestId",
                table: "TimeAttackClears",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAttackPlayers_DeviceAccountId",
                table: "TimeAttackPlayers",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceivedRankingTierRewards");

            migrationBuilder.DropTable(
                name: "TimeAttackClearUnits");

            migrationBuilder.DropTable(
                name: "TimeAttackPlayers");

            migrationBuilder.DropTable(
                name: "TimeAttackClears");
        }
    }
}
