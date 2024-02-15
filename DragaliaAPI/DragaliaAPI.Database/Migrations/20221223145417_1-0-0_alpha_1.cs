using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class _100alpha1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartyData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PartyNo = table.Column<int>(type: "integer", nullable: false),
                    PartyName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyData", x => new { x.DeviceAccountId, x.PartyNo });
                });

            migrationBuilder.CreateTable(
                name: "PlayerAbilityCrests",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    AbilityCrestId = table.Column<int>(type: "integer", nullable: false),
                    BuildupCount = table.Column<int>(type: "integer", nullable: false),
                    LimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    EquipableCount = table.Column<int>(type: "integer", nullable: false),
                    HpPlusCount = table.Column<int>(type: "integer", nullable: false),
                    AttackPlusCount = table.Column<int>(type: "integer", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAbilityCrests", x => new { x.DeviceAccountId, x.AbilityCrestId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerBannerData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    SummonBannerId = table.Column<int>(type: "integer", nullable: false),
                    Pity = table.Column<byte>(type: "smallint", nullable: false),
                    SummonCount = table.Column<int>(type: "integer", nullable: false),
                    DailyLimitedSummons = table.Column<int>(type: "integer", nullable: false),
                    FreeSummonAvailable = table.Column<int>(type: "integer", nullable: false),
                    BeginnerSummonAvailable = table.Column<int>(type: "integer", nullable: false),
                    CsSummonAvailable = table.Column<int>(type: "integer", nullable: false),
                    SummonPoints = table.Column<int>(type: "integer", nullable: false),
                    CsSummonPoints = table.Column<int>(type: "integer", nullable: false),
                    CsSummonPointsMinDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CsSummonPointsMaxDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBannerData", x => new { x.DeviceAccountId, x.SummonBannerId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharaData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<byte>(type: "smallint", nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    AddMaxLevel = table.Column<byte>(type: "smallint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "smallint", nullable: false),
                    AtkPlusCount = table.Column<byte>(type: "smallint", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    Skill1Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    Skill2Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    Abil1Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    Abil2Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    Abil3Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    BurstAtkLvl = table.Column<byte>(type: "smallint", nullable: false),
                    ComboBuildupCount = table.Column<int>(type: "integer", nullable: false),
                    HpBase = table.Column<int>(type: "integer", nullable: false),
                    HpNode = table.Column<int>(type: "integer", nullable: false),
                    AtkBase = table.Column<int>(type: "integer", nullable: false),
                    AtkNode = table.Column<int>(type: "integer", nullable: false),
                    ExAbility1Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    ExAbility2Lvl = table.Column<byte>(type: "smallint", nullable: false),
                    IsTemp = table.Column<bool>(type: "boolean", nullable: false),
                    IsUnlockEditSkill = table.Column<bool>(type: "boolean", nullable: false),
                    ManaNodeUnlockCount = table.Column<int>(type: "integer", nullable: false),
                    ListViewFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharaData", x => new { x.DeviceAccountId, x.CharaId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerCurrency",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CurrencyType = table.Column<int>(type: "integer", nullable: false),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    DragonId = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "smallint", nullable: false),
                    AttackPlusCount = table.Column<byte>(type: "smallint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "smallint", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    Skill1Level = table.Column<byte>(type: "smallint", nullable: false),
                    Abil1Level = table.Column<byte>(type: "smallint", nullable: false),
                    Abil2Level = table.Column<byte>(type: "smallint", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonData", x => x.DragonKeyId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonReliability",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    DragonId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    TotalExp = table.Column<int>(type: "integer", nullable: false),
                    LastContactTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonReliability", x => new { x.DeviceAccountId, x.DragonId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerMaterial",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMaterial", x => new { x.DeviceAccountId, x.MaterialId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerSetUnit",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    UnitSetNo = table.Column<int>(type: "integer", nullable: false),
                    UnitSetName = table.Column<string>(type: "text", nullable: false),
                    EquipDragonKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EquipWeaponBodyId = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipTalismanKeyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSetUnit", x => new { x.DeviceAccountId, x.CharaId, x.UnitSetNo });
                });

            migrationBuilder.CreateTable(
                name: "PlayerStoryState",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    StoryType = table.Column<int>(type: "integer", nullable: false),
                    StoryId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<byte>(type: "smallint", nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    BannerId = table.Column<int>(type: "integer", nullable: false),
                    SummonExecType = table.Column<byte>(type: "smallint", nullable: false),
                    SummonDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    Rarity = table.Column<byte>(type: "smallint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "smallint", nullable: false),
                    HpPlusCount = table.Column<int>(type: "integer", nullable: false),
                    AtkPlusCount = table.Column<int>(type: "integer", nullable: false),
                    SummonPrizeRank = table.Column<int>(type: "integer", nullable: false),
                    SummonPointGet = table.Column<int>(type: "integer", nullable: false),
                    DewPointGet = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSummonHistory", x => x.KeyId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUserData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: false),
                    Crystal = table.Column<int>(type: "integer", nullable: false),
                    Coin = table.Column<int>(type: "integer", nullable: false),
                    MaxDragonQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxWeaponQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxAmuletQuantity = table.Column<int>(type: "integer", nullable: false),
                    QuestSkipPoint = table.Column<int>(type: "integer", nullable: false),
                    MainPartyNo = table.Column<int>(type: "integer", nullable: false),
                    EmblemId = table.Column<int>(type: "integer", nullable: false),
                    ActiveMemoryEventId = table.Column<int>(type: "integer", nullable: false),
                    ManaPoint = table.Column<int>(type: "integer", nullable: false),
                    DewPoint = table.Column<int>(type: "integer", nullable: false),
                    BuildTimePoint = table.Column<int>(type: "integer", nullable: false),
                    LastLoginTime = table.Column<int>(type: "integer", nullable: false),
                    StaminaSingle = table.Column<int>(type: "integer", nullable: false),
                    LastStaminaSingleUpdateTime = table.Column<int>(type: "integer", nullable: false),
                    StaminaSingleSurplusSecond = table.Column<int>(type: "integer", nullable: false),
                    StaminaMulti = table.Column<int>(type: "integer", nullable: false),
                    LastStaminaMultiUpdateTime = table.Column<int>(type: "integer", nullable: false),
                    StaminaMultiSurplusSecond = table.Column<int>(type: "integer", nullable: false),
                    TutorialStatus = table.Column<int>(type: "integer", nullable: false),
                    TutorialFlag = table.Column<int>(type: "integer", nullable: false),
                    PrologueEndTime = table.Column<int>(type: "integer", nullable: false),
                    IsOptin = table.Column<int>(type: "integer", nullable: false),
                    FortOpenTime = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUserData", x => x.DeviceAccountId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerWeapons",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    WeaponBodyId = table.Column<int>(type: "integer", nullable: false),
                    BuildupCount = table.Column<int>(type: "integer", nullable: false),
                    LimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    LimitOverCount = table.Column<int>(type: "integer", nullable: false),
                    EquipableCount = table.Column<int>(type: "integer", nullable: false),
                    AdditionalCrestSlotType1Count = table.Column<int>(type: "integer", nullable: false),
                    AdditionalCrestSlotType2Count = table.Column<int>(type: "integer", nullable: false),
                    AdditionalCrestSlotType3Count = table.Column<int>(type: "integer", nullable: false),
                    UnlockWeaponPassiveAbilityNoString = table.Column<string>(type: "text", nullable: false),
                    FortPassiveCharaWeaponBuildupCount = table.Column<int>(type: "integer", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerWeapons", x => new { x.DeviceAccountId, x.WeaponBodyId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerQuests",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<byte>(type: "smallint", nullable: false),
                    IsMissionClear1 = table.Column<bool>(type: "boolean", nullable: false),
                    IsMissionClear2 = table.Column<bool>(type: "boolean", nullable: false),
                    IsMissionClear3 = table.Column<bool>(type: "boolean", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false),
                    DailyPlayCount = table.Column<int>(type: "integer", nullable: false),
                    WeeklyPlayCount = table.Column<int>(type: "integer", nullable: false),
                    LastDailyResetTime = table.Column<int>(type: "integer", nullable: false),
                    LastWeeklyResetTime = table.Column<int>(type: "integer", nullable: false),
                    IsAppear = table.Column<bool>(type: "boolean", nullable: false),
                    BestClearTime = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerQuests", x => new { x.DeviceAccountId, x.QuestId });
                    table.ForeignKey(
                        name: "FK_PlayerQuests_DeviceAccounts_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "DeviceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTalismans",
                columns: table => new
                {
                    TalismanKeyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    TalismanId = table.Column<int>(type: "integer", nullable: false),
                    TalismanAbilityId1 = table.Column<int>(type: "integer", nullable: false),
                    TalismanAbilityId2 = table.Column<int>(type: "integer", nullable: false),
                    TalismanAbilityId3 = table.Column<int>(type: "integer", nullable: false),
                    AdditionalHp = table.Column<int>(type: "integer", nullable: false),
                    AdditionalAttack = table.Column<int>(type: "integer", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    IsLock = table.Column<bool>(type: "boolean", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTalismans", x => x.TalismanKeyId);
                    table.ForeignKey(
                        name: "FK_PlayerTalismans_DeviceAccounts_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "DeviceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPartyUnits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PartyNo = table.Column<int>(type: "integer", nullable: false),
                    UnitNo = table.Column<int>(type: "integer", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    EquipDragonKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EquipWeaponBodyId = table.Column<int>(type: "integer", nullable: false),
                    EquipWeaponSkinId = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "integer", nullable: false),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "integer", nullable: false),
                    EquipTalismanKeyId = table.Column<long>(type: "bigint", nullable: false),
                    EditSkill1CharaId = table.Column<int>(type: "integer", nullable: false),
                    EditSkill2CharaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPartyUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPartyUnits_PartyData_DeviceAccountId_PartyNo",
                        columns: x => new { x.DeviceAccountId, x.PartyNo },
                        principalTable: "PartyData",
                        principalColumns: new[] { "DeviceAccountId", "PartyNo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "DeviceAccountId", "PartyNo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAbilityCrests");

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
                name: "PlayerQuests");

            migrationBuilder.DropTable(
                name: "PlayerSetUnit");

            migrationBuilder.DropTable(
                name: "PlayerStoryState");

            migrationBuilder.DropTable(
                name: "PlayerSummonHistory");

            migrationBuilder.DropTable(
                name: "PlayerTalismans");

            migrationBuilder.DropTable(
                name: "PlayerUserData");

            migrationBuilder.DropTable(
                name: "PlayerWeapons");

            migrationBuilder.DropTable(
                name: "PartyData");

            migrationBuilder.DropTable(
                name: "DeviceAccounts");
        }
    }
}
