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
    public static readonly MasterAssetData<Charas, CharaData> CharaData =
        new("CharaData.json", x => x.Id);

    /// <summary>
    /// Contains information about dragons.
    /// </summary>
    public static readonly MasterAssetData<Dragons, DragonData> DragonData =
        new("DragonData.json", x => x.Id);

    public static readonly MasterAssetData<int, DragonRarity> DragonRarity =
        new("DragonRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about quests.
    /// </summary>
    public static readonly MasterAssetData<int, QuestData> QuestData =
        new("QuestData.json", x => x.Id);

    public static readonly MasterAssetData<Materials, MaterialData> MaterialData =
        new("MaterialData.json", x => x.Id);

    /// <summary>
    /// Contains information about Halidom buildings.
    /// </summary>
    /// <remarks>
    /// To generate keys, use <see cref="MasterAssetUtils.GetPlantDetailId"/>
    /// </remarks>
    public static readonly MasterAssetData<int, FortPlantDetail> FortPlant =
        new("FortPlantDetail.json", x => x.Id);

    /// <summary>
    /// Contains information about weapons.
    /// </summary>
    public static readonly MasterAssetData<WeaponBodies, WeaponBody> WeaponBody =
        new("WeaponBody.json", x => x.Id);

    /// <summary>
    /// Contains information about miscellaneous weapon upgrade steps, i.e. anything not passive abilities or stat upgrades.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponBodyBuildupGroup> WeaponBodyBuildupGroup =
        new("WeaponBodyBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about stat weapon upgrade steps.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponBodyBuildupLevel> WeaponBodyBuildupLevel =
        new("WeaponBodyBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about weapon passive ability unlocks.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponPassiveAbility> WeaponPassiveAbility =
        new("WeaponPassiveAbility.json", x => x.Id);

    public static readonly MasterAssetData<int, WeaponBodyRarity> WeaponBodyRarity =
        new("WeaponBodyRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about weapons.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponSkin> WeaponSkin =
        new("WeaponSkin.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to unbind ability crests.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestBuildupGroup> AbilityCrestBuildupGroup =
        new("AbilityCrestBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to level up ability crests.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestBuildupLevel> AbilityCrestBuildupLevel =
        new("AbilityCrestBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about the level limits of different ability crest rarities.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestRarity> AbilityCrestRarity =
        new("AbilityCrestRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about ability crests.
    /// </summary>
    public static readonly MasterAssetData<AbilityCrests, AbilityCrest> AbilityCrest =
        new("AbilityCrest.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestEventGroup> QuestEventGroup =
        new("QuestEventGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestEvent> QuestEvent =
        new("QuestEvent.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestTreasureData> QuestTreasureData =
        new("QuestTreasureData.json", x => x.Id);

    public static readonly MasterAssetData<UseItem, UseItemData> UseItem =
        new("UseItem.json", x => x.Id);

    public static readonly MasterAssetData<int, AbilityData> AbilityData =
        new("AbilityData.json", x => x.Id);

    public static readonly MasterAssetData<int, AbilityLimitedGroup> AbilityLimitedGroup =
        new("AbilityLimitedGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, ExAbilityData> ExAbilityData =
        new("ExAbilityData.json", x => x.Id);

    public static readonly MasterAssetData<int, UnionAbility> UnionAbility =
        new("UnionAbility.json", x => x.Id);

    public static readonly MasterAssetData<int, SkillData> SkillData =
        new("SkillData.json", x => x.Id);

    #region Missions

    public static readonly MasterAssetData<int, AlbumMission> AlbumMission =
        new("Missions/MissionAlbumData.json", x => x.Id);

    public static readonly MasterAssetData<int, NormalMission> BeginnerMission =
        new("Missions/MissionBeginnerData.json", x => x.Id);

    public static readonly MasterAssetData<int, DailyMission> DailyMission =
        new("Missions/MissionDailyData.json", x => x.Id);

    public static readonly MasterAssetData<int, DrillMission> DrillMission =
        new("Missions/MissionDrillData.json", x => x.Id);

    public static readonly MasterAssetData<int, DrillMissionGroup> DrillMissionGroup =
        new("Missions/MissionDrillGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, MainStoryMission> MainStoryMission =
        new("Missions/MissionMainStoryData.json", x => x.Id);

    public static readonly MasterAssetData<int, MainStoryMissionGroup> MainStoryMissionGroup =
        new("Missions/MissionMainStoryGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, MemoryEventMission> MemoryEventMission =
        new("Missions/MissionMemoryEventData.json", x => x.Id);

    public static readonly MasterAssetData<int, NormalMission> NormalMission =
        new("Missions/MissionNormalData.json", x => x.Id);

    public static readonly MasterAssetData<int, PeriodMission> PeriodMission =
        new("Missions/MissionPeriodData.json", x => x.Id);

    public static readonly MasterAssetData<int, SpecialMission> SpecialMission =
        new("Missions/MissionSpecialData.json", x => x.Id);

    public static readonly MasterAssetData<int, SpecialMissionGroup> SpecialMissionGroup =
        new("Missions/MissionSpecialGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, MissionProgressionInfo> MissionProgressionInfo =
        new("Missions/MissionProgressionInfo.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        MainStoryMissionGroupRewards
    > MainStoryMissionGroupRewards = new("Missions/MainStoryMissionGroupRewards.json", x => x.Id);

    #endregion

    #region Stamps

    /// <summary>
    /// Contains information about stickers.
    /// </summary>
    public static readonly MasterAssetData<int, Stamp> StampData = new("StampData.json", x => x.Id);

    #endregion

    #region Shops

    public static readonly MasterAssetData<int, NormalShop> NormalShop =
        new("Shop/NormalShop.json", x => x.Id);

    public static readonly MasterAssetData<int, SpecialShop> SpecialShop =
        new("Shop/SpecialShop.json", x => x.Id);

    public static readonly MasterAssetData<int, MaterialShop> MaterialShopDaily =
        new("Shop/MaterialShopDaily.json", x => x.Id);

    public static readonly MasterAssetData<int, MaterialShop> MaterialShopWeekly =
        new("Shop/MaterialShopWeekly.json", x => x.Id);

    public static readonly MasterAssetData<int, MaterialShop> MaterialShopMonthly =
        new("Shop/MaterialShopMonthly.json", x => x.Id);

    #endregion

    #region Treasure Trade / Wyrmprint Trade

    /// <summary>
    /// Contains information about ability crests in the shop.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestTrade> AbilityCrestTrade =
        new("Trade/AbilityCrestTrade.json", x => x.Id);

    public static readonly MasterAssetData<int, TreasureTrade> TreasureTrade =
        new("Trade/TreasureTrade.json", x => x.Id);

    public static readonly MasterAssetData<int, TreasureTrade> EventTreasureTrade =
        new("Trade/EventTreasureTradeInfo.json", x => x.Id);

    #endregion

    #region Login Bonus

    public static readonly MasterAssetData<int, LoginBonusData> LoginBonusData =
        new("Login/LoginBonusData.json", x => x.Id);

    public static readonly MasterAssetData<int, LoginBonusReward> LoginBonusReward =
        new("Login/LoginBonusReward.json", x => x.Id);

    #endregion

    #region Mana Circles

    /// <summary>
    /// Contains information about mana circle nodes.
    /// </summary>
    public static readonly MasterAssetData<int, ManaNode> ManaNode =
        new("ManaCircle/MC.json", x => x.MC_0);

    public static readonly MasterAssetData<int, ManaPieceMaterial> ManaPieceMaterial =
        new("ManaCircle/ManaPieceMaterial.json", x => x.Id);

    public static readonly MasterAssetData<ManaNodeTypes, ManaPieceType> ManaPieceType =
        new("ManaCircle/ManaPieceType.json", x => x.Id);

    public static readonly MasterAssetData<int, CharaLimitBreak> CharaLimitBreak =
        new("ManaCircle/CharaLimitBreak.json", x => x.Id);

    #endregion

    #region Story

    /// <summary>
    /// Dragon StoryId Arrays indexed by DragonId
    /// </summary>
    public static readonly MasterAssetData<int, StoryData> DragonStories =
        new("Story/DragonStories.json", x => x.id);

    /// <summary>
    /// Character StoryId Arrays indexed by CharaId
    /// </summary>
    public static readonly MasterAssetData<int, StoryData> CharaStories =
        new("Story/CharaStories.json", x => x.id);

    public static readonly MasterAssetData<int, UnitStory> UnitStory =
        new("Story/UnitStory.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestStory> QuestStory =
        new("Story/QuestStory.json", x => x.Id);

    public static readonly MasterAssetData<int, EventStory> EventStory =
        new("Story/EventStory.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestStoryRewardInfo> QuestStoryRewardInfo =
        new("Story/QuestStoryRewardInfo.json", x => x.Id);

    #endregion

    #region Quest Drops

    /// <summary>
    /// Contains information about the <see cref="Models.QuestDrops.EnemyParam"/> IDs in particular quest maps.
    /// </summary>
    public static readonly MasterAssetData<string, QuestEnemies> QuestEnemies =
        new("QuestDrops/QuestEnemies.json", x => x.AreaName);

    /// <summary>
    /// Contains information about instances of enemies within a quest.
    /// </summary>
    public static readonly MasterAssetData<int, EnemyParam> EnemyParam =
        new("QuestDrops/EnemyParam.json", x => x.Id);

    /// <summary>
    /// Contains information about rewards from quests.
    /// </summary>
    /// <remarks>
    /// Generated from parsing wiki Cargo data.
    /// </remarks>
    public static readonly MasterAssetData<int, QuestDropInfo> QuestDrops =
        new("QuestDrops/QuestDrops.json", x => x.QuestId);

    /// <summary>
    /// Contains information about bonus rewards from quests.
    /// </summary>
    /// <remarks>
    /// Generated from parsing wiki Cargo data.
    /// </remarks>
    public static readonly MasterAssetData<int, QuestBonusReward> QuestBonusRewards =
        new("QuestDrops/QuestBonusRewards.json", x => x.QuestId);

    #endregion

    #region Quest Rewards

    public static readonly MasterAssetData<int, QuestRewardData> QuestRewardData =
        new("QuestRewards/QuestRewardData.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        QuestScoreMissionRewardInfo
    > QuestScoreMissionRewardInfo = new("QuestRewards/QuestScoreMissionRewardInfo.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestScoreMissionData> QuestScoreMissionData =
        new("QuestRewards/QuestScoreMissionData.json", x => x.Id);

    #endregion

    #region Events

    public static readonly MasterAssetData<int, EventData> EventData =
        new("Event/EventData.json", x => x.Id);

    public static readonly MasterAssetData<int, EventTradeGroup> EventTradeGroup =
        new("Event/EventTradeGroup.json", x => x.Id);

    public static readonly MasterAssetGroup<int, int, BuildEventReward> BuildEventReward =
        new("Event/BuildEventReward.json", x => x.Id);

    public static readonly MasterAssetGroup<int, int, RaidEventReward> RaidEventReward =
        new("Event/RaidEventReward.json", x => x.Id);

    public static readonly MasterAssetData<int, CombatEventLocation> CombatEventLocation =
        new("Event/CombatEventLocation.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        CombatEventLocationReward
    > CombatEventLocationReward = new("Event/CombatEventLocationReward.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<BuildEventItemType>> BuildEventItem =
        new("Event/BuildEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<CombatEventItemType>> CombatEventItem =
        new("Event/CombatEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, RaidEventItem> RaidEventItem =
        new("Event/RaidEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<SimpleEventItemType>> SimpleEventItem =
        new("Event/SimpleEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<ExRushEventItemType>> ExRushEventItem =
        new("Event/ExRushEventItem.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        EventItem<ExHunterEventItemType>
    > ExHunterEventItem = new("Event/ExHunterEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<EarnEventItemType>> EarnEventItem =
        new("Event/EarnEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<CollectEventItemType>> CollectEventItem =
        new("Event/CollectEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventItem<Clb01EventItemType>> Clb01EventItem =
        new("Event/Clb01EventItem.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        EventItem<BattleRoyalEventItemType>
    > BattleRoyalEventItem = new("Event/BattleRoyalEventItem.json", x => x.Id);

    public static readonly MasterAssetData<int, EventPassive> EventPassive =
        new("Event/EventPassive.json", x => x.Id);

    #endregion

    #region Dmode

    public static readonly MasterAssetData<int, DmodeQuestFloor> DmodeQuestFloor =
        new("Dmode/DmodeQuestFloor.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeDungeonArea> DmodeDungeonArea =
        new("Dmode/DmodeDungeonArea.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeDungeonTheme> DmodeDungeonTheme =
        new("Dmode/DmodeDungeonTheme.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeEnemyTheme> DmodeEnemyTheme =
        new("Dmode/DmodeEnemyTheme.json", x => x.Id);

    public static readonly MasterAssetData<string, DmodeAreaInfo> DmodeAreaInfo =
        new("Dmode/DmodeAreaInfo.json", x => x.AreaName);

    public static readonly MasterAssetData<int, DmodeEnemyParam> DmodeEnemyParam =
        new("Dmode/DmodeEnemyParam.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeCharaLevel> DmodeCharaLevel =
        new("Dmode/DmodeCharaLevel.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeWeapon> DmodeWeapon =
        new("Dmode/DmodeWeapon.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeAbilityCrest> DmodeAbilityCrest =
        new("Dmode/DmodeAbilityCrest.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeStrengthParam> DmodeStrengthParam =
        new("Dmode/DmodeStrengthParam.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeStrengthSkill> DmodeStrengthSkill =
        new("Dmode/DmodeStrengthSkill.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeStrengthAbility> DmodeStrengthAbility =
        new("Dmode/DmodeStrengthAbility.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeDungeonItemData> DmodeDungeonItemData =
        new("Dmode/DmodeDungeonItemData.json", x => x.Id);

    public static readonly MasterAssetData<
        int,
        DmodeServitorPassiveLevel
    > DmodeServitorPassiveLevel = new("Dmode/DmodeServitorPassiveLevel.json", x => x.Id);

    public static readonly MasterAssetData<int, DmodeExpeditionFloor> DmodeExpeditionFloor =
        new("Dmode/DmodeExpeditionFloor.json", x => x.Id);

    #endregion

    #region User

    public static readonly MasterAssetData<int, UserLevel> UserLevel =
        new("User/UserLevel.json", x => x.Id);

    #endregion

    #region Quest Schedule

    public static readonly MasterAssetData<int, QuestScheduleInfo> QuestScheduleInfo =
        new("QuestSchedule/QuestScheduleInfo.json", x => x.Id);

    #endregion

    #region Time Attack

    /// <summary>
    /// Contains information about the last two ranked quests in each Time Attack event.
    /// </summary>
    public static readonly MasterAssetData<int, RankingData> RankingData =
        new("TimeAttack/RankingData.json", x => x.QuestId);

    /// <summary>
    /// Contains information about the initial non-ranked rewards for clearing Time Attack quests.
    /// </summary>
    public static readonly MasterAssetData<int, RankingTierReward> RankingTierReward =
        new("TimeAttack/RankingTierReward.json", x => x.Id);

    #endregion

    #region Wall
    /// <summary>
    /// Contains information about Mercurial Gauntlet quests.
    /// </summary>
    public static readonly MasterAssetData<int, QuestWallDetail> QuestWallDetail =
        new("Wall/QuestWallDetail.json", x => x.Id);

    /// <summary>
    /// Contains information about Mercurial Gauntlet monthly rewards.
    /// </summary>
    public static readonly MasterAssetData<int, QuestWallMonthlyReward> QuestWallMonthlyReward =
        new("Wall/QuestWallMonthlyReward.json", x => x.TotalWallLevel);
    #endregion
}
