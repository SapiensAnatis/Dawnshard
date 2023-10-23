using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class userimpersionation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserImpersonations",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    ImpersonatedDeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    ImpersonatedViewerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImpersonations", x => x.DeviceAccountId);
                    table.ForeignKey(
                        name: "FK_UserImpersonations_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserImpersonations_Players_ImpersonatedDeviceAccountId",
                        column: x => x.ImpersonatedDeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserImpersonations_ImpersonatedDeviceAccountId",
                table: "UserImpersonations",
                column: "ImpersonatedDeviceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserImpersonations");
        }
    }
}
