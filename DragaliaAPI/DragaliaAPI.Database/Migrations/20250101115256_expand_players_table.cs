using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class expand_players_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSavefileImportTime",
                table: "Players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavefileOrigin",
                table: "Players",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
            
            migrationBuilder.Sql(
                """
                UPDATE "Players" p
                    SET "LastSavefileImportTime" = u."LastSaveImportTime"
                FROM "PlayerUserData" u
                    WHERE u."ViewerId" = p."ViewerId"
                """);
            
            migrationBuilder.DropColumn(
                name: "LastSaveImportTime",
                table: "PlayerUserData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastSavefileImportTime",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SavefileOrigin",
                table: "Players");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSaveImportTime",
                table: "PlayerUserData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
