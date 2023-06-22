using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class presents1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerPresent",
                columns: table => new
                {
                    PresentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    MasterId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityQuantity = table.Column<int>(type: "integer", nullable: false),
                    EntityLevel = table.Column<int>(type: "integer", nullable: false),
                    EntityLimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    EntityStatusPlusCount = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue1 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue2 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue3 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue4 = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReceiveLimitTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPresent", x => x.PresentId);
                    table.ForeignKey(
                        name: "FK_PlayerPresent_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPresentHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityQuantity = table.Column<int>(type: "integer", nullable: false),
                    EntityLevel = table.Column<int>(type: "integer", nullable: false),
                    EntityLimitBreakCount = table.Column<int>(type: "integer", nullable: false),
                    EntityStatusPlusCount = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue1 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue2 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue3 = table.Column<int>(type: "integer", nullable: false),
                    MessageParamValue4 = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPresentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPresentHistory_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresent_DeviceAccountId",
                table: "PlayerPresent",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresentHistory_DeviceAccountId",
                table: "PlayerPresentHistory",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPresent");

            migrationBuilder.DropTable(
                name: "PlayerPresentHistory");
        }
    }
}
