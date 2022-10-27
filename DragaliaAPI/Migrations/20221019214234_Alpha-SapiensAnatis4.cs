using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class AlphaSapiensAnatis4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PlayerUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id");

            migrationBuilder.CreateTable(
                name: "PartyData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartyNo = table.Column<int>(type: "int", nullable: false),
                    PartyName = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyData", x => new { x.DeviceAccountId, x.PartyNo });
                });

            migrationBuilder.CreateTable(
                name: "PlayerPartyUnits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartyDeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartyNo = table.Column<int>(type: "int", nullable: false),
                    UnitNo = table.Column<int>(type: "int", nullable: false),
                    CharaId = table.Column<int>(type: "int", nullable: false),
                    EquipDragonKeyId = table.Column<int>(type: "int", nullable: false),
                    EquipWeaponBodyId = table.Column<int>(type: "int", nullable: false),
                    EquipWeaponSkinId = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipTalismanKeyId = table.Column<int>(type: "int", nullable: false),
                    EditSkill1CharaId = table.Column<int>(type: "int", nullable: false),
                    EditSkill2CharaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPartyUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPartyUnits_PartyData_PartyDeviceAccountId_PartyNo",
                        columns: x => new { x.PartyDeviceAccountId, x.PartyNo },
                        principalTable: "PartyData",
                        principalColumns: new[] { "DeviceAccountId", "PartyNo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_PartyDeviceAccountId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "PartyDeviceAccountId", "PartyNo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPartyUnits");

            migrationBuilder.DropTable(
                name: "PartyData");

            migrationBuilder.InsertData(
                table: "PlayerUserData",
                columns: new[] { "DeviceAccountId", "ActiveMemoryEventId", "BuildTimePoint", "Coin", "CreateTime", "Crystal", "DewPoint", "EmblemId", "Exp", "FortOpenTime", "IsOptin", "LastLoginTime", "LastStaminaMultiUpdateTime", "LastStaminaSingleUpdateTime", "Level", "MainPartyNo", "ManaPoint", "MaxAmuletQuantity", "MaxDragonQuantity", "MaxWeaponQuantity", "Name", "PrologueEndTime", "QuestSkipPoint", "StaminaMulti", "StaminaMultiSurplusSecond", "StaminaSingle", "StaminaSingleSurplusSecond", "TutorialFlag", "TutorialStatus", "ViewerId" },
                values: new object[] { "id", 0, 0, 0, 1666029088, 0, 0, 40000001, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 160, 0, "Euden", 0, 0, 12, 0, 18, 0, 0, 0, 0L });
        }
    }
}
