using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class FriendRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "PlayerFriendships");

            migrationBuilder.CreateTable(
                name: "PlayerFriendRequests",
                columns: table => new
                {
                    FromPlayerViewerId = table.Column<long>(type: "bigint", nullable: false),
                    ToPlayerViewerId = table.Column<long>(type: "bigint", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFriendRequests", x => new { x.FromPlayerViewerId, x.ToPlayerViewerId });
                    table.ForeignKey(
                        name: "FK_PlayerFriendRequests_Players_FromPlayerViewerId",
                        column: x => x.FromPlayerViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerFriendRequests_Players_ToPlayerViewerId",
                        column: x => x.ToPlayerViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFriendRequests_ToPlayerViewerId",
                table: "PlayerFriendRequests",
                column: "ToPlayerViewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFriendRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "PlayerFriendships",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
