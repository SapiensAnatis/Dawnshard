using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class develop4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "FortOpenTime",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "IsOptin",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "LastStaminaMultiUpdateTime",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "LastStaminaSingleUpdateTime",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "MaxAmuletQuantity",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "MaxWeaponQuantity",
                table: "PlayerUserData");

            migrationBuilder.DropColumn(
                name: "PrologueEndTime",
                table: "PlayerUserData");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSaveImportTime",
                table: "PlayerUserData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSaveImportTime",
                table: "PlayerUserData");

            migrationBuilder.AddColumn<int>(
                name: "CreateTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FortOpenTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsOptin",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastLoginTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastStaminaMultiUpdateTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastStaminaSingleUpdateTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAmuletQuantity",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxWeaponQuantity",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrologueEndTime",
                table: "PlayerUserData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
