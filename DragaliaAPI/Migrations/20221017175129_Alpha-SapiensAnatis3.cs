using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class AlphaSapiensAnatis3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PlayerUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1666029088);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PlayerUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665527367);
        }
    }
}
