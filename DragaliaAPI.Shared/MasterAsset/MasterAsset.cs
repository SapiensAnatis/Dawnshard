using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.Login;
using DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;
using DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;
using DragaliaAPI.Shared.MasterAsset.Models.QuestSchedule;
using DragaliaAPI.Shared.MasterAsset.Models.Shop;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using DragaliaAPI.Shared.MasterAsset.Models.User;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;

namespace DragaliaAPI.Shared.MasterAsset;

/// <summary>
/// Provides access to instances of <see cref="MasterAssetData{TKey,TItem}"/> to retrieve internal game data.
/// </summary>
public static class MasterAsset
{
    /// <summary>
    /// Contains information about characters.
    /// </summary>
    public static MasterAssetData<Charas, CharaData> CharaData { get; } =
        new("CharaData.json", x => x.Id);

    /// <summary>
    /// Contains information about dragons.
    /// </summary>
    public static MasterAssetData<Dragons, DragonData> DragonData { get; } =
        new("DragonData.json", x => x.Id);

    public static MasterAssetData<int, DragonRarity> DragonRarity { get; } =
        new("DragonRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about quests.
    /// </summary>
    public static MasterAssetData<int, QuestData> QuestData { get; } =
        new("QuestData.json", x => x.Id);

    public static MasterAssetData<Materials, MaterialData> MaterialData { get; } =
        new("MaterialData.json", x => x.Id);

    /// <summary>
    /// Contains information about Halidom buildings.
    /// </summary>
    /// <remarks>
    /// To generate keys, use <see cref="MasterAssetUtils.GetPlantDetailId"/>
    /// </remarks>
    public static MasterAssetData<int, FortPlantDetail> FortPlant { get; } =
        new("FortPlantDetail.json", x => x.Id);

    /// <summary>
    /// Contains information about weapons.
    /// </summary>
    public static MasterAssetData<WeaponBodies, WeaponBody> WeaponBody { get; } =
        new("WeaponBody.json", x => x.Id);

    /// <summary>
    /// Contains information about miscellaneous weapon upgrade steps, i.e. anything not passive abilities or stat upgrades.
    /// </summary>
    public static MasterAssetData<int, WeaponBodyBuildupGroup> WeaponBodyBuildupGroup { get; } =
        new("WeaponBodyBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about stat weapon upgrade steps.
    /// </summary>
    public static MasterAssetData<int, WeaponBodyBuildupLevel> WeaponBodyBuildupLevel { get; } =
        new("WeaponBodyBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about weapon passive ability unlocks.
    /// </summary>
    public static MasterAssetData<int, WeaponPassiveAbility> WeaponPassiveAbility { get; } =
        new("WeaponPassiveAbility.json", x => x.Id);

    public static MasterAssetData<int, WeaponBodyRarity> WeaponBodyRarity { get; } =
        new("WeaponBodyRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about weapons.
    /// </summary>
    public static MasterAssetData<int, WeaponSkin> WeaponSkin { get; } =
        new("WeaponSkin.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to unbind ability crests.
    /// </summary>
    public static MasterAssetData<int, AbilityCrestBuildupGroup> AbilityCrestBuildupGroup { get; } =
        new("AbilityCrestBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to level up ability crests.
    /// </summary>
    public static MasterAssetData<int, AbilityCrestBuildupLevel> AbilityCrestBuildupLevel { get; } =
        new("AbilityCrestBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about the level limits of different ability crest rarities.
    /// </summary>
    public static MasterAssetData<int, AbilityCrestRarity> AbilityCrestRarity { get; } =
        new("AbilityCrestRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about ability crests.
    /// </summary>
    public static MasterAssetData<AbilityCrests, AbilityCrest> AbilityCrest { get; } =
        new("AbilityCrest.json", x => x.Id);

    public static MasterAssetData<int, QuestEventGroup> QuestEventGroup { get; } =
        new("QuestEventGroup.json", x => x.Id);

    public static MasterAssetData<int, QuestEvent> QuestEvent { get; } =
        new("QuestEvent.json", x => x.Id);

    public static MasterAssetData<int, QuestTreasureData> QuestTreasureData { get; } =
        new("QuestTreasureData.json", x => x.Id);

    public static MasterAssetData<UseItem, UseItemData> UseItem { get; } =
        new("UseItem.json", x => x.Id);

    public static MasterAssetData<int, AbilityData> AbilityData { get; } =
        new("AbilityData.json", x => x.Id);

    public static MasterAssetData<int, AbilityLimitedGroup> AbilityLimitedGroup { get; } =
        new("AbilityLimitedGroup.json", x => x.Id);

    public static MasterAssetData<int, ExAbilityData> ExAbilityData { get; } =
        new("ExAbilityData.json", x => x.Id);

    public static MasterAssetData<int, UnionAbility> UnionAbility { get; } =
        new("UnionAbility.json", x => x.Id);

    public static MasterAssetData<int, SkillData> SkillData { get; } =
        new("SkillData.json", x => x.Id);

    #region Missions

    public static MasterAssetData<int, AlbumMission> AlbumMission { get; } =
        new("Missions/MissionAlbumData.json", x => x.Id);

    public static MasterAssetData<int, NormalMission> BeginnerMission { get; } =
        new("Missions/MissionBeginnerData.json", x => x.Id);

    public static MasterAssetData<int, DailyMission> DailyMission { get; } =
        new("Missions/MissionDailyData.json", x => x.Id);

    public static MasterAssetData<int, DrillMission> DrillMission { get; } =
        new("Missions/MissionDrillData.json", x => x.Id);

    public static MasterAssetData<int, DrillMissionGroup> DrillMissionGroup { get; } =
        new("Missions/MissionDrillGroup.json", x => x.Id);

    public static MasterAssetData<int, MainStoryMission> MainStoryMission { get; } =
        new("Missions/MissionMainStoryData.json", x => x.Id);

    public static MasterAssetData<int, MainStoryMissionGroup> MainStoryMissionGroup { get; } =
        new("Missions/MissionMainStoryGroup.json", x => x.Id);

    public static MasterAssetData<int, MemoryEventMission> MemoryEventMission { get; } =
        new("Missions/MissionMemoryEventData.json", x => x.Id);

    public static MasterAssetData<int, NormalMission> NormalMission { get; } =
        new("Missions/MissionNormalData.json", x => x.Id);

    public static MasterAssetData<int, PeriodMission> PeriodMission { get; } =
        new("Missions/MissionPeriodData.json", x => x.Id);

    public static MasterAssetData<int, SpecialMission> SpecialMission { get; } =
        new("Missions/MissionSpecialData.json", x => x.Id);

    public static MasterAssetData<int, SpecialMissionGroup> SpecialMissionGroup { get; } =
        new("Missions/MissionSpecialGroup.json", x => x.Id);

    public static MasterAssetData<int, MissionProgressionInfo> MissionProgressionInfo { get; } =
        new("Missions/MissionProgressionInfo.json", x => x.Id);

    public static MasterAssetData<
        int,
        MainStoryMissionGroupRewards
    > MainStoryMissionGroupRewards = new("Missions/MainStoryMissionGroupRewards.json", x => x.Id);

    #endregion

    #region Stamps

    /// <summary>
    /// Contains information about stickers.
    /// </summary>
    public static MasterAssetData<int, Stamp> StampData = new("StampData.json", x => x.Id);

    #endregion

    #region Shops

    public static MasterAssetData<int, NormalShop> NormalShop { get; } =
        new("Shop/NormalShop.json", x => x.Id);

    public static MasterAssetData<int, SpecialShop> SpecialShop { get; } =
        new("Shop/SpecialShop.json", x => x.Id);

    public static MasterAssetData<int, MaterialShop> MaterialShopDaily { get; } =
        new("Shop/MaterialShopDaily.json", x => x.Id);

    public static MasterAssetData<int, MaterialShop> MaterialShopWeekly { get; } =
        new("Shop/MaterialShopWeekly.json", x => x.Id);

    public static MasterAssetData<int, MaterialShop> MaterialShopMonthly { get; } =
        new("Shop/MaterialShopMonthly.json", x => x.Id);

    #endregion

    #region Treasure Trade / Wyrmprint Trade

    /// <summary>
    /// Contains information about ability crests in the shop.
    /// </summary>
    public static MasterAssetData<int, AbilityCrestTrade> AbilityCrestTrade { get; } =
        new("Trade/AbilityCrestTrade.json", x => x.Id);

    public static MasterAssetData<int, TreasureTrade> TreasureTrade { get; } =
        new("Trade/TreasureTrade.json", x => x.Id);

    public static MasterAssetData<int, TreasureTrade> EventTreasureTrade { get; } =
        new("Trade/EventTreasureTradeInfo.json", x => x.Id);

    #endregion

    #region Login Bonus

    public static MasterAssetData<int, LoginBonusData> LoginBonusData { get; } =
        new("Login/LoginBonusData.json", x => x.Id);

    public static MasterAssetData<int, LoginBonusReward> LoginBonusReward { get; } =
        new("Login/LoginBonusReward.json", x => x.Id);

    #endregion

    #region Mana Circles

    /// <summary>
    /// Contains information about mana circle nodes.
    /// </summary>
    public static MasterAssetData<int, ManaNode> ManaNode { get; } =
        new("ManaCircle/MC.json", x => x.MC_0);

    public static MasterAssetData<int, ManaPieceMaterial> ManaPieceMaterial { get; } =
        new("ManaCircle/ManaPieceMaterial.json", x => x.Id);

    public static MasterAssetData<ManaNodeTypes, ManaPieceType> ManaPieceType { get; } =
        new("ManaCircle/ManaPieceType.json", x => x.Id);

    public static MasterAssetData<int, CharaLimitBreak> CharaLimitBreak { get; } =
        new("ManaCircle/CharaLimitBreak.json", x => x.Id);

    #endregion

    #region Story

    /// <summary>
    /// Dragon StoryId Arrays indexed by DragonId
    /// </summary>
    public static MasterAssetData<int, StoryData> DragonStories { get; } =
        new("Story/DragonStories.json", x => x.id);

    /// <summary>
    /// Character StoryId Arrays indexed by CharaId
    /// </summary>
    public static MasterAssetData<int, StoryData> CharaStories { get; } =
        new("Story/CharaStories.json", x => x.id);

    public static MasterAssetData<int, UnitStory> UnitStory { get; } =
        new("Story/UnitStory.json", x => x.Id);

    public static MasterAssetData<int, QuestStory> QuestStory { get; } =
        new("Story/QuestStory.json", x => x.Id);

    public static MasterAssetData<int, EventStory> EventStory { get; } =
        new("Story/EventStory.json", x => x.Id);

    public static MasterAssetData<int, QuestStoryRewardInfo> QuestStoryRewardInfo { get; } =
        new("Story/QuestStoryRewardInfo.json", x => x.Id);

    #endregion

    #region Quest Drops

    /// <summary>
    /// Contains information about the <see cref="Models.QuestDrops.EnemyParam"/> IDs in particular quest maps.
    /// </summary>
    public static MasterAssetData<string, QuestEnemies> QuestEnemies { get; } =
        new("QuestDrops/QuestEnemies.json", x => x.AreaName);

    /// <summary>
    /// Contains information about instances of enemies within a quest.
    /// </summary>
    public static MasterAssetData<int, EnemyParam> EnemyParam { get; } =
        new("QuestDrops/EnemyParam.json", x => x.Id);

    /// <summary>
    /// Contains information about rewards from quests.
    /// </summary>
    /// <remarks>
    /// Generated from parsing wiki Cargo data.
    /// </remarks>
    public static MasterAssetData<int, QuestDropInfo> QuestDrops { get; } =
        new("QuestDrops/QuestDrops.json", x => x.QuestId);

    /// <summary>
    /// Contains information about bonus rewards from quests.
    /// </summary>
    /// <remarks>
    /// Generated from parsing wiki Cargo data.
    /// </remarks>
    public static MasterAssetData<int, QuestBonusReward> QuestBonusRewards { get; } =
        new("QuestDrops/QuestBonusRewards.json", x => x.QuestId);

    #endregion

    #region Quest Rewards

    public static MasterAssetData<int, QuestRewardData> QuestRewardData { get; } =
        new("QuestRewards/QuestRewardData.json", x => x.Id);

    public static MasterAssetData<
        int,
        QuestScoreMissionRewardInfo
    > QuestScoreMissionRewardInfo = new("QuestRewards/QuestScoreMissionRewardInfo.json", x => x.Id);

    public static MasterAssetData<int, QuestScoreMissionData> QuestScoreMissionData { get; } =
        new("QuestRewards/QuestScoreMissionData.json", x => x.Id);

    #endregion

    #region Events

    public static MasterAssetData<int, EventData> EventData { get; } =
        new("Event/EventData.json", x => x.Id);

    public static MasterAssetData<int, EventTradeGroup> EventTradeGroup { get; } =
        new("Event/EventTradeGroup.json", x => x.Id);

    public static MasterAssetGroup<int, int, BuildEventReward> BuildEventReward { get; } =
        new("Event/BuildEventReward.json", x => x.Id);

    public static MasterAssetGroup<int, int, RaidEventReward> RaidEventReward { get; } =
        new("Event/RaidEventReward.json", x => x.Id);

    public static MasterAssetData<int, CombatEventLocation> CombatEventLocation { get; } =
        new("Event/CombatEventLocation.json", x => x.Id);

    public static MasterAssetData<
        int,
        CombatEventLocationReward
    > CombatEventLocationReward = new("Event/CombatEventLocationReward.json", x => x.Id);

    public static MasterAssetData<int, EventItem<BuildEventItemType>> BuildEventItem { get; } =
        new("Event/BuildEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<CombatEventItemType>> CombatEventItem { get; } =
        new("Event/CombatEventItem.json", x => x.Id);

    public static MasterAssetData<int, RaidEventItem> RaidEventItem { get; } =
        new("Event/RaidEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<SimpleEventItemType>> SimpleEventItem { get; } =
        new("Event/SimpleEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<ExRushEventItemType>> ExRushEventItem { get; } =
        new("Event/ExRushEventItem.json", x => x.Id);

    public static MasterAssetData<
        int,
        EventItem<ExHunterEventItemType>
    > ExHunterEventItem = new("Event/ExHunterEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<EarnEventItemType>> EarnEventItem { get; } =
        new("Event/EarnEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<CollectEventItemType>> CollectEventItem { get; } =
        new("Event/CollectEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventItem<Clb01EventItemType>> Clb01EventItem { get; } =
        new("Event/Clb01EventItem.json", x => x.Id);

    public static MasterAssetData<
        int,
        EventItem<BattleRoyalEventItemType>
    > BattleRoyalEventItem = new("Event/BattleRoyalEventItem.json", x => x.Id);

    public static MasterAssetData<int, EventPassive> EventPassive { get; } =
        new("Event/EventPassive.json", x => x.Id);

    #endregion

    #region Dmode

    public static MasterAssetData<int, DmodeQuestFloor> DmodeQuestFloor { get; } =
        new("Dmode/DmodeQuestFloor.json", x => x.Id);

    public static MasterAssetData<int, DmodeDungeonArea> DmodeDungeonArea { get; } =
        new("Dmode/DmodeDungeonArea.json", x => x.Id);

    public static MasterAssetData<int, DmodeDungeonTheme> DmodeDungeonTheme { get; } =
        new("Dmode/DmodeDungeonTheme.json", x => x.Id);

    public static MasterAssetData<int, DmodeEnemyTheme> DmodeEnemyTheme { get; } =
        new("Dmode/DmodeEnemyTheme.json", x => x.Id);

    public static MasterAssetData<string, DmodeAreaInfo> DmodeAreaInfo { get; } =
        new("Dmode/DmodeAreaInfo.json", x => x.AreaName);

    public static MasterAssetData<int, DmodeEnemyParam> DmodeEnemyParam { get; } =
        new("Dmode/DmodeEnemyParam.json", x => x.Id);

    public static MasterAssetData<int, DmodeCharaLevel> DmodeCharaLevel { get; } =
        new("Dmode/DmodeCharaLevel.json", x => x.Id);

    public static MasterAssetData<int, DmodeWeapon> DmodeWeapon { get; } =
        new("Dmode/DmodeWeapon.json", x => x.Id);

    public static MasterAssetData<int, DmodeAbilityCrest> DmodeAbilityCrest { get; } =
        new("Dmode/DmodeAbilityCrest.json", x => x.Id);

    public static MasterAssetData<int, DmodeStrengthParam> DmodeStrengthParam { get; } =
        new("Dmode/DmodeStrengthParam.json", x => x.Id);

    public static MasterAssetData<int, DmodeStrengthSkill> DmodeStrengthSkill { get; } =
        new("Dmode/DmodeStrengthSkill.json", x => x.Id);

    public static MasterAssetData<int, DmodeStrengthAbility> DmodeStrengthAbility { get; } =
        new("Dmode/DmodeStrengthAbility.json", x => x.Id);

    public static MasterAssetData<int, DmodeDungeonItemData> DmodeDungeonItemData { get; } =
        new("Dmode/DmodeDungeonItemData.json", x => x.Id);

    public static MasterAssetData<
        int,
        DmodeServitorPassiveLevel
    > DmodeServitorPassiveLevel = new("Dmode/DmodeServitorPassiveLevel.json", x => x.Id);

    public static MasterAssetData<int, DmodeExpeditionFloor> DmodeExpeditionFloor { get; } =
        new("Dmode/DmodeExpeditionFloor.json", x => x.Id);

    #endregion

    #region User

    public static MasterAssetData<int, UserLevel> UserLevel { get; } =
        new("User/UserLevel.json", x => x.Id);

    #endregion

    #region Quest Schedule

    public static MasterAssetData<int, QuestScheduleInfo> QuestScheduleInfo { get; } =
        new("QuestSchedule/QuestScheduleInfo.json", x => x.Id);

    #endregion

    #region Time Attack

    /// <summary>
    /// Contains information about the last two ranked quests in each Time Attack event.
    /// </summary>
    public static MasterAssetData<int, RankingData> RankingData { get; } =
        new("TimeAttack/RankingData.json", x => x.QuestId);

    /// <summary>
    /// Contains information about the initial non-ranked rewards for clearing Time Attack quests.
    /// </summary>
    public static MasterAssetData<int, RankingTierReward> RankingTierReward { get; } =
        new("TimeAttack/RankingTierReward.json", x => x.Id);

    #endregion

    #region Wall
    /// <summary>
    /// Contains information about Mercurial Gauntlet quests.
    /// </summary>
    public static MasterAssetData<int, QuestWallDetail> QuestWallDetail { get; } =
        new("Wall/QuestWallDetail.json", x => x.Id);

    /// <summary>
    /// Contains information about Mercurial Gauntlet monthly rewards.
    /// </summary>
    public static MasterAssetData<int, QuestWallMonthlyReward> QuestWallMonthlyReward { get; } =
        new("Wall/QuestWallMonthlyReward.json", x => x.TotalWallLevel);
    #endregion
}
