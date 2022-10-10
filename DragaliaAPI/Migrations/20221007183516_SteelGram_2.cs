using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class SteelGram_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerUnitData",
                table: "PlayerUnitData");

            migrationBuilder.RenameTable(
                name: "PlayerUnitData",
                newName: "PlayerCharaData");

            migrationBuilder.AlterColumn<int>(
                name: "DRAGON_ID",
                table: "PlayerDragonReliability",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData",
                columns: new[] { "DEVICE_ACCOUNT_ID", "CHARA_ID" });

            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665167716);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData");

            migrationBuilder.RenameTable(
                name: "PlayerCharaData",
                newName: "PlayerUnitData");

            migrationBuilder.AlterColumn<long>(
                name: "DRAGON_ID",
                table: "PlayerDragonReliability",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerUnitData",
                table: "PlayerUnitData",
                columns: new[] { "DEVICE_ACCOUNT_ID", "CHARA_ID" });

            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665102049);
        }
    }
}
