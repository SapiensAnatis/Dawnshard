using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class Friends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "PlayerFriendships",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFriendships", x => x.FriendshipId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerFriendshipPlayers",
                columns: table => new
                {
                    PlayerViewerId = table.Column<long>(type: "bigint", nullable: false),
                    FriendshipId = table.Column<int>(type: "integer", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFriendshipPlayers", x => new { x.FriendshipId, x.PlayerViewerId });
                    table.ForeignKey(
                        name: "FK_PlayerFriendshipPlayers_PlayerFriendships_FriendshipId",
                        column: x => x.FriendshipId,
                        principalTable: "PlayerFriendships",
                        principalColumn: "FriendshipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerFriendshipPlayers_Players_PlayerViewerId",
                        column: x => x.PlayerViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFriendRequests_ToPlayerViewerId",
                table: "PlayerFriendRequests",
                column: "ToPlayerViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFriendshipPlayers_PlayerViewerId",
                table: "PlayerFriendshipPlayers",
                column: "PlayerViewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFriendRequests");

            migrationBuilder.DropTable(
                name: "PlayerFriendshipPlayers");

            migrationBuilder.DropTable(
                name: "PlayerFriendships");
        }
    }
}
