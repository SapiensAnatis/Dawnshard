using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class develop3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuests_DeviceAccounts_DeviceAccountId",
                table: "PlayerQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTalismans_DeviceAccounts_DeviceAccountId",
                table: "PlayerTalismans");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerQuests_DeviceAccounts_DeviceAccountId",
                table: "PlayerQuests",
                column: "DeviceAccountId",
                principalTable: "DeviceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTalismans_DeviceAccounts_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId",
                principalTable: "DeviceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
