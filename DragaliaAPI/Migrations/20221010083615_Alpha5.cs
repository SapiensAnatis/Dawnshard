using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class Alpha5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665390975);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665001975);
        }
    }
}
