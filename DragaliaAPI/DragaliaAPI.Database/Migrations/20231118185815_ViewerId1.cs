using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DragaliaAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class ViewerId1 : Migration
    {
        private void PopulateViewerId(MigrationBuilder migrationBuilder)
        {
            string[] tables = [
                "TimeAttackPlayers",
                "TimeAttackClearUnits",
                "ReceivedRankingTierRewards",
                "QuestTreasureList",
                "QuestEvents",
                "QuestClearPartyUnits",
                "PlayerWeaponSkins",
                "PlayerWeapons",
                "PlayerUseItems",
                "PlayerTrades",
                "PlayerTalismans",
                "PlayerSummonTickets",
                "PlayerSummonHistory",
                "PlayerStoryState",
                "PlayerShopInfos",
                "PlayerSetUnit",
                "PlayerQuestWalls",
                "PlayerQuests",
                "PlayerPurchases",
                "PlayerPresentHistory",
                "PlayerPresent",
                "PlayerPassiveAbilities",
                "PlayerPartyUnits",
                "PlayerMissions",
                "PlayerMaterial",
                "PlayerFortDetail",
                "PlayerFortBuilds",
                "PlayerEventRewards",
                "PlayerEventPassives",
                "PlayerEventItems",
                "PlayerEventData",
                "PlayerDragonReliability",
                "PlayerDragonGift",
                "PlayerDragonData",
                "PlayerDmodeServitorPassives",
                "PlayerDmodeInfos",
                "PlayerDmodeExpeditions",
                "PlayerDmodeDungeons",
                "PlayerDmodeCharas",
                "PlayerCharaData",
                "PlayerBannerData",
                "PlayerAbilityCrestSets",
                "PlayerAbilityCrests",
                "PartyPowers",
                "PartyData",
                "LoginBonuses",
                "EquippedStamps",
                "Emblems",
            ];

            foreach (string table in tables)
            {
                // Modify the column in-place instead of dropping and renaming, otherwise the ViewerId
                // column is at the end of the table which deeply annoys me

                migrationBuilder.Sql($"""
                    UPDATE "{table}" t
                    SET "DeviceAccountId" = p."ViewerId"::text
                    FROM "PlayerUserData" p
                    WHERE p."DeviceAccountId" = t."DeviceAccountId";
                    """
                );

                migrationBuilder.Sql($"""
                    ALTER TABLE "{table}"
                    ALTER COLUMN "DeviceAccountId" TYPE bigint
                    USING "DeviceAccountId"::integer;
                    """
                );

                migrationBuilder.Sql($"""
                    ALTER TABLE "{table}"
                    RENAME COLUMN "DeviceAccountId" TO "ViewerId";
                    """
                );

            }
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET LOCAL statement_timeout = 60000;");

            migrationBuilder.DropForeignKey(
                name: "FK_Emblems_Players_DeviceAccountId",
                table: "Emblems");

            migrationBuilder.DropForeignKey(
                name: "FK_EquippedStamps_Players_DeviceAccountId",
                table: "EquippedStamps");

            migrationBuilder.DropForeignKey(
                name: "FK_LoginBonuses_Players_DeviceAccountId",
                table: "LoginBonuses");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyData_Players_DeviceAccountId",
                table: "PartyData");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyPowers_Players_DeviceAccountId",
                table: "PartyPowers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilityCrests_Players_DeviceAccountId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilityCrestSets_Players_DeviceAccountId",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerBannerData_Players_DeviceAccountId",
                table: "PlayerBannerData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharaData_Players_DeviceAccountId",
                table: "PlayerCharaData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeCharas_Players_DeviceAccountId",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeDungeons_Players_DeviceAccountId",
                table: "PlayerDmodeDungeons");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeExpeditions_Players_DeviceAccountId",
                table: "PlayerDmodeExpeditions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeInfos_Players_DeviceAccountId",
                table: "PlayerDmodeInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeServitorPassives_Players_DeviceAccountId",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonData_Players_DeviceAccountId",
                table: "PlayerDragonData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonReliability_Players_DeviceAccountId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventData_Players_DeviceAccountId",
                table: "PlayerEventData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventItems_Players_DeviceAccountId",
                table: "PlayerEventItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventPassives_Players_DeviceAccountId",
                table: "PlayerEventPassives");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventRewards_Players_DeviceAccountId",
                table: "PlayerEventRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFortBuilds_Players_DeviceAccountId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFortDetail_Players_DeviceAccountId",
                table: "PlayerFortDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMaterial_Players_DeviceAccountId",
                table: "PlayerMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMissions_Players_DeviceAccountId",
                table: "PlayerMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPartyUnits_PartyData_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPassiveAbilities_Players_DeviceAccountId",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPresent_Players_DeviceAccountId",
                table: "PlayerPresent");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPresentHistory_Players_DeviceAccountId",
                table: "PlayerPresentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPurchases_Players_DeviceAccountId",
                table: "PlayerPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuests_Players_DeviceAccountId",
                table: "PlayerQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuestWalls_Players_DeviceAccountId",
                table: "PlayerQuestWalls");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSetUnit_Players_DeviceAccountId",
                table: "PlayerSetUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerShopInfos_Players_DeviceAccountId",
                table: "PlayerShopInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStoryState_Players_DeviceAccountId",
                table: "PlayerStoryState");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSummonHistory_Players_DeviceAccountId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSummonTickets_Players_DeviceAccountId",
                table: "PlayerSummonTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTalismans_Players_DeviceAccountId",
                table: "PlayerTalismans");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrades_Players_DeviceAccountId",
                table: "PlayerTrades");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerUseItems_Players_DeviceAccountId",
                table: "PlayerUseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerUserData_Players_DeviceAccountId",
                table: "PlayerUserData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerWeapons_Players_DeviceAccountId",
                table: "PlayerWeapons");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerWeaponSkins_Players_DeviceAccountId",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestClearPartyUnits_Players_DeviceAccountId",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestEvents_Players_DeviceAccountId",
                table: "QuestEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestTreasureList_Players_DeviceAccountId",
                table: "QuestTreasureList");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedRankingTierRewards_Players_DeviceAccountId",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_DeviceAccount~",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackPlayers_Players_DeviceAccountId",
                table: "TimeAttackPlayers");

            migrationBuilder.DropTable(
                name: "PlayerCurrency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeAttackPlayers",
                table: "TimeAttackPlayers");

            migrationBuilder.DropIndex(
                name: "IX_TimeAttackPlayers_DeviceAccountId",
                table: "TimeAttackPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeAttackClearUnits",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceivedRankingTierRewards",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedRankingTierRewards_DeviceAccountId",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestTreasureList",
                table: "QuestTreasureList");

            migrationBuilder.DropIndex(
                name: "IX_QuestTreasureList_DeviceAccountId",
                table: "QuestTreasureList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestEvents",
                table: "QuestEvents");

            migrationBuilder.DropIndex(
                name: "IX_QuestEvents_DeviceAccountId",
                table: "QuestEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestClearPartyUnits",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerWeaponSkins",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropIndex(
                name: "IX_PlayerWeaponSkins_DeviceAccountId",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerWeapons",
                table: "PlayerWeapons");

            migrationBuilder.DropIndex(
                name: "IX_PlayerWeapons_DeviceAccountId",
                table: "PlayerWeapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerUserData",
                table: "PlayerUserData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerUserData_DeviceAccountId",
                table: "PlayerUserData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerUseItems",
                table: "PlayerUseItems");

            migrationBuilder.DropIndex(
                name: "IX_PlayerUseItems_DeviceAccountId",
                table: "PlayerUseItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerTrades",
                table: "PlayerTrades");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTrades_DeviceAccountId",
                table: "PlayerTrades");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTrades_DeviceAccountId_TradeType",
                table: "PlayerTrades");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSummonTickets_DeviceAccountId",
                table: "PlayerSummonTickets");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSummonHistory_DeviceAccountId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerStoryState",
                table: "PlayerStoryState");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStoryState_DeviceAccountId",
                table: "PlayerStoryState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerShopInfos",
                table: "PlayerShopInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSetUnit",
                table: "PlayerSetUnit");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSetUnit_DeviceAccountId",
                table: "PlayerSetUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuestWalls",
                table: "PlayerQuestWalls");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQuestWalls_DeviceAccountId",
                table: "PlayerQuestWalls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQuests_DeviceAccountId",
                table: "PlayerQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPurchases",
                table: "PlayerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPurchases_DeviceAccountId",
                table: "PlayerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPresentHistory_DeviceAccountId",
                table: "PlayerPresentHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPresent_DeviceAccountId",
                table: "PlayerPresent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPassiveAbilities",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPassiveAbilities_DeviceAccountId",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId",
                table: "PlayerPartyUnits");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMaterial",
                table: "PlayerMaterial");

            migrationBuilder.DropIndex(
                name: "IX_PlayerMaterial_DeviceAccountId",
                table: "PlayerMaterial");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerFortDetail",
                table: "PlayerFortDetail");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFortDetail_DeviceAccountId",
                table: "PlayerFortDetail");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFortBuilds_DeviceAccountId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventRewards",
                table: "PlayerEventRewards");

            migrationBuilder.DropIndex(
                name: "IX_PlayerEventRewards_DeviceAccountId_EventId",
                table: "PlayerEventRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventPassives",
                table: "PlayerEventPassives");

            migrationBuilder.DropIndex(
                name: "IX_PlayerEventPassives_DeviceAccountId_EventId",
                table: "PlayerEventPassives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventItems",
                table: "PlayerEventItems");

            migrationBuilder.DropIndex(
                name: "IX_PlayerEventItems_DeviceAccountId_EventId",
                table: "PlayerEventItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventData",
                table: "PlayerEventData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerEventData_DeviceAccountId",
                table: "PlayerEventData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonReliability",
                table: "PlayerDragonReliability");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonReliability_DeviceAccountId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonGift",
                table: "PlayerDragonGift");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonData_DeviceAccountId",
                table: "PlayerDragonData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeServitorPassives",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDmodeServitorPassives_DeviceAccountId",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeInfos",
                table: "PlayerDmodeInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeExpeditions",
                table: "PlayerDmodeExpeditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeDungeons",
                table: "PlayerDmodeDungeons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeCharas",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDmodeCharas_DeviceAccountId",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCharaData_DeviceAccountId",
                table: "PlayerCharaData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerBannerData",
                table: "PlayerBannerData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerBannerData_DeviceAccountId",
                table: "PlayerBannerData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAbilityCrestSets",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAbilityCrestSets_DeviceAccountId",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAbilityCrests",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAbilityCrests_DeviceAccountId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyPowers",
                table: "PartyPowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyData",
                table: "PartyData");

            migrationBuilder.DropIndex(
                name: "IX_PartyData_DeviceAccountId",
                table: "PartyData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginBonuses",
                table: "LoginBonuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquippedStamps",
                table: "EquippedStamps");

            migrationBuilder.DropIndex(
                name: "IX_EquippedStamps_DeviceAccountId",
                table: "EquippedStamps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emblems",
                table: "Emblems");

            migrationBuilder.DropIndex(
                name: "IX_Emblems_DeviceAccountId",
                table: "Emblems");

            migrationBuilder.AlterColumn<long>(
                name: "ViewerId",
                table: "PlayerUserData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "ViewerId",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.Sql($"""
                UPDATE "Players" t 
                SET "ViewerId" = p."ViewerId"
                FROM "PlayerUserData" p
                WHERE p."DeviceAccountId" = t."AccountId";
                """);

            this.PopulateViewerId(migrationBuilder);

            migrationBuilder.DropColumn("DeviceAccountId", "PlayerUserData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeAttackPlayers",
                table: "TimeAttackPlayers",
                columns: new[] { "GameId", "ViewerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeAttackClearUnits",
                table: "TimeAttackClearUnits",
                columns: new[] { "GameId", "ViewerId", "UnitNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceivedRankingTierRewards",
                table: "ReceivedRankingTierRewards",
                columns: new[] { "ViewerId", "RewardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestTreasureList",
                table: "QuestTreasureList",
                columns: new[] { "ViewerId", "QuestTreasureId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestEvents",
                table: "QuestEvents",
                columns: new[] { "ViewerId", "QuestEventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestClearPartyUnits",
                table: "QuestClearPartyUnits",
                columns: new[] { "ViewerId", "QuestId", "IsMulti", "UnitNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerWeaponSkins",
                table: "PlayerWeaponSkins",
                columns: new[] { "ViewerId", "WeaponSkinId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerWeapons",
                table: "PlayerWeapons",
                columns: new[] { "ViewerId", "WeaponBodyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerUserData",
                table: "PlayerUserData",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerUseItems",
                table: "PlayerUseItems",
                columns: new[] { "ViewerId", "ItemId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerTrades",
                table: "PlayerTrades",
                columns: new[] { "ViewerId", "TradeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerStoryState",
                table: "PlayerStoryState",
                columns: new[] { "ViewerId", "StoryType", "StoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerShopInfos",
                table: "PlayerShopInfos",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSetUnit",
                table: "PlayerSetUnit",
                columns: new[] { "ViewerId", "CharaId", "UnitSetNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuestWalls",
                table: "PlayerQuestWalls",
                columns: new[] { "ViewerId", "WallId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests",
                columns: new[] { "ViewerId", "QuestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPurchases",
                table: "PlayerPurchases",
                columns: new[] { "ViewerId", "GoodsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPassiveAbilities",
                table: "PlayerPassiveAbilities",
                columns: new[] { "ViewerId", "WeaponPassiveAbilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions",
                columns: new[] { "ViewerId", "MissionId", "Type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMaterial",
                table: "PlayerMaterial",
                columns: new[] { "ViewerId", "MaterialId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerFortDetail",
                table: "PlayerFortDetail",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventRewards",
                table: "PlayerEventRewards",
                columns: new[] { "ViewerId", "EventId", "RewardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventPassives",
                table: "PlayerEventPassives",
                columns: new[] { "ViewerId", "EventId", "PassiveId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventItems",
                table: "PlayerEventItems",
                columns: new[] { "ViewerId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventData",
                table: "PlayerEventData",
                columns: new[] { "ViewerId", "EventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonReliability",
                table: "PlayerDragonReliability",
                columns: new[] { "ViewerId", "DragonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonGift",
                table: "PlayerDragonGift",
                columns: new[] { "ViewerId", "DragonGiftId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeServitorPassives",
                table: "PlayerDmodeServitorPassives",
                columns: new[] { "ViewerId", "PassiveId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeInfos",
                table: "PlayerDmodeInfos",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeExpeditions",
                table: "PlayerDmodeExpeditions",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeDungeons",
                table: "PlayerDmodeDungeons",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeCharas",
                table: "PlayerDmodeCharas",
                columns: new[] { "ViewerId", "CharaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData",
                columns: new[] { "ViewerId", "CharaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerBannerData",
                table: "PlayerBannerData",
                columns: new[] { "ViewerId", "SummonBannerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAbilityCrestSets",
                table: "PlayerAbilityCrestSets",
                columns: new[] { "ViewerId", "AbilityCrestSetNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAbilityCrests",
                table: "PlayerAbilityCrests",
                columns: new[] { "ViewerId", "AbilityCrestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyPowers",
                table: "PartyPowers",
                column: "ViewerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyData",
                table: "PartyData",
                columns: new[] { "ViewerId", "PartyNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginBonuses",
                table: "LoginBonuses",
                columns: new[] { "ViewerId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquippedStamps",
                table: "EquippedStamps",
                columns: new[] { "ViewerId", "Slot" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emblems",
                table: "Emblems",
                columns: new[] { "ViewerId", "EmblemId" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeAttackPlayers_ViewerId",
                table: "TimeAttackPlayers",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrades_ViewerId_TradeType",
                table: "PlayerTrades",
                columns: new[] { "ViewerId", "TradeType" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_ViewerId",
                table: "PlayerTalismans",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSummonTickets_ViewerId",
                table: "PlayerSummonTickets",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSummonHistory_ViewerId",
                table: "PlayerSummonHistory",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_AccountId",
                table: "Players",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresentHistory_ViewerId",
                table: "PlayerPresentHistory",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresent_ViewerId",
                table: "PlayerPresent",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_ViewerId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "ViewerId", "PartyNo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortBuilds_ViewerId",
                table: "PlayerFortBuilds",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventItems_ViewerId_EventId",
                table: "PlayerEventItems",
                columns: new[] { "ViewerId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonData_ViewerId",
                table: "PlayerDragonData",
                column: "ViewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emblems_Players_ViewerId",
                table: "Emblems",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquippedStamps_Players_ViewerId",
                table: "EquippedStamps",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginBonuses_Players_ViewerId",
                table: "LoginBonuses",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyData_Players_ViewerId",
                table: "PartyData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPowers_Players_ViewerId",
                table: "PartyPowers",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAbilityCrests_Players_ViewerId",
                table: "PlayerAbilityCrests",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAbilityCrestSets_Players_ViewerId",
                table: "PlayerAbilityCrestSets",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerBannerData_Players_ViewerId",
                table: "PlayerBannerData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharaData_Players_ViewerId",
                table: "PlayerCharaData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeCharas_Players_ViewerId",
                table: "PlayerDmodeCharas",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeDungeons_Players_ViewerId",
                table: "PlayerDmodeDungeons",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeExpeditions_Players_ViewerId",
                table: "PlayerDmodeExpeditions",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeInfos_Players_ViewerId",
                table: "PlayerDmodeInfos",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeServitorPassives_Players_ViewerId",
                table: "PlayerDmodeServitorPassives",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonData_Players_ViewerId",
                table: "PlayerDragonData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonGift_Players_ViewerId",
                table: "PlayerDragonGift",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDragonReliability_Players_ViewerId",
                table: "PlayerDragonReliability",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventData_Players_ViewerId",
                table: "PlayerEventData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventItems_Players_ViewerId",
                table: "PlayerEventItems",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventPassives_Players_ViewerId",
                table: "PlayerEventPassives",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventRewards_Players_ViewerId",
                table: "PlayerEventRewards",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFortBuilds_Players_ViewerId",
                table: "PlayerFortBuilds",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFortDetail_Players_ViewerId",
                table: "PlayerFortDetail",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerMaterial_Players_ViewerId",
                table: "PlayerMaterial",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerMissions_Players_ViewerId",
                table: "PlayerMissions",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPartyUnits_PartyData_ViewerId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "ViewerId", "PartyNo" },
                principalTable: "PartyData",
                principalColumns: new[] { "ViewerId", "PartyNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPassiveAbilities_Players_ViewerId",
                table: "PlayerPassiveAbilities",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPresent_Players_ViewerId",
                table: "PlayerPresent",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPresentHistory_Players_ViewerId",
                table: "PlayerPresentHistory",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPurchases_Players_ViewerId",
                table: "PlayerPurchases",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerQuests_Players_ViewerId",
                table: "PlayerQuests",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerQuestWalls_Players_ViewerId",
                table: "PlayerQuestWalls",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSetUnit_Players_ViewerId",
                table: "PlayerSetUnit",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerShopInfos_Players_ViewerId",
                table: "PlayerShopInfos",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStoryState_Players_ViewerId",
                table: "PlayerStoryState",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSummonHistory_Players_ViewerId",
                table: "PlayerSummonHistory",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSummonTickets_Players_ViewerId",
                table: "PlayerSummonTickets",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTalismans_Players_ViewerId",
                table: "PlayerTalismans",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTrades_Players_ViewerId",
                table: "PlayerTrades",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUseItems_Players_ViewerId",
                table: "PlayerUseItems",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUserData_Players_ViewerId",
                table: "PlayerUserData",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerWeapons_Players_ViewerId",
                table: "PlayerWeapons",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerWeaponSkins_Players_ViewerId",
                table: "PlayerWeaponSkins",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestClearPartyUnits_Players_ViewerId",
                table: "QuestClearPartyUnits",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestEvents_Players_ViewerId",
                table: "QuestEvents",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestTreasureList_Players_ViewerId",
                table: "QuestTreasureList",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedRankingTierRewards_Players_ViewerId",
                table: "ReceivedRankingTierRewards",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_ViewerId",
                table: "TimeAttackClearUnits",
                columns: new[] { "GameId", "ViewerId" },
                principalTable: "TimeAttackPlayers",
                principalColumns: new[] { "GameId", "ViewerId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackPlayers_Players_ViewerId",
                table: "TimeAttackPlayers",
                column: "ViewerId",
                principalTable: "Players",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emblems_Players_ViewerId",
                table: "Emblems");

            migrationBuilder.DropForeignKey(
                name: "FK_EquippedStamps_Players_ViewerId",
                table: "EquippedStamps");

            migrationBuilder.DropForeignKey(
                name: "FK_LoginBonuses_Players_ViewerId",
                table: "LoginBonuses");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyData_Players_ViewerId",
                table: "PartyData");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyPowers_Players_ViewerId",
                table: "PartyPowers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilityCrests_Players_ViewerId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAbilityCrestSets_Players_ViewerId",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerBannerData_Players_ViewerId",
                table: "PlayerBannerData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharaData_Players_ViewerId",
                table: "PlayerCharaData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeCharas_Players_ViewerId",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeDungeons_Players_ViewerId",
                table: "PlayerDmodeDungeons");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeExpeditions_Players_ViewerId",
                table: "PlayerDmodeExpeditions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeInfos_Players_ViewerId",
                table: "PlayerDmodeInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDmodeServitorPassives_Players_ViewerId",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonData_Players_ViewerId",
                table: "PlayerDragonData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonGift_Players_ViewerId",
                table: "PlayerDragonGift");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerDragonReliability_Players_ViewerId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventData_Players_ViewerId",
                table: "PlayerEventData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventItems_Players_ViewerId",
                table: "PlayerEventItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventPassives_Players_ViewerId",
                table: "PlayerEventPassives");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerEventRewards_Players_ViewerId",
                table: "PlayerEventRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFortBuilds_Players_ViewerId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFortDetail_Players_ViewerId",
                table: "PlayerFortDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMaterial_Players_ViewerId",
                table: "PlayerMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerMissions_Players_ViewerId",
                table: "PlayerMissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPartyUnits_PartyData_ViewerId_PartyNo",
                table: "PlayerPartyUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPassiveAbilities_Players_ViewerId",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPresent_Players_ViewerId",
                table: "PlayerPresent");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPresentHistory_Players_ViewerId",
                table: "PlayerPresentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPurchases_Players_ViewerId",
                table: "PlayerPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuests_Players_ViewerId",
                table: "PlayerQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerQuestWalls_Players_ViewerId",
                table: "PlayerQuestWalls");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSetUnit_Players_ViewerId",
                table: "PlayerSetUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerShopInfos_Players_ViewerId",
                table: "PlayerShopInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStoryState_Players_ViewerId",
                table: "PlayerStoryState");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSummonHistory_Players_ViewerId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSummonTickets_Players_ViewerId",
                table: "PlayerSummonTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTalismans_Players_ViewerId",
                table: "PlayerTalismans");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrades_Players_ViewerId",
                table: "PlayerTrades");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerUseItems_Players_ViewerId",
                table: "PlayerUseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerUserData_Players_ViewerId",
                table: "PlayerUserData");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerWeapons_Players_ViewerId",
                table: "PlayerWeapons");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerWeaponSkins_Players_ViewerId",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestClearPartyUnits_Players_ViewerId",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestEvents_Players_ViewerId",
                table: "QuestEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestTreasureList_Players_ViewerId",
                table: "QuestTreasureList");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedRankingTierRewards_Players_ViewerId",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_ViewerId",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeAttackPlayers_Players_ViewerId",
                table: "TimeAttackPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeAttackPlayers",
                table: "TimeAttackPlayers");

            migrationBuilder.DropIndex(
                name: "IX_TimeAttackPlayers_ViewerId",
                table: "TimeAttackPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeAttackClearUnits",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceivedRankingTierRewards",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestTreasureList",
                table: "QuestTreasureList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestEvents",
                table: "QuestEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestClearPartyUnits",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerWeaponSkins",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerWeapons",
                table: "PlayerWeapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerUserData",
                table: "PlayerUserData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerUseItems",
                table: "PlayerUseItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerTrades",
                table: "PlayerTrades");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTrades_ViewerId_TradeType",
                table: "PlayerTrades");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTalismans_ViewerId",
                table: "PlayerTalismans");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSummonTickets_ViewerId",
                table: "PlayerSummonTickets");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSummonHistory_ViewerId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerStoryState",
                table: "PlayerStoryState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerShopInfos",
                table: "PlayerShopInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSetUnit",
                table: "PlayerSetUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_AccountId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuestWalls",
                table: "PlayerQuestWalls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPurchases",
                table: "PlayerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPresentHistory_ViewerId",
                table: "PlayerPresentHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPresent_ViewerId",
                table: "PlayerPresent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPassiveAbilities",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropIndex(
                name: "IX_PlayerPartyUnits_ViewerId_PartyNo",
                table: "PlayerPartyUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerMaterial",
                table: "PlayerMaterial");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerFortDetail",
                table: "PlayerFortDetail");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFortBuilds_ViewerId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventRewards",
                table: "PlayerEventRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventPassives",
                table: "PlayerEventPassives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventItems",
                table: "PlayerEventItems");

            migrationBuilder.DropIndex(
                name: "IX_PlayerEventItems_ViewerId_EventId",
                table: "PlayerEventItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerEventData",
                table: "PlayerEventData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonReliability",
                table: "PlayerDragonReliability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDragonGift",
                table: "PlayerDragonGift");

            migrationBuilder.DropIndex(
                name: "IX_PlayerDragonData_ViewerId",
                table: "PlayerDragonData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeServitorPassives",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeInfos",
                table: "PlayerDmodeInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeExpeditions",
                table: "PlayerDmodeExpeditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeDungeons",
                table: "PlayerDmodeDungeons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerDmodeCharas",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerBannerData",
                table: "PlayerBannerData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAbilityCrestSets",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerAbilityCrests",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyPowers",
                table: "PartyPowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyData",
                table: "PartyData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginBonuses",
                table: "LoginBonuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquippedStamps",
                table: "EquippedStamps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emblems",
                table: "Emblems");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "TimeAttackPlayers");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "TimeAttackClearUnits");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "ReceivedRankingTierRewards");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "QuestTreasureList");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "QuestEvents");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "QuestClearPartyUnits");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerWeaponSkins");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerWeapons");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerUseItems");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerTrades");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerTalismans");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerSummonTickets");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerSummonHistory");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerStoryState");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerShopInfos");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerSetUnit");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerQuestWalls");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerQuests");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerPurchases");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerPresentHistory");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerPresent");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerPassiveAbilities");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerPartyUnits");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerMissions");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerMaterial");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerFortDetail");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerFortBuilds");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerEventRewards");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerEventPassives");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerEventItems");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerEventData");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDragonReliability");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDragonGift");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDragonData");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDmodeServitorPassives");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDmodeInfos");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDmodeExpeditions");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDmodeDungeons");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerDmodeCharas");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerCharaData");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerBannerData");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerAbilityCrestSets");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PlayerAbilityCrests");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PartyPowers");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "PartyData");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "LoginBonuses");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "EquippedStamps");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "Emblems");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "TimeAttackPlayers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "TimeAttackClearUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "ReceivedRankingTierRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "QuestTreasureList",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "QuestEvents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "QuestClearPartyUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerWeaponSkins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerWeapons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ViewerId",
                table: "PlayerUserData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerUserData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerUseItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerTrades",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerTalismans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerSummonTickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerSummonHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerStoryState",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerShopInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerSetUnit",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerQuestWalls",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerQuests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerPurchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerPresentHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerPresent",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerPassiveAbilities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerPartyUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerMissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerMaterial",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerFortDetail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerFortBuilds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerEventRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerEventPassives",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerEventItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerEventData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDragonReliability",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDragonGift",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDragonData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDmodeServitorPassives",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDmodeInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDmodeExpeditions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDmodeDungeons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerDmodeCharas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerCharaData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerBannerData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerAbilityCrestSets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PlayerAbilityCrests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PartyPowers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "PartyData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "LoginBonuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "EquippedStamps",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAccountId",
                table: "Emblems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeAttackPlayers",
                table: "TimeAttackPlayers",
                columns: new[] { "GameId", "DeviceAccountId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeAttackClearUnits",
                table: "TimeAttackClearUnits",
                columns: new[] { "GameId", "DeviceAccountId", "UnitNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceivedRankingTierRewards",
                table: "ReceivedRankingTierRewards",
                columns: new[] { "DeviceAccountId", "RewardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestTreasureList",
                table: "QuestTreasureList",
                columns: new[] { "DeviceAccountId", "QuestTreasureId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestEvents",
                table: "QuestEvents",
                columns: new[] { "DeviceAccountId", "QuestEventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestClearPartyUnits",
                table: "QuestClearPartyUnits",
                columns: new[] { "DeviceAccountId", "QuestId", "IsMulti", "UnitNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerWeaponSkins",
                table: "PlayerWeaponSkins",
                columns: new[] { "DeviceAccountId", "WeaponSkinId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerWeapons",
                table: "PlayerWeapons",
                columns: new[] { "DeviceAccountId", "WeaponBodyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerUserData",
                table: "PlayerUserData",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerUseItems",
                table: "PlayerUseItems",
                columns: new[] { "DeviceAccountId", "ItemId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerTrades",
                table: "PlayerTrades",
                columns: new[] { "DeviceAccountId", "TradeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerStoryState",
                table: "PlayerStoryState",
                columns: new[] { "DeviceAccountId", "StoryType", "StoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerShopInfos",
                table: "PlayerShopInfos",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSetUnit",
                table: "PlayerSetUnit",
                columns: new[] { "DeviceAccountId", "CharaId", "UnitSetNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuestWalls",
                table: "PlayerQuestWalls",
                columns: new[] { "DeviceAccountId", "WallId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests",
                columns: new[] { "DeviceAccountId", "QuestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPurchases",
                table: "PlayerPurchases",
                columns: new[] { "DeviceAccountId", "GoodsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPassiveAbilities",
                table: "PlayerPassiveAbilities",
                columns: new[] { "DeviceAccountId", "WeaponPassiveAbilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMissions",
                table: "PlayerMissions",
                columns: new[] { "DeviceAccountId", "MissionId", "Type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerMaterial",
                table: "PlayerMaterial",
                columns: new[] { "DeviceAccountId", "MaterialId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerFortDetail",
                table: "PlayerFortDetail",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventRewards",
                table: "PlayerEventRewards",
                columns: new[] { "DeviceAccountId", "EventId", "RewardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventPassives",
                table: "PlayerEventPassives",
                columns: new[] { "DeviceAccountId", "EventId", "PassiveId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventItems",
                table: "PlayerEventItems",
                columns: new[] { "DeviceAccountId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerEventData",
                table: "PlayerEventData",
                columns: new[] { "DeviceAccountId", "EventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonReliability",
                table: "PlayerDragonReliability",
                columns: new[] { "DeviceAccountId", "DragonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDragonGift",
                table: "PlayerDragonGift",
                columns: new[] { "DeviceAccountId", "DragonGiftId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeServitorPassives",
                table: "PlayerDmodeServitorPassives",
                columns: new[] { "DeviceAccountId", "PassiveId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeInfos",
                table: "PlayerDmodeInfos",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeExpeditions",
                table: "PlayerDmodeExpeditions",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeDungeons",
                table: "PlayerDmodeDungeons",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerDmodeCharas",
                table: "PlayerDmodeCharas",
                columns: new[] { "DeviceAccountId", "CharaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerCharaData",
                table: "PlayerCharaData",
                columns: new[] { "DeviceAccountId", "CharaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerBannerData",
                table: "PlayerBannerData",
                columns: new[] { "DeviceAccountId", "SummonBannerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAbilityCrestSets",
                table: "PlayerAbilityCrestSets",
                columns: new[] { "DeviceAccountId", "AbilityCrestSetNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerAbilityCrests",
                table: "PlayerAbilityCrests",
                columns: new[] { "DeviceAccountId", "AbilityCrestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyPowers",
                table: "PartyPowers",
                column: "DeviceAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyData",
                table: "PartyData",
                columns: new[] { "DeviceAccountId", "PartyNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginBonuses",
                table: "LoginBonuses",
                columns: new[] { "DeviceAccountId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquippedStamps",
                table: "EquippedStamps",
                columns: new[] { "DeviceAccountId", "Slot" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emblems",
                table: "Emblems",
                columns: new[] { "DeviceAccountId", "EmblemId" });

            migrationBuilder.CreateTable(
                name: "PlayerCurrency",
                columns: table => new
                {
                    DeviceAccountId = table.Column<string>(type: "text", nullable: false),
                    CurrencyType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCurrency", x => new { x.DeviceAccountId, x.CurrencyType });
                    table.ForeignKey(
                        name: "FK_PlayerCurrency_Players_DeviceAccountId",
                        column: x => x.DeviceAccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeAttackPlayers_DeviceAccountId",
                table: "TimeAttackPlayers",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedRankingTierRewards_DeviceAccountId",
                table: "ReceivedRankingTierRewards",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestTreasureList_DeviceAccountId",
                table: "QuestTreasureList",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestEvents_DeviceAccountId",
                table: "QuestEvents",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerWeaponSkins_DeviceAccountId",
                table: "PlayerWeaponSkins",
                column: "DeviceAccountId");

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
                name: "IX_PlayerUseItems_DeviceAccountId",
                table: "PlayerUseItems",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrades_DeviceAccountId",
                table: "PlayerTrades",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrades_DeviceAccountId_TradeType",
                table: "PlayerTrades",
                columns: new[] { "DeviceAccountId", "TradeType" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTalismans_DeviceAccountId",
                table: "PlayerTalismans",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSummonTickets_DeviceAccountId",
                table: "PlayerSummonTickets",
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
                name: "IX_PlayerQuestWalls_DeviceAccountId",
                table: "PlayerQuestWalls",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_DeviceAccountId",
                table: "PlayerQuests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPurchases_DeviceAccountId",
                table: "PlayerPurchases",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresentHistory_DeviceAccountId",
                table: "PlayerPresentHistory",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPresent_DeviceAccountId",
                table: "PlayerPresent",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPassiveAbilities_DeviceAccountId",
                table: "PlayerPassiveAbilities",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId",
                table: "PlayerPartyUnits",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyUnits_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "DeviceAccountId", "PartyNo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMaterial_DeviceAccountId",
                table: "PlayerMaterial",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortDetail_DeviceAccountId",
                table: "PlayerFortDetail",
                column: "DeviceAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFortBuilds_DeviceAccountId",
                table: "PlayerFortBuilds",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventRewards_DeviceAccountId_EventId",
                table: "PlayerEventRewards",
                columns: new[] { "DeviceAccountId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventPassives_DeviceAccountId_EventId",
                table: "PlayerEventPassives",
                columns: new[] { "DeviceAccountId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventItems_DeviceAccountId_EventId",
                table: "PlayerEventItems",
                columns: new[] { "DeviceAccountId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerEventData_DeviceAccountId",
                table: "PlayerEventData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonReliability_DeviceAccountId",
                table: "PlayerDragonReliability",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonGift_DeviceAccountId",
                table: "PlayerDragonGift",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDragonData_DeviceAccountId",
                table: "PlayerDragonData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDmodeServitorPassives_DeviceAccountId",
                table: "PlayerDmodeServitorPassives",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDmodeCharas_DeviceAccountId",
                table: "PlayerDmodeCharas",
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
                name: "IX_PlayerAbilityCrestSets_DeviceAccountId",
                table: "PlayerAbilityCrestSets",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAbilityCrests_DeviceAccountId",
                table: "PlayerAbilityCrests",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyData_DeviceAccountId",
                table: "PartyData",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EquippedStamps_DeviceAccountId",
                table: "EquippedStamps",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Emblems_DeviceAccountId",
                table: "Emblems",
                column: "DeviceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCurrency_DeviceAccountId",
                table: "PlayerCurrency",
                column: "DeviceAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emblems_Players_DeviceAccountId",
                table: "Emblems",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquippedStamps_Players_DeviceAccountId",
                table: "EquippedStamps",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginBonuses_Players_DeviceAccountId",
                table: "LoginBonuses",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyData_Players_DeviceAccountId",
                table: "PartyData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPowers_Players_DeviceAccountId",
                table: "PartyPowers",
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
                name: "FK_PlayerAbilityCrestSets_Players_DeviceAccountId",
                table: "PlayerAbilityCrestSets",
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
                name: "FK_PlayerDmodeCharas_Players_DeviceAccountId",
                table: "PlayerDmodeCharas",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeDungeons_Players_DeviceAccountId",
                table: "PlayerDmodeDungeons",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeExpeditions_Players_DeviceAccountId",
                table: "PlayerDmodeExpeditions",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeInfos_Players_DeviceAccountId",
                table: "PlayerDmodeInfos",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerDmodeServitorPassives_Players_DeviceAccountId",
                table: "PlayerDmodeServitorPassives",
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
                name: "FK_PlayerDragonGift_Players_DeviceAccountId",
                table: "PlayerDragonGift",
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
                name: "FK_PlayerEventData_Players_DeviceAccountId",
                table: "PlayerEventData",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventItems_Players_DeviceAccountId",
                table: "PlayerEventItems",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventPassives_Players_DeviceAccountId",
                table: "PlayerEventPassives",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerEventRewards_Players_DeviceAccountId",
                table: "PlayerEventRewards",
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
                name: "FK_PlayerFortDetail_Players_DeviceAccountId",
                table: "PlayerFortDetail",
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
                name: "FK_PlayerMissions_Players_DeviceAccountId",
                table: "PlayerMissions",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPartyUnits_PartyData_DeviceAccountId_PartyNo",
                table: "PlayerPartyUnits",
                columns: new[] { "DeviceAccountId", "PartyNo" },
                principalTable: "PartyData",
                principalColumns: new[] { "DeviceAccountId", "PartyNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPassiveAbilities_Players_DeviceAccountId",
                table: "PlayerPassiveAbilities",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPresent_Players_DeviceAccountId",
                table: "PlayerPresent",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPresentHistory_Players_DeviceAccountId",
                table: "PlayerPresentHistory",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPurchases_Players_DeviceAccountId",
                table: "PlayerPurchases",
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
                name: "FK_PlayerQuestWalls_Players_DeviceAccountId",
                table: "PlayerQuestWalls",
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
                name: "FK_PlayerShopInfos_Players_DeviceAccountId",
                table: "PlayerShopInfos",
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
                name: "FK_PlayerSummonTickets_Players_DeviceAccountId",
                table: "PlayerSummonTickets",
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
                name: "FK_PlayerTrades_Players_DeviceAccountId",
                table: "PlayerTrades",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUseItems_Players_DeviceAccountId",
                table: "PlayerUseItems",
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

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerWeaponSkins_Players_DeviceAccountId",
                table: "PlayerWeaponSkins",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestClearPartyUnits_Players_DeviceAccountId",
                table: "QuestClearPartyUnits",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestEvents_Players_DeviceAccountId",
                table: "QuestEvents",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestTreasureList_Players_DeviceAccountId",
                table: "QuestTreasureList",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedRankingTierRewards_Players_DeviceAccountId",
                table: "ReceivedRankingTierRewards",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackClearUnits_TimeAttackPlayers_GameId_DeviceAccount~",
                table: "TimeAttackClearUnits",
                columns: new[] { "GameId", "DeviceAccountId" },
                principalTable: "TimeAttackPlayers",
                principalColumns: new[] { "GameId", "DeviceAccountId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeAttackPlayers_Players_DeviceAccountId",
                table: "TimeAttackPlayers",
                column: "DeviceAccountId",
                principalTable: "Players",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
