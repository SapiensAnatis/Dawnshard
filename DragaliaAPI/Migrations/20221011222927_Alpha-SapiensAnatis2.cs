using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class AlphaSapiensAnatis2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerInfo");

            migrationBuilder.CreateSequence(
                name: "Dragon_key_id",
                schema: "dbo",
                startValue: 10000000000L);

            migrationBuilder.AlterColumn<int>(
                name: "GetTime",
                table: "PlayerDragonData",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<long>(
                name: "DragonKeyId",
                table: "PlayerDragonData",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.Dragon_key_id",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "GetTime",
                table: "PlayerCharaData",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Exp",
                table: "PlayerCharaData",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ComboBuildupCount",
                table: "PlayerCharaData",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "PlayerUserData",
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
                    table.PrimaryKey("PK_PlayerUserData", x => x.DeviceAccountId);
                });

            migrationBuilder.InsertData(
                table: "PlayerUserData",
                columns: new[] { "DeviceAccountId", "ActiveMemoryEventId", "BuildTimePoint", "Coin", "CreateTime", "Crystal", "DewPoint", "EmblemId", "Exp", "FortOpenTime", "IsOptin", "LastLoginTime", "LastStaminaMultiUpdateTime", "LastStaminaSingleUpdateTime", "Level", "MainPartyNo", "ManaPoint", "MaxAmuletQuantity", "MaxDragonQuantity", "MaxWeaponQuantity", "Name", "PrologueEndTime", "QuestSkipPoint", "StaminaMulti", "StaminaMultiSurplusSecond", "StaminaSingle", "StaminaSingleSurplusSecond", "TutorialFlag", "TutorialStatus" },
                values: new object[] { "id", 0, 0, 0, 1665527367, 0, 0, 40000001, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 160, 0, "Euden", 0, 0, 12, 0, 18, 0, 0, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerUserData");

            migrationBuilder.DropSequence(
                name: "Dragon_key_id",
                schema: "dbo");

            migrationBuilder.AlterColumn<bool>(
                name: "GetTime",
                table: "PlayerDragonData",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "DragonKeyId",
                table: "PlayerDragonData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR dbo.Dragon_key_id");

            migrationBuilder.AlterColumn<bool>(
                name: "GetTime",
                table: "PlayerCharaData",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Exp",
                table: "PlayerCharaData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "ComboBuildupCount",
                table: "PlayerCharaData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "PlayerInfo",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActiveMemoryEventId = table.Column<int>(type: "int", nullable: false),
                    BuildTimePoint = table.Column<int>(type: "int", nullable: false),
                    Coin = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<int>(type: "int", nullable: false),
                    Crystal = table.Column<int>(type: "int", nullable: false),
                    DewPoint = table.Column<int>(type: "int", nullable: false),
                    EmblemId = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    FortOpenTime = table.Column<int>(type: "int", nullable: false),
                    IsOptin = table.Column<int>(type: "int", nullable: false),
                    LastLoginTime = table.Column<int>(type: "int", nullable: false),
                    LastStaminaMultiUpdateTime = table.Column<int>(type: "int", nullable: false),
                    LastStaminaSingleUpdateTime = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    MainPartyNo = table.Column<int>(type: "int", nullable: false),
                    ManaPoint = table.Column<int>(type: "int", nullable: false),
                    MaxAmuletQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxDragonQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxWeaponQuantity = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrologueEndTime = table.Column<int>(type: "int", nullable: false),
                    QuestSkipPoint = table.Column<int>(type: "int", nullable: false),
                    StaminaMulti = table.Column<int>(type: "int", nullable: false),
                    StaminaMultiSurplusSecond = table.Column<int>(type: "int", nullable: false),
                    StaminaSingle = table.Column<int>(type: "int", nullable: false),
                    StaminaSingleSurplusSecond = table.Column<int>(type: "int", nullable: false),
                    TutorialFlag = table.Column<int>(type: "int", nullable: false),
                    TutorialStatus = table.Column<int>(type: "int", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.Viewer_id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerInfo", x => x.DeviceAccountId);
                });

            migrationBuilder.InsertData(
                table: "PlayerInfo",
                columns: new[] { "DeviceAccountId", "ActiveMemoryEventId", "BuildTimePoint", "Coin", "CreateTime", "Crystal", "DewPoint", "EmblemId", "Exp", "FortOpenTime", "IsOptin", "LastLoginTime", "LastStaminaMultiUpdateTime", "LastStaminaSingleUpdateTime", "Level", "MainPartyNo", "ManaPoint", "MaxAmuletQuantity", "MaxDragonQuantity", "MaxWeaponQuantity", "Name", "PrologueEndTime", "QuestSkipPoint", "StaminaMulti", "StaminaMultiSurplusSecond", "StaminaSingle", "StaminaSingleSurplusSecond", "TutorialFlag", "TutorialStatus", "ViewerId" },
                values: new object[] { "id", 0, 0, 0, 1665434277, 0, 0, 40000001, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 160, 0, "Euden", 0, 0, 12, 0, 18, 0, 0, 0, 0L });
        }
    }
}
