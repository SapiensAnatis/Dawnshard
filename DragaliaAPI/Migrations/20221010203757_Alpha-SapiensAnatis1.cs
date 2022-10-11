using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class AlphaSapiensAnatis1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateSequence(
                name: "Viewer_id",
                schema: "dbo",
                startValue: 10000000000L);

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
                name: "PlayerCharaData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharaId = table.Column<int>(type: "int", nullable: false),
                    Rarity = table.Column<byte>(type: "tinyint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    AdditionalMaxLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "tinyint", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    FirstSkillLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    SecondSkillLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    FirstAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    SecondAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    ThirdAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    BurstAttackLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    ComboBuildupCount = table.Column<long>(type: "bigint", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    FirstExAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    SecondExAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    IsTemporary = table.Column<bool>(type: "bit", nullable: false),
                    IsUnlockEditSkill = table.Column<bool>(type: "bit", nullable: false),
                    ManaNodeUnlockCount = table.Column<int>(type: "int", nullable: false),
                    ListViewFlag = table.Column<bool>(type: "bit", nullable: false),
                    GetTime = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharaData", x => new { x.DeviceAccountId, x.CharaId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonData",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DragonKeyId = table.Column<long>(type: "bigint", nullable: false),
                    DragonId = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    HpPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    AttackPlusCount = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitBreakCount = table.Column<byte>(type: "tinyint", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    FirstSkillLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    FirstAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    SecondAbilityLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    GetTime = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonData", x => new { x.DeviceAccountId, x.DragonKeyId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonReliability",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DragonId = table.Column<int>(type: "int", nullable: false),
                    ReliabilityExp = table.Column<long>(type: "bigint", nullable: false),
                    ReliabilityTotalExp = table.Column<long>(type: "bigint", nullable: false),
                    LastContactTime = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonReliability", x => new { x.DeviceAccountId, x.DragonId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerInfo",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.Viewer_id"),
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
                    table.PrimaryKey("PK_PlayerInfo", x => x.DeviceAccountId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUnitStory",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    StoryId = table.Column<long>(type: "bigint", nullable: false),
                    DragonId = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUnitStory", x => new { x.DeviceAccountId, x.EntityType, x.EntityId, x.StoryId });
                });

            migrationBuilder.InsertData(
                table: "DeviceAccounts",
                columns: new[] { "Id", "HashedPassword" },
                values: new object[] { "id", "NMvdakTznEF6khwWcz17i6GTnDA=" });

            migrationBuilder.InsertData(
                table: "PlayerInfo",
                columns: new[] { "DeviceAccountId", "ActiveMemoryEventId", "BuildTimePoint", "Coin", "CreateTime", "Crystal", "DewPoint", "EmblemId", "Exp", "FortOpenTime", "IsOptin", "LastLoginTime", "LastStaminaMultiUpdateTime", "LastStaminaSingleUpdateTime", "Level", "MainPartyNo", "ManaPoint", "MaxAmuletQuantity", "MaxDragonQuantity", "MaxWeaponQuantity", "Name", "PrologueEndTime", "QuestSkipPoint", "StaminaMulti", "StaminaMultiSurplusSecond", "StaminaSingle", "StaminaSingleSurplusSecond", "TutorialFlag", "TutorialStatus" },
                values: new object[] { "id", 0, 0, 0, 1665434277, 0, 0, 40000001, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 160, 0, "Euden", 0, 0, 12, 0, 18, 0, 0, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceAccounts");

            migrationBuilder.DropTable(
                name: "PlayerCharaData");

            migrationBuilder.DropTable(
                name: "PlayerDragonData");

            migrationBuilder.DropTable(
                name: "PlayerDragonReliability");

            migrationBuilder.DropTable(
                name: "PlayerInfo");

            migrationBuilder.DropTable(
                name: "PlayerUnitStory");

            migrationBuilder.DropSequence(
                name: "Viewer_id",
                schema: "dbo");
        }
    }
}
