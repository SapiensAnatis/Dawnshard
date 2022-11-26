using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    public partial class AlphaSapiensAnatis1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbQuest",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<byte>(type: "tinyint", nullable: false),
                    IsMissionClear1 = table.Column<bool>(type: "bit", nullable: false),
                    IsMissionClear2 = table.Column<bool>(type: "bit", nullable: false),
                    IsMissionClear3 = table.Column<bool>(type: "bit", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    DailyPlayCount = table.Column<int>(type: "int", nullable: false),
                    WeeklyPlayCount = table.Column<int>(type: "int", nullable: false),
                    LastDailyResetTime = table.Column<int>(type: "int", nullable: false),
                    LastWeeklyResetTime = table.Column<int>(type: "int", nullable: false),
                    IsAppear = table.Column<bool>(type: "bit", nullable: false),
                    BestClearTime = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbQuest", x => new { x.DeviceAccountId, x.QuestId });
                });

            migrationBuilder.CreateTable(
                name: "DeviceAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAccounts", x => x.Id);
                });

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
                name: "PlayerBannerData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SummonBannerId = table.Column<int>(type: "int", nullable: false),
                    Pity = table.Column<byte>(type: "tinyint", nullable: false),
                    SummonCount = table.Column<int>(type: "int", nullable: false),
                    DailyLimitedSummons = table.Column<int>(type: "int", nullable: false),
                    FreeSummonAvailable = table.Column<int>(type: "int", nullable: false),
                    BeginnerSummonAvailable = table.Column<int>(type: "int", nullable: false),
                    CsSummonAvailable = table.Column<int>(type: "int", nullable: false),
                    SummonPoints = table.Column<int>(type: "int", nullable: false),
                    CsSummonPoints = table.Column<int>(type: "int", nullable: false),
                    CsSummonPointsMinDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CsSummonPointsMaxDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBannerData", x => new { x.DeviceAccountId, x.SummonBannerId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharaData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharaId = table.Column<int>(type: "int", nullable: false),
                    Rarity = table.Column<byte>(type: "tinyint", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    AddMaxLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    AtkPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "tinyint", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    Skill1Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    Skill2Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    Abil1Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    Abil2Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    Abil3Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    BurstAtkLvl = table.Column<byte>(type: "tinyint", nullable: false),
                    ComboBuildupCount = table.Column<int>(type: "int", nullable: false),
                    HpBase = table.Column<int>(type: "int", nullable: false),
                    HpNode = table.Column<int>(type: "int", nullable: false),
                    AtkBase = table.Column<int>(type: "int", nullable: false),
                    AtkNode = table.Column<int>(type: "int", nullable: false),
                    ExAbility1Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    ExAbility2Lvl = table.Column<byte>(type: "tinyint", nullable: false),
                    IsTemp = table.Column<bool>(type: "bit", nullable: false),
                    IsUnlockEditSkill = table.Column<bool>(type: "bit", nullable: false),
                    ManaNodeUnlockCount = table.Column<int>(type: "int", nullable: false),
                    ListViewFlag = table.Column<bool>(type: "bit", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharaData", x => new { x.DeviceAccountId, x.CharaId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerCurrency",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrencyType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCurrency", x => new { x.DeviceAccountId, x.CurrencyType });
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonData",
                columns: table => new
                {
                    DragonKeyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DragonId = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "tinyint", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    Skill1Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Abil1Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Abil2Level = table.Column<byte>(type: "tinyint", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonData", x => x.DragonKeyId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonReliability",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DragonId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    TotalExp = table.Column<int>(type: "int", nullable: false),
                    LastContactTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonReliability", x => new { x.DeviceAccountId, x.DragonId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerMaterial",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMaterial", x => new { x.DeviceAccountId, x.MaterialId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerStoryState",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoryType = table.Column<int>(type: "int", nullable: false),
                    StoryId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStoryState", x => new { x.DeviceAccountId, x.StoryType, x.StoryId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerSummonHistory",
                columns: table => new
                {
                    KeyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceAccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannerId = table.Column<int>(type: "int", nullable: false),
                    SummonExecType = table.Column<byte>(type: "tinyint", nullable: false),
                    SummonDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Rarity = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "tinyint", nullable: false),
                    HpPlusCount = table.Column<int>(type: "int", nullable: false),
                    AtkPlusCount = table.Column<int>(type: "int", nullable: false),
                    SummonPrizeRank = table.Column<int>(type: "int", nullable: false),
                    SummonPointGet = table.Column<int>(type: "int", nullable: false),
                    DewPointGet = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSummonHistory", x => x.KeyId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUserData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Crystal = table.Column<int>(type: "int", nullable: false),
                    Coin = table.Column<int>(type: "int", nullable: false),
                    MaxDragonQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxWeaponQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxAmuletQuantity = table.Column<int>(type: "int", nullable: false),
                    QuestSkipPoint = table.Column<int>(type: "int", nullable: false),
                    MainPartyNo = table.Column<int>(type: "int", nullable: false),
                    EmblemId = table.Column<int>(type: "int", nullable: false),
                    ActiveMemoryEventId = table.Column<int>(type: "int", nullable: false),
                    ManaPoint = table.Column<int>(type: "int", nullable: false),
                    DewPoint = table.Column<int>(type: "int", nullable: false),
                    BuildTimePoint = table.Column<int>(type: "int", nullable: false),
                    LastLoginTime = table.Column<int>(type: "int", nullable: false),
                    StaminaSingle = table.Column<int>(type: "int", nullable: false),
                    LastStaminaSingleUpdateTime = table.Column<int>(type: "int", nullable: false),
                    StaminaSingleSurplusSecond = table.Column<int>(type: "int", nullable: false),
                    StaminaMulti = table.Column<int>(type: "int", nullable: false),
                    LastStaminaMultiUpdateTime = table.Column<int>(type: "int", nullable: false),
                    StaminaMultiSurplusSecond = table.Column<int>(type: "int", nullable: false),
                    TutorialStatus = table.Column<int>(type: "int", nullable: false),
                    TutorialFlag = table.Column<int>(type: "int", nullable: false),
                    PrologueEndTime = table.Column<int>(type: "int", nullable: false),
                    IsOptin = table.Column<int>(type: "int", nullable: false),
                    FortOpenTime = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUserData", x => x.DeviceAccountId);
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
                    EquipDragonKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EquipWeaponBodyId = table.Column<int>(type: "int", nullable: false),
                    EquipWeaponSkinId = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "int", nullable: false),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "int", nullable: false),
                    EquipTalismanKeyId = table.Column<long>(type: "bigint", nullable: false),
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
                name: "DbQuest");

            migrationBuilder.DropTable(
                name: "DeviceAccounts");

            migrationBuilder.DropTable(
                name: "PlayerBannerData");

            migrationBuilder.DropTable(
                name: "PlayerCharaData");

            migrationBuilder.DropTable(
                name: "PlayerCurrency");

            migrationBuilder.DropTable(
                name: "PlayerDragonData");

            migrationBuilder.DropTable(
                name: "PlayerDragonReliability");

            migrationBuilder.DropTable(
                name: "PlayerMaterial");

            migrationBuilder.DropTable(
                name: "PlayerPartyUnits");

            migrationBuilder.DropTable(
                name: "PlayerStoryState");

            migrationBuilder.DropTable(
                name: "PlayerSummonHistory");

            migrationBuilder.DropTable(
                name: "PlayerUserData");

            migrationBuilder.DropTable(
                name: "PartyData");
        }
    }
}
