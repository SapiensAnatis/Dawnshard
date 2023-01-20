using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class _110alpha3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.AccountId);
                });

            // Legacy compatibility with existing deployed database
            migrationBuilder.Sql(
                """
                INSERT INTO "Players" ("AccountId")
                SELECT "DeviceAccountId" FROM "PlayerUserData"
                """
            );

            migrationBuilder.CreateIndex(
                name: "IX_PlayerWeapons_DeviceAccountId",
                table: "PlayerWeapons",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUserData_DeviceAccountId",
                table: "PlayerUserData",
                column: "DeviceAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSummonHistory_DeviceAccountId",
                table: "PlayerSummonHistory",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStoryState_DeviceAccountId",
                table: "PlayerStoryState",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSetUnit_DeviceAccountId",
                table: "PlayerSetUnit",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_DeviceAccountId",
                table: "PlayerQuests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId",
                table: "PlayerPartyUnits",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMaterial_DeviceAccountId",
                table: "PlayerMaterial",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortBuilds_DeviceAccountId",
                table: "PlayerFortBuilds",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonReliability_DeviceAccountId",
                table: "PlayerDragonReliability",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonData_DeviceAccountId",
                table: "PlayerDragonData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCurrency_DeviceAccountId",
                table: "PlayerCurrency",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharaData_DeviceAccountId",
                table: "PlayerCharaData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBannerData_DeviceAccountId",
                table: "PlayerBannerData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAbilityCrests_DeviceAccountId",
                table: "PlayerAbilityCrests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyData_DeviceAccountId",
                table: "PartyData",
                column: "DeviceAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartyData_Players_DeviceAccountId",
                table: "PartyData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAbilityCrests_Players_DeviceAccountId",
                table: "PlayerAbilityCrests",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerBannerData_Players_DeviceAccountId",
                table: "PlayerBannerData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharaData_Players_DeviceAccountId",
                table: "PlayerCharaData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCurrency_Players_DeviceAccountId",
                table: "PlayerCurrency",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonData_Players_DeviceAccountId",
                table: "PlayerDragonData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonReliability_Players_DeviceAccountId",
                table: "PlayerDragonReliability",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFortBuilds_Players_DeviceAccountId",
                table: "PlayerFortBuilds",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerMaterial_Players_DeviceAccountId",
                table: "PlayerMaterial",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerQuests_Players_DeviceAccountId",
                table: "PlayerQuests",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSetUnit_Players_DeviceAccountId",
                table: "PlayerSetUnit",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStoryState_Players_DeviceAccountId",
                table: "PlayerStoryState",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSummonHistory_Players_DeviceAccountId",
                table: "PlayerSummonHistory",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTalismans_Players_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUserData_Players_DeviceAccountId",
                table: "PlayerUserData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerWeapons_Players_DeviceAccountId",
                table: "PlayerWeapons",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyData_Players_DeviceAccountId",
                table: "PartyData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilityCrests_Players_DeviceAccountId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerBannerData_Players_DeviceAccountId",
                table: "PlayerBannerData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharaData_Players_DeviceAccountId",
                table: "PlayerCharaData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCurrency_Players_DeviceAccountId",
                table: "PlayerCurrency");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonData_Players_DeviceAccountId",
                table: "PlayerDragonData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonReliability_Players_DeviceAccountId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFortBuilds_Players_DeviceAccountId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMaterial_Players_DeviceAccountId",
                table: "PlayerMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuests_Players_DeviceAccountId",
                table: "PlayerQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSetUnit_Players_DeviceAccountId",
                table: "PlayerSetUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStoryState_Players_DeviceAccountId",
                table: "PlayerStoryState");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSummonHistory_Players_DeviceAccountId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTalismans_Players_DeviceAccountId",
                table: "PlayerTalismans");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerUserData_Players_DeviceAccountId",
                table: "PlayerUserData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerWeapons_Players_DeviceAccountId",
                table: "PlayerWeapons");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropIndex(
                name: "IX_PlayerWeapons_DeviceAccountId",
                table: "PlayerWeapons");

            migrationBuilder.DropIndex(
                name: "IX_PlayerUserData_DeviceAccountId",
                table: "PlayerUserData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSummonHistory_DeviceAccountId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStoryState_DeviceAccountId",
                table: "PlayerStoryState");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSetUnit_DeviceAccountId",
                table: "PlayerSetUnit");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQuests_DeviceAccountId",
                table: "PlayerQuests");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId",
                table: "PlayerPartyUnits");

            migrationBuilder.DropIndex(
                name: "IX_PlayerMaterial_DeviceAccountId",
                table: "PlayerMaterial");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFortBuilds_DeviceAccountId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonReliability_DeviceAccountId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonData_DeviceAccountId",
                table: "PlayerDragonData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCurrency_DeviceAccountId",
                table: "PlayerCurrency");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCharaData_DeviceAccountId",
                table: "PlayerCharaData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerBannerData_DeviceAccountId",
                table: "PlayerBannerData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAbilityCrests_DeviceAccountId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropIndex(
                name: "IX_PartyData_DeviceAccountId",
                table: "PartyData");
        }
    }
}
