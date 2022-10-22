using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class AlphaSapiensAnatis7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonData",
                table: "PlayerDragonData");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDragonData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonData",
                table: "PlayerDragonData",
                column: "DragonKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonData",
                table: "PlayerDragonData");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDragonData",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonData",
                table: "PlayerDragonData",
                columns: new[] { "DeviceAccountId", "DragonKeyId" });
        }
    }
}
