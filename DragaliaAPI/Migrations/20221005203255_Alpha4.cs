using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class Alpha4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TutorialFlag",
                table: "SavefileUserData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "SavefileUserData",
                keyColumn: "DeviceAccountId",
                keyValue: "id",
                column: "CreateTime",
                value: 1665001975);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
