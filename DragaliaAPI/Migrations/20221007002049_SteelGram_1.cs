using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class SteelGram_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TutorialFlag",
                table: "SavefileUserData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlayerDragonData",
                columns: table => new
                {
                    DEVICE_ACCOUNT_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DRAGON_KEY_ID = table.Column<long>(type: "bigint", nullable: false),
                    DRAGON_ID = table.Column<int>(type: "int", nullable: false),
                    EXP = table.Column<long>(type: "bigint", nullable: false),
                    LEVEL = table.Column<byte>(type: "tinyint", nullable: false),
                    HP_PLUS_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    ATK_PLUS_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    LIMIT_BREAK_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_LOCKED = table.Column<bool>(type: "bit", nullable: false),
                    IS_NEW = table.Column<bool>(type: "bit", nullable: false),
                    SKILL_1_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    ABIL_1_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    ABIL_2_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    GET_TIME = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonData", x => new { x.DEVICE_ACCOUNT_ID, x.DRAGON_KEY_ID });
                });

            migrationBuilder.CreateTable(
                name: "PlayerDragonReliability",
                columns: table => new
                {
                    DEVICE_ACCOUNT_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DRAGON_ID = table.Column<long>(type: "bigint", nullable: false),
                    REL_EXP = table.Column<long>(type: "bigint", nullable: false),
                    REL_TOTAL_EXP = table.Column<long>(type: "bigint", nullable: false),
                    LAST_CONTACT_TIME = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDragonReliability", x => new { x.DEVICE_ACCOUNT_ID, x.DRAGON_ID });
                });

            migrationBuilder.CreateTable(
                name: "PlayerUnitData",
                columns: table => new
                {
                    DEVICE_ACCOUNT_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CHARA_ID = table.Column<int>(type: "int", nullable: false),
                    RARITY = table.Column<byte>(type: "tinyint", nullable: false),
                    EXP = table.Column<long>(type: "bigint", nullable: false),
                    LEVEL = table.Column<byte>(type: "tinyint", nullable: false),
                    ADD_MAX_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    HP_PLUS_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    ATK_PLUS_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    LIMIT_BREAK_CNT = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_NEW = table.Column<bool>(type: "bit", nullable: false),
                    SKILL_1_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    SKILL_2_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    ABIL_1_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    ABIL_2_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    ABIL_3_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    BRST_ATK_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    COMBO_BUILDUP_CNT = table.Column<long>(type: "bigint", nullable: false),
                    HP = table.Column<int>(type: "int", nullable: false),
                    ATK = table.Column<int>(type: "int", nullable: false),
                    EX_ABIL_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    EX_ABIL_2_LVL = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_TEMP = table.Column<bool>(type: "bit", nullable: false),
                    IS_UNLOCK_EDIT_SKILL = table.Column<bool>(type: "bit", nullable: false),
                    MANA_NODE_UNLOCK_CNT = table.Column<int>(type: "int", nullable: false),
                    LIST_VIEW_FLAG = table.Column<bool>(type: "bit", nullable: false),
                    GET_TIME = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUnitData", x => new { x.DEVICE_ACCOUNT_ID, x.CHARA_ID });
                });

            migrationBuilder.CreateTable(
                name: "PlayerUnitStory",
                columns: table => new
                {
                    DEVICE_ACCOUNT_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ENTITY_TYPE = table.Column<byte>(type: "tinyint", nullable: false),
                    ENTITY_ID = table.Column<long>(type: "bigint", nullable: false),
                    STORY_ID = table.Column<long>(type: "bigint", nullable: false),
                    DRAGON_ID = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUnitStory", x => new { x.DEVICE_ACCOUNT_ID, x.ENTITY_TYPE, x.ENTITY_ID, x.STORY_ID });
                });

            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665102049);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerDragonData");

            migrationBuilder.DropTable(
                name: "PlayerDragonReliability");

            migrationBuilder.DropTable(
                name: "PlayerUnitData");

            migrationBuilder.DropTable(
                name: "PlayerUnitStory");

            migrationBuilder.DropColumn(
                name: "TutorialFlag",
                table: "SavefileUserData");

            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1664719303);
        }
    }
}
