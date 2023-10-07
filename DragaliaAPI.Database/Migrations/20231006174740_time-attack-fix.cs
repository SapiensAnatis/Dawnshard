using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class timeattackfix : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_RoomName_DeviceAccou~",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackPlayers_TimeAttackClears_RoomName",
                table: "TimeAttackPlayers");

            migrationBuilder.RenameColumn(
                name: "RoomName",
                table: "TimeAttackPlayers",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "RoomName",
                table: "TimeAttackClearUnits",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "RoomName",
                table: "TimeAttackClears",
                newName: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_DeviceAccount~",
                table: "TimeAttackClearUnits",
                columns: new[] { "GameId", "DeviceAccountId" },
                principalTable: "TimeAttackPlayers",
                principalColumns: new[] { "GameId", "DeviceAccountId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackPlayers_TimeAttackClears_GameId",
                table: "TimeAttackPlayers",
                column: "GameId",
                principalTable: "TimeAttackClears",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_DeviceAccount~",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackPlayers_TimeAttackClears_GameId",
                table: "TimeAttackPlayers");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "TimeAttackPlayers",
                newName: "RoomName");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "TimeAttackClearUnits",
                newName: "RoomName");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "TimeAttackClears",
                newName: "RoomName");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_RoomName_DeviceAccou~",
                table: "TimeAttackClearUnits",
                columns: new[] { "RoomName", "DeviceAccountId" },
                principalTable: "TimeAttackPlayers",
                principalColumns: new[] { "RoomName", "DeviceAccountId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackPlayers_TimeAttackClears_RoomName",
                table: "TimeAttackPlayers",
                column: "RoomName",
                principalTable: "TimeAttackClears",
                principalColumn: "RoomName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
