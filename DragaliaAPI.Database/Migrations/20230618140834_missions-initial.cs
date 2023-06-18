using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class missionsinitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions",
                columns: new[] { "DeviceAccountId", "Type", "MissionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions",
                columns: new[] { "DeviceAccountId", "MissionId" });
        }
    }
}
