using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetHelper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerHelpers",
                columns: table => new
                {
                    ViewerId = table.Column<long>(type: "bigint", nullable: false),
                    CharaId = table.Column<int>(type: "integer", nullable: false),
                    EquipDragonKeyId = table.Column<long>(type: "bigint", nullable: true),
                    EquipWeaponBodyId = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType1CrestId1 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType1CrestId2 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType1CrestId3 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType2CrestId1 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType2CrestId2 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType3CrestId1 = table.Column<int>(type: "integer", nullable: true),
                    EquipCrestSlotType3CrestId2 = table.Column<int>(type: "integer", nullable: true),
                    EquipTalismanKeyId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHelpers", x => x.ViewerId);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotTy~",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType1CrestId1 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~1",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType1CrestId2 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~2",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType1CrestId3 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~3",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType2CrestId1 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~4",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType2CrestId2 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~5",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType3CrestId1 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerAbilityCrests_ViewerId_EquipCrestSlotT~6",
                        columns: x => new { x.ViewerId, x.EquipCrestSlotType3CrestId2 },
                        principalTable: "PlayerAbilityCrests",
                        principalColumns: new[] { "ViewerId", "AbilityCrestId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerCharaData_ViewerId_CharaId",
                        columns: x => new { x.ViewerId, x.CharaId },
                        principalTable: "PlayerCharaData",
                        principalColumns: new[] { "ViewerId", "CharaId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerDragonData_EquipDragonKeyId",
                        column: x => x.EquipDragonKeyId,
                        principalTable: "PlayerDragonData",
                        principalColumn: "DragonKeyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerTalismans_EquipTalismanKeyId",
                        column: x => x.EquipTalismanKeyId,
                        principalTable: "PlayerTalismans",
                        principalColumn: "TalismanKeyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_PlayerWeapons_ViewerId_EquipWeaponBodyId",
                        columns: x => new { x.ViewerId, x.EquipWeaponBodyId },
                        principalTable: "PlayerWeapons",
                        principalColumns: new[] { "ViewerId", "WeaponBodyId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerHelpers_Players_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Players",
                        principalColumn: "ViewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_EquipDragonKeyId",
                table: "PlayerHelpers",
                column: "EquipDragonKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_EquipTalismanKeyId",
                table: "PlayerHelpers",
                column: "EquipTalismanKeyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_CharaId",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "CharaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType1CrestId1",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType1CrestId1" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType1CrestId2",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType1CrestId2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType1CrestId3",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType1CrestId3" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType2CrestId1",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType2CrestId1" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType2CrestId2",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType2CrestId2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType3CrestId1",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType3CrestId1" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipCrestSlotType3CrestId2",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipCrestSlotType3CrestId2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHelpers_ViewerId_EquipWeaponBodyId",
                table: "PlayerHelpers",
                columns: new[] { "ViewerId", "EquipWeaponBodyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerHelpers");
        }
    }
}
