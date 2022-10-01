using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class Alpha : Migration
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
                name: "SavefilePlayerInfo",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.Viewer_id"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Euden"),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Exp = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Crystal = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Coin = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxDragonQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 160),
                    MaxWeaponQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxAmuletQuantity = table.Column<int>(type: "int", nullable: false),
                    QuestSkipPoint = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MainPartyNo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EmblemId = table.Column<int>(type: "int", nullable: false, defaultValue: 40000001),
                    ActiveMemoryEventId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ManaPoint = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DewPoint = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BuildTimePoint = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastLoginTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StaminaSingle = table.Column<int>(type: "int", nullable: false, defaultValue: 18),
                    LastStaminaSingleUpdateTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StaminaSingleSurplusSecond = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StaminaMulti = table.Column<int>(type: "int", nullable: false, defaultValue: 12),
                    LastStaminaMultiUpdateTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StaminaMultiSurplusSecond = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TutorialStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PrologueEndTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsOptin = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FortOpenTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreateTime = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavefilePlayerInfo", x => x.DeviceAccountId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceAccounts");

            migrationBuilder.DropTable(
                name: "SavefilePlayerInfo");

            migrationBuilder.DropSequence(
                name: "Viewer_id",
                schema: "dbo");
        }
    }
}
