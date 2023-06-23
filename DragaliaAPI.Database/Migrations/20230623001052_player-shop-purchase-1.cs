using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class playershoppurchase1 : Migration
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
                name: "Players",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    SavefileVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "EquippedStamps",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    StampId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquippedStamps", x => new { x.DeviceAccountId, x.Slot });
                    table.ForeignKey(
                        name: "FK_EquippedStamps_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PartyNo = table.Column<int>(type: "integer", nullable: false),
                    PartyName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyData", x => new { x.DeviceAccountId, x.PartyNo });
                    table.ForeignKey(
                        name: "FK_PartyData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerAbilityCrests_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    table.ForeignKey(
                        name: "FK_PlayerBannerData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerCharaData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerCurrency_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerDragonData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonGift",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    DragonGiftId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonGift", x => new { x.DeviceAccountId, x.DragonGiftId });
                    table.ForeignKey(
                        name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerDragonReliability_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerFortBuilds",
                columns: table => new
                {
                    BuildId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PlantId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    PositionX = table.Column<int>(type: "integer", nullable: false),
                    PositionZ = table.Column<int>(type: "integer", nullable: false),
                    BuildStartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BuildEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    LastIncomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFortBuilds", x => x.BuildId);
                    table.ForeignKey(
                        name: "FK_PlayerFortBuilds_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerFortDetail",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CarpenterNum = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFortDetail", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerFortDetail_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerMaterial_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMissions",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    MissionId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Pickup = table.Column<bool>(type: "boolean", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMissions", x => new { x.DeviceAccountId, x.MissionId, x.Type });
                    table.ForeignKey(
                        name: "FK_PlayerMissions_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPassiveAbilities",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    WeaponPassiveAbilityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPassiveAbilities", x => new { x.DeviceAccountId, x.WeaponPassiveAbilityId });
                    table.ForeignKey(
                        name: "FK_PlayerPassiveAbilities_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPresent",
                columns: table => new
                {
                    PresentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityQuantity = table.Column<int>(type: "integer", nullable: false),
                    EntityLevel = table.Column<int>(type: "integer", nullable: false),
                    EntityLimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    EntityStatusPlusCount = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue1 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue2 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue3 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue4 = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReceiveLimitTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPresent", x => x.PresentId);
                    table.ForeignKey(
                        name: "FK_PlayerPresent_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPresentHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityQuantity = table.Column<int>(type: "integer", nullable: false),
                    EntityLevel = table.Column<int>(type: "integer", nullable: false),
                    EntityLimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    EntityStatusPlusCount = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue1 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue2 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue3 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue4 = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPresentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPresentHistory_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPurchases",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    ShopType = table.Column<int>(type: "integer", nullable: false),
                    BuyCount = table.Column<int>(type: "integer", nullable: false),
                    LastBuyTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectStartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectEndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPurchases", x => new { x.DeviceAccountId, x.GoodsId });
                    table.ForeignKey(
                        name: "FK_PlayerPurchases_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_PlayerQuests_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerSetUnit_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerShopInfos",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    LastSummonTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DailySummonCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerShopInfos", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerShopInfos_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStoryState",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    StoryType = table.Column<int>(type: "integer", nullable: false),
                    StoryId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStoryState", x => new { x.DeviceAccountId, x.StoryType, x.StoryId });
                    table.ForeignKey(
                        name: "FK_PlayerStoryState_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerSummonHistory_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
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
                        name: "FK_PlayerTalismans_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUserData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: false),
                    Crystal = table.Column<int>(type: "integer", nullable: false),
                    Coin = table.Column<long>(type: "bigint", nullable: false),
                    MaxDragonQuantity = table.Column<int>(type: "integer", nullable: false),
                    QuestSkipPoint = table.Column<int>(type: "integer", nullable: false),
                    MainPartyNo = table.Column<int>(type: "integer", nullable: false),
                    EmblemId = table.Column<int>(type: "integer", nullable: false),
                    ActiveMemoryEventId = table.Column<int>(type: "integer", nullable: false),
                    ManaPoint = table.Column<int>(type: "integer", nullable: false),
                    DewPoint = table.Column<int>(type: "integer", nullable: false),
                    BuildTimePoint = table.Column<int>(type: "integer", nullable: false),
                    LastLoginTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StaminaSingle = table.Column<int>(type: "integer", nullable: false),
                    LastStaminaSingleUpdateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StaminaSingleSurplusSecond = table.Column<int>(type: "integer", nullable: false),
                    StaminaMulti = table.Column<int>(type: "integer", nullable: false),
                    LastStaminaMultiUpdateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StaminaMultiSurplusSecond = table.Column<int>(type: "integer", nullable: false),
                    TutorialStatus = table.Column<int>(type: "integer", nullable: false),
                    TutorialFlag = table.Column<int>(type: "integer", nullable: false),
                    FortOpenTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSaveImportTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUserData", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_PlayerUserData_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PlayerWeapons_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerWeaponSkins",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    WeaponSkinId = table.Column<int>(type: "integer", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    GetTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerWeaponSkins", x => new { x.DeviceAccountId, x.WeaponSkinId });
                    table.ForeignKey(
                        name: "FK_PlayerWeaponSkins_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
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
                name: "IX_EquippedStamps_DeviceAccountId",
                table: "EquippedStamps",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyData_DeviceAccountId",
                table: "PartyData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAbilityCrests_DeviceAccountId",
                table: "PlayerAbilityCrests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAbilityCrestSets_DeviceAccountId",
                table: "PlayerAbilityCrestSets",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBannerData_DeviceAccountId",
                table: "PlayerBannerData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharaData_DeviceAccountId",
                table: "PlayerCharaData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCurrency_DeviceAccountId",
                table: "PlayerCurrency",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonData_DeviceAccountId",
                table: "PlayerDragonData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonReliability_DeviceAccountId",
                table: "PlayerDragonReliability",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortBuilds_DeviceAccountId",
                table: "PlayerFortBuilds",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortDetail_DeviceAccountId",
                table: "PlayerFortDetail",
                column: "DeviceAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMaterial_DeviceAccountId",
                table: "PlayerMaterial",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId",
                table: "PlayerPartyUnits",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "DeviceAccountId", "PartyNo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPassiveAbilities_DeviceAccountId",
                table: "PlayerPassiveAbilities",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresent_DeviceAccountId",
                table: "PlayerPresent",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresentHistory_DeviceAccountId",
                table: "PlayerPresentHistory",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPurchases_DeviceAccountId",
                table: "PlayerPurchases",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_DeviceAccountId",
                table: "PlayerQuests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSetUnit_DeviceAccountId",
                table: "PlayerSetUnit",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStoryState_DeviceAccountId",
                table: "PlayerStoryState",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSummonHistory_DeviceAccountId",
                table: "PlayerSummonHistory",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUserData_DeviceAccountId",
                table: "PlayerUserData",
                column: "DeviceAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerWeapons_DeviceAccountId",
                table: "PlayerWeapons",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerWeaponSkins_DeviceAccountId",
                table: "PlayerWeaponSkins",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceAccounts");

            migrationBuilder.DropTable(
                name: "EquippedStamps");

            migrationBuilder.DropTable(
                name: "PlayerAbilityCrests");

            migrationBuilder.DropTable(
                name: "PlayerAbilityCrestSets");

            migrationBuilder.DropTable(
                name: "PlayerBannerData");

            migrationBuilder.DropTable(
                name: "PlayerCharaData");

            migrationBuilder.DropTable(
                name: "PlayerCurrency");

            migrationBuilder.DropTable(
                name: "PlayerDragonData");

            migrationBuilder.DropTable(
                name: "PlayerDragonGift");

            migrationBuilder.DropTable(
                name: "PlayerDragonReliability");

            migrationBuilder.DropTable(
                name: "PlayerFortBuilds");

            migrationBuilder.DropTable(
                name: "PlayerFortDetail");

            migrationBuilder.DropTable(
                name: "PlayerMaterial");

            migrationBuilder.DropTable(
                name: "PlayerMissions");

            migrationBuilder.DropTable(
                name: "PlayerPartyUnits");

            migrationBuilder.DropTable(
                name: "PlayerPassiveAbilities");

            migrationBuilder.DropTable(
                name: "PlayerPresent");

            migrationBuilder.DropTable(
                name: "PlayerPresentHistory");

            migrationBuilder.DropTable(
                name: "PlayerPurchases");

            migrationBuilder.DropTable(
                name: "PlayerQuests");

            migrationBuilder.DropTable(
                name: "PlayerSetUnit");

            migrationBuilder.DropTable(
                name: "PlayerShopInfos");

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
                name: "PlayerWeaponSkins");

            migrationBuilder.DropTable(
                name: "PartyData");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
