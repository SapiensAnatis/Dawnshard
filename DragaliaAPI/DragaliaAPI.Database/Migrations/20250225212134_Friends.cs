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
                name: "DbPlayerFriendship",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPlayerFriendship", x => x.FriendshipId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerFriendships",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "integer", nullable: false),
                    PlayerViewerId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    PlayerFriendshipId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFriendships", x => new { x.FriendshipId, x.PlayerViewerId });
                    table.ForeignKey(
                        name: "FK_PlayerFriendships_DbPlayerFriendship_FriendshipId",
                        column: x => x.FriendshipId,
                        principalTable: "DbPlayerFriendship",
                        principalColumn: "FriendshipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerFriendships_Players_PlayerViewerId",
                        column: x => x.PlayerViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFriendships_PlayerViewerId",
                table: "PlayerFriendships",
                column: "PlayerViewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFriendships");

            migrationBuilder.DropTable(
                name: "DbPlayerFriendship");
        }
    }
}
