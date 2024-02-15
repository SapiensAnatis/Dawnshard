using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class questclearparties2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquippedDragonEntityId",
                table: "QuestClearPartyUnits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquippedTalismanEntityId",
                table: "QuestClearPartyUnits",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquippedDragonEntityId",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropColumn(
                name: "EquippedTalismanEntityId",
                table: "QuestClearPartyUnits");
        }
    }
}
