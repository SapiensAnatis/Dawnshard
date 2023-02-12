using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class _201alpha1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerFortDetail",
                columns: table => new
                {
                    FortId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CarpenterNum = table.Column<int>(type: "integer", nullable: false),
                    MaxCarpenterCount = table.Column<int>(type: "integer", nullable: false),
                    WorkingCarpenterNum = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFortDetail", x => x.FortId);
                    table.ForeignKey(
                        name: "FK_PlayerFortDetail_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortDetail_DeviceAccountId",
                table: "PlayerFortDetail",
                column: "DeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFortDetail");
        }
    }
}
