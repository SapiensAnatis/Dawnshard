using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceAccount",
                table: "DeviceAccount");

            migrationBuilder.RenameTable(
                name: "DeviceAccount",
                newName: "DeviceAccounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceAccounts",
                table: "DeviceAccounts",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceAccounts",
                table: "DeviceAccounts");

            migrationBuilder.RenameTable(
                name: "DeviceAccounts",
                newName: "DeviceAccount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceAccount",
                table: "DeviceAccount",
                column: "Id");
        }
    }
}
