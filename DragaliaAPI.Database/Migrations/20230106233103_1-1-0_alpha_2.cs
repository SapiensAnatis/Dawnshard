using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class _110alpha2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerFortBuilds",
                columns: table => new
                {
                    BuildId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    PlantId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    PositionX = table.Column<int>(type: "integer", nullable: false),
                    PositionZ = table.Column<int>(type: "integer", nullable: false),
                    BuildStartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BuildEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    LastIncomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFortBuilds", x => x.BuildId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFortBuilds");
        }
    }
}
