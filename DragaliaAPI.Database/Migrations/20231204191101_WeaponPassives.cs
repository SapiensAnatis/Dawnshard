using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class WeaponPassives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<int[]>(
                name: "UnlockWeaponPassiveAbilityNoList",
                table: "PlayerWeapons",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.Sql(
                """
                UPDATE "PlayerWeapons" w SET "UnlockWeaponPassiveAbilityNoList" = string_to_array("UnlockWeaponPassiveAbilityNoString", ',')::int[];
                """);
            
            migrationBuilder.DropColumn(
                name: "UnlockWeaponPassiveAbilityNoString",
                table: "PlayerWeapons");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnlockWeaponPassiveAbilityNoList",
                table: "PlayerWeapons");

            migrationBuilder.AddColumn<string>(
                name: "UnlockWeaponPassiveAbilityNoString",
                table: "PlayerWeapons",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
