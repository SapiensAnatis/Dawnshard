using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using DragaliaAPI.Shared.MasterAsset.Models.Enemy;
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
    public static MasterAssetData<Charas, CharaData> CharaData { get; private set; } = null!;
    public static MasterAssetData<Dragons, DragonData> DragonData { get; private set; } = null!;
    public static MasterAssetData<int, DragonRarity> DragonRarity { get; private set; } = null!;
    public static MasterAssetData<int, QuestData> QuestData { get; private set; } = null!;
    public static MasterAssetData<Materials, MaterialData> MaterialData { get; private set; } =
        null!;
    public static MasterAssetData<int, FortPlantDetail> FortPlant { get; private set; } = null!;
    public static MasterAssetData<WeaponBodies, WeaponBody> WeaponBody { get; private set; } =
        null!;
    public static MasterAssetData<int, WeaponBodyBuildupGroup> WeaponBodyBuildupGroup
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, WeaponBodyBuildupLevel> WeaponBodyBuildupLevel
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, WeaponPassiveAbility> WeaponPassiveAbility
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, WeaponBodyRarity> WeaponBodyRarity { get; private set; } =
        null!;
    public static MasterAssetData<int, WeaponSkin> WeaponSkin { get; private set; } = null!;
    public static MasterAssetData<int, AbilityCrestBuildupGroup> AbilityCrestBuildupGroup
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, AbilityCrestBuildupLevel> AbilityCrestBuildupLevel
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, AbilityCrestRarity> AbilityCrestRarity
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<AbilityCrests, AbilityCrest> AbilityCrest { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestEventGroup> QuestEventGroup { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestEvent> QuestEvent { get; private set; } = null!;
    public static MasterAssetData<int, QuestTreasureData> QuestTreasureData { get; private set; } =
        null!;
    public static MasterAssetData<UseItem, UseItemData> UseItem { get; private set; } = null!;
    public static MasterAssetData<int, AbilityData> AbilityData { get; private set; } = null!;
    public static MasterAssetData<int, AbilityLimitedGroup> AbilityLimitedGroup
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, ExAbilityData> ExAbilityData { get; private set; } = null!;
    public static MasterAssetData<int, UnionAbility> UnionAbility { get; private set; } = null!;
    public static MasterAssetData<int, SkillData> SkillData { get; private set; } = null!;
    public static MasterAssetData<int, AlbumMission> AlbumMission { get; private set; } = null!;
    public static MasterAssetData<int, NormalMission> BeginnerMission { get; private set; } = null!;
    public static MasterAssetData<int, DailyMission> DailyMission { get; private set; } = null!;
    public static MasterAssetData<int, DrillMission> DrillMission { get; private set; } = null!;
    public static MasterAssetData<int, DrillMissionGroup> DrillMissionGroup { get; private set; } =
        null!;
    public static MasterAssetData<int, MainStoryMission> MainStoryMission { get; private set; } =
        null!;
    public static MasterAssetData<int, MainStoryMissionGroup> MainStoryMissionGroup
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, MemoryEventMission> MemoryEventMission
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, NormalMission> NormalMission { get; private set; } = null!;
    public static MasterAssetData<int, PeriodMission> PeriodMission { get; private set; } = null!;
    public static MasterAssetData<int, SpecialMission> SpecialMission { get; private set; } = null!;
    public static MasterAssetData<int, SpecialMissionGroup> SpecialMissionGroup
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, MissionProgressionInfo> MissionProgressionInfo
    {
        get;
        private set;
    } = null!;

    public static MasterAssetData<int, MainStoryMissionGroupRewards> MainStoryMissionGroupRewards
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, Stamp> StampData { get; private set; } = null!;
    public static MasterAssetData<int, NormalShop> NormalShop { get; private set; } = null!;
    public static MasterAssetData<int, SpecialShop> SpecialShop { get; private set; } = null!;
    public static MasterAssetData<int, MaterialShop> MaterialShopDaily { get; private set; } =
        null!;
    public static MasterAssetData<int, MaterialShop> MaterialShopWeekly { get; private set; } =
        null!;
    public static MasterAssetData<int, MaterialShop> MaterialShopMonthly { get; private set; } =
        null!;
    public static MasterAssetData<int, AbilityCrestTrade> AbilityCrestTrade { get; private set; } =
        null!;
    public static MasterAssetData<int, TreasureTrade> TreasureTrade { get; private set; } = null!;
    public static MasterAssetData<int, TreasureTrade> EventTreasureTrade { get; private set; } =
        null!;
    public static MasterAssetData<int, LoginBonusData> LoginBonusData { get; private set; } = null!;
    public static MasterAssetData<int, LoginBonusReward> LoginBonusReward { get; private set; } =
        null!;
    public static MasterAssetData<int, ManaNode> ManaNode { get; private set; } = null!;
    public static MasterAssetData<int, ManaPieceMaterial> ManaPieceMaterial { get; private set; } =
        null!;
    public static MasterAssetData<ManaNodeTypes, ManaPieceType> ManaPieceType
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, CharaLimitBreak> CharaLimitBreak { get; private set; } =
        null!;
    public static MasterAssetData<int, StoryData> DragonStories { get; private set; } = null!;
    public static MasterAssetData<int, StoryData> CharaStories { get; private set; } = null!;
    public static MasterAssetData<int, UnitStory> UnitStory { get; private set; } = null!;
    public static MasterAssetData<int, QuestStory> QuestStory { get; private set; } = null!;
    public static MasterAssetData<int, EventStory> EventStory { get; private set; } = null!;
    public static MasterAssetData<int, QuestStoryRewardInfo> QuestStoryRewardInfo
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<string, QuestEnemies> QuestEnemies { get; private set; } = null!;
    public static MasterAssetData<int, EnemyParam> EnemyParam { get; private set; } = null!;
    public static MasterAssetData<int, EnemyData> EnemyData { get; private set; } = null!;
    public static MasterAssetData<int, QuestDropInfo> QuestDrops { get; private set; } = null!;
    public static MasterAssetData<int, QuestBonusReward> QuestBonusRewards { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestRewardData> QuestRewardData { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestScoreMissionRewardInfo> QuestScoreMissionRewardInfo
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, QuestScoreMissionData> QuestScoreMissionData
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventData> EventData { get; private set; } = null!;
    public static MasterAssetData<int, EventTradeGroup> EventTradeGroup { get; private set; } =
        null!;
    public static MasterAssetGroup<int, int, BuildEventReward> BuildEventReward
    {
        get;
        private set;
    } = null!;
    public static MasterAssetGroup<int, int, RaidEventReward> RaidEventReward
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, CombatEventLocation> CombatEventLocation
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, CombatEventLocationReward> CombatEventLocationReward
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<BuildEventItemType>> BuildEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<CombatEventItemType>> CombatEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, RaidEventItem> RaidEventItem { get; private set; } = null!;
    public static MasterAssetData<int, EventItem<SimpleEventItemType>> SimpleEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<ExRushEventItemType>> ExRushEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<ExHunterEventItemType>> ExHunterEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<EarnEventItemType>> EarnEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<CollectEventItemType>> CollectEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<Clb01EventItemType>> Clb01EventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventItem<BattleRoyalEventItemType>> BattleRoyalEventItem
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, EventPassive> EventPassive { get; private set; } = null!;
    public static MasterAssetData<int, QuestScoringEnemy> QuestScoringEnemy { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeQuestFloor> DmodeQuestFloor { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeDungeonArea> DmodeDungeonArea { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeDungeonTheme> DmodeDungeonTheme { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeEnemyTheme> DmodeEnemyTheme { get; private set; } =
        null!;
    public static MasterAssetData<string, DmodeAreaInfo> DmodeAreaInfo { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeEnemyParam> DmodeEnemyParam { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeCharaLevel> DmodeCharaLevel { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeWeapon> DmodeWeapon { get; private set; } = null!;
    public static MasterAssetData<int, DmodeAbilityCrest> DmodeAbilityCrest { get; private set; } =
        null!;
    public static MasterAssetData<int, DmodeStrengthParam> DmodeStrengthParam
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, DmodeStrengthSkill> DmodeStrengthSkill
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, DmodeStrengthAbility> DmodeStrengthAbility
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, DmodeDungeonItemData> DmodeDungeonItemData
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, DmodeServitorPassiveLevel> DmodeServitorPassiveLevel
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, DmodeExpeditionFloor> DmodeExpeditionFloor
    {
        get;
        private set;
    } = null!;
    public static MasterAssetData<int, UserLevel> UserLevel { get; private set; } = null!;
    public static MasterAssetData<int, QuestScheduleInfo> QuestScheduleInfo { get; private set; } =
        null!;
    public static MasterAssetData<int, RankingData> RankingData { get; private set; } = null!;
    public static MasterAssetData<int, RankingTierReward> RankingTierReward { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestWallDetail> QuestWallDetail { get; private set; } =
        null!;
    public static MasterAssetData<int, QuestWallMonthlyReward> QuestWallMonthlyReward
    {
        get;
        private set;
    } = null!;

    public static async Task LoadAsync()
    {
        ValueTask<MasterAssetData<Charas, CharaData>> charaData = MasterAssetData.LoadAsync<
            Charas,
            CharaData
        >("CharaData.json", x => x.Id);

        ValueTask<MasterAssetData<Dragons, DragonData>> dragonData = MasterAssetData.LoadAsync<
            Dragons,
            DragonData
        >("DragonData.json", x => x.Id);

        ValueTask<MasterAssetData<int, DragonRarity>> dragonRarity = MasterAssetData.LoadAsync<
            int,
            DragonRarity
        >("DragonRarity.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestData>> questData = MasterAssetData.LoadAsync<
            int,
            QuestData
        >("QuestData.json", x => x.Id);

        ValueTask<MasterAssetData<Materials, MaterialData>> materialData =
            MasterAssetData.LoadAsync<Materials, MaterialData>("MaterialData.json", x => x.Id);

        ValueTask<MasterAssetData<int, FortPlantDetail>> fortPlant = MasterAssetData.LoadAsync<
            int,
            FortPlantDetail
        >("FortPlantDetail.json", x => x.Id);

        ValueTask<MasterAssetData<WeaponBodies, WeaponBody>> weaponBody = MasterAssetData.LoadAsync<
            WeaponBodies,
            WeaponBody
        >("WeaponBody.json", x => x.Id);

        ValueTask<MasterAssetData<int, WeaponBodyBuildupGroup>> weaponBodyBuildupGroup =
            MasterAssetData.LoadAsync<int, WeaponBodyBuildupGroup>(
                "WeaponBodyBuildupGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, WeaponBodyBuildupLevel>> weaponBodyBuildupLevel =
            MasterAssetData.LoadAsync<int, WeaponBodyBuildupLevel>(
                "WeaponBodyBuildupLevel.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, WeaponPassiveAbility>> weaponPassiveAbility =
            MasterAssetData.LoadAsync<int, WeaponPassiveAbility>(
                "WeaponPassiveAbility.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, WeaponBodyRarity>> weaponBodyRarity =
            MasterAssetData.LoadAsync<int, WeaponBodyRarity>("WeaponBodyRarity.json", x => x.Id);

        ValueTask<MasterAssetData<int, WeaponSkin>> weaponSkin = MasterAssetData.LoadAsync<
            int,
            WeaponSkin
        >("WeaponSkin.json", x => x.Id);

        ValueTask<MasterAssetData<int, AbilityCrestBuildupGroup>> abilityCrestBuildupGroup =
            MasterAssetData.LoadAsync<int, AbilityCrestBuildupGroup>(
                "AbilityCrestBuildupGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, AbilityCrestBuildupLevel>> abilityCrestBuildupLevel =
            MasterAssetData.LoadAsync<int, AbilityCrestBuildupLevel>(
                "AbilityCrestBuildupLevel.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, AbilityCrestRarity>> abilityCrestRarity =
            MasterAssetData.LoadAsync<int, AbilityCrestRarity>(
                "AbilityCrestRarity.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<AbilityCrests, AbilityCrest>> abilityCrest =
            MasterAssetData.LoadAsync<AbilityCrests, AbilityCrest>("AbilityCrest.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestEventGroup>> questEventGroup =
            MasterAssetData.LoadAsync<int, QuestEventGroup>("QuestEventGroup.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestEvent>> questEvent = MasterAssetData.LoadAsync<
            int,
            QuestEvent
        >("QuestEvent.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestTreasureData>> questTreasureData =
            MasterAssetData.LoadAsync<int, QuestTreasureData>("QuestTreasureData.json", x => x.Id);

        ValueTask<MasterAssetData<UseItem, UseItemData>> useItem = MasterAssetData.LoadAsync<
            UseItem,
            UseItemData
        >("UseItem.json", x => x.Id);

        ValueTask<MasterAssetData<int, AbilityData>> abilityData = MasterAssetData.LoadAsync<
            int,
            AbilityData
        >("AbilityData.json", x => x.Id);

        ValueTask<MasterAssetData<int, AbilityLimitedGroup>> abilityLimitedGroup =
            MasterAssetData.LoadAsync<int, AbilityLimitedGroup>(
                "AbilityLimitedGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, ExAbilityData>> exAbilityData = MasterAssetData.LoadAsync<
            int,
            ExAbilityData
        >("ExAbilityData.json", x => x.Id);

        ValueTask<MasterAssetData<int, UnionAbility>> unionAbility = MasterAssetData.LoadAsync<
            int,
            UnionAbility
        >("UnionAbility.json", x => x.Id);

        ValueTask<MasterAssetData<int, SkillData>> skillData = MasterAssetData.LoadAsync<
            int,
            SkillData
        >("SkillData.json", x => x.Id);

        ValueTask<MasterAssetData<int, AlbumMission>> albumMission = MasterAssetData.LoadAsync<
            int,
            AlbumMission
        >("Missions/MissionAlbumData.json", x => x.Id);

        ValueTask<MasterAssetData<int, NormalMission>> beginnerMission = MasterAssetData.LoadAsync<
            int,
            NormalMission
        >("Missions/MissionBeginnerData.json", x => x.Id);

        ValueTask<MasterAssetData<int, DailyMission>> dailyMission = MasterAssetData.LoadAsync<
            int,
            DailyMission
        >("Missions/MissionDailyData.json", x => x.Id);

        ValueTask<MasterAssetData<int, DrillMission>> drillMission = MasterAssetData.LoadAsync<
            int,
            DrillMission
        >("Missions/MissionDrillData.json", x => x.Id);

        ValueTask<MasterAssetData<int, DrillMissionGroup>> drillMissionGroup =
            MasterAssetData.LoadAsync<int, DrillMissionGroup>(
                "Missions/MissionDrillGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, MainStoryMission>> mainStoryMission =
            MasterAssetData.LoadAsync<int, MainStoryMission>(
                "Missions/MissionMainStoryData.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, MainStoryMissionGroup>> mainStoryMissionGroup =
            MasterAssetData.LoadAsync<int, MainStoryMissionGroup>(
                "Missions/MissionMainStoryGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, MemoryEventMission>> memoryEventMission =
            MasterAssetData.LoadAsync<int, MemoryEventMission>(
                "Missions/MissionMemoryEventData.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, NormalMission>> normalMission = MasterAssetData.LoadAsync<
            int,
            NormalMission
        >("Missions/MissionNormalData.json", x => x.Id);

        ValueTask<MasterAssetData<int, PeriodMission>> periodMission = MasterAssetData.LoadAsync<
            int,
            PeriodMission
        >("Missions/MissionPeriodData.json", x => x.Id);

        ValueTask<MasterAssetData<int, SpecialMission>> specialMission = MasterAssetData.LoadAsync<
            int,
            SpecialMission
        >("Missions/MissionSpecialData.json", x => x.Id);

        ValueTask<MasterAssetData<int, SpecialMissionGroup>> specialMissionGroup =
            MasterAssetData.LoadAsync<int, SpecialMissionGroup>(
                "Missions/MissionSpecialGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, MissionProgressionInfo>> missionProgressionInfo =
            MasterAssetData.LoadAsync<int, MissionProgressionInfo>(
                "Missions/MissionProgressionInfo.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, MainStoryMissionGroupRewards>> mainStoryMissionGroupRewards =
            MasterAssetData.LoadAsync<int, MainStoryMissionGroupRewards>(
                "Missions/MainStoryMissionGroupRewards.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, Stamp>> stampData = MasterAssetData.LoadAsync<int, Stamp>(
            "StampData.json",
            x => x.Id
        );

        ValueTask<MasterAssetData<int, NormalShop>> normalShop = MasterAssetData.LoadAsync<
            int,
            NormalShop
        >("Shop/NormalShop.json", x => x.Id);

        ValueTask<MasterAssetData<int, SpecialShop>> specialShop = MasterAssetData.LoadAsync<
            int,
            SpecialShop
        >("Shop/SpecialShop.json", x => x.Id);

        ValueTask<MasterAssetData<int, MaterialShop>> materialShopDaily = MasterAssetData.LoadAsync<
            int,
            MaterialShop
        >("Shop/MaterialShopDaily.json", x => x.Id);

        ValueTask<MasterAssetData<int, MaterialShop>> materialShopWeekly =
            MasterAssetData.LoadAsync<int, MaterialShop>("Shop/MaterialShopWeekly.json", x => x.Id);

        ValueTask<MasterAssetData<int, MaterialShop>> materialShopMonthly =
            MasterAssetData.LoadAsync<int, MaterialShop>(
                "Shop/MaterialShopMonthly.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, AbilityCrestTrade>> abilityCrestTrade =
            MasterAssetData.LoadAsync<int, AbilityCrestTrade>(
                "Trade/AbilityCrestTrade.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, TreasureTrade>> treasureTrade = MasterAssetData.LoadAsync<
            int,
            TreasureTrade
        >("Trade/TreasureTrade.json", x => x.Id);

        ValueTask<MasterAssetData<int, TreasureTrade>> eventTreasureTrade =
            MasterAssetData.LoadAsync<int, TreasureTrade>(
                "Trade/EventTreasureTradeInfo.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, LoginBonusData>> loginBonusData = MasterAssetData.LoadAsync<
            int,
            LoginBonusData
        >("Login/LoginBonusData.json", x => x.Id);

        ValueTask<MasterAssetData<int, LoginBonusReward>> loginBonusReward =
            MasterAssetData.LoadAsync<int, LoginBonusReward>(
                "Login/LoginBonusReward.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, ManaNode>> manaNode = MasterAssetData.LoadAsync<
            int,
            ManaNode
        >("ManaCircle/MC.json", x => x.MC_0);

        ValueTask<MasterAssetData<int, ManaPieceMaterial>> manaPieceMaterial =
            MasterAssetData.LoadAsync<int, ManaPieceMaterial>(
                "ManaCircle/ManaPieceMaterial.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<ManaNodeTypes, ManaPieceType>> manaPieceType =
            MasterAssetData.LoadAsync<ManaNodeTypes, ManaPieceType>(
                "ManaCircle/ManaPieceType.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, CharaLimitBreak>> charaLimitBreak =
            MasterAssetData.LoadAsync<int, CharaLimitBreak>(
                "ManaCircle/CharaLimitBreak.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, StoryData>> dragonStories = MasterAssetData.LoadAsync<
            int,
            StoryData
        >("Story/DragonStories.json", x => x.id);

        ValueTask<MasterAssetData<int, StoryData>> charaStories = MasterAssetData.LoadAsync<
            int,
            StoryData
        >("Story/CharaStories.json", x => x.id);

        ValueTask<MasterAssetData<int, UnitStory>> unitStory = MasterAssetData.LoadAsync<
            int,
            UnitStory
        >("Story/UnitStory.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestStory>> questStory = MasterAssetData.LoadAsync<
            int,
            QuestStory
        >("Story/QuestStory.json", x => x.Id);

        ValueTask<MasterAssetData<int, EventStory>> eventStory = MasterAssetData.LoadAsync<
            int,
            EventStory
        >("Story/EventStory.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestStoryRewardInfo>> questStoryRewardInfo =
            MasterAssetData.LoadAsync<int, QuestStoryRewardInfo>(
                "Story/QuestStoryRewardInfo.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<string, QuestEnemies>> questEnemies = MasterAssetData.LoadAsync<
            string,
            QuestEnemies
        >("Enemy/QuestEnemies.json", x => x.AreaName);

        ValueTask<MasterAssetData<int, EnemyParam>> enemyParam = MasterAssetData.LoadAsync<
            int,
            EnemyParam
        >("Enemy/EnemyParam.json", x => x.Id);

        ValueTask<MasterAssetData<int, EnemyData>> enemyData = MasterAssetData.LoadAsync<
            int,
            EnemyData
        >("Enemy/EnemyData.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestDropInfo>> questDrops = MasterAssetData.LoadAsync<
            int,
            QuestDropInfo
        >("QuestDrops/QuestDrops.json", x => x.QuestId);

        ValueTask<MasterAssetData<int, QuestBonusReward>> questBonusRewards =
            MasterAssetData.LoadAsync<int, QuestBonusReward>(
                "QuestDrops/QuestBonusRewards.json",
                x => x.QuestId
            );

        ValueTask<MasterAssetData<int, QuestRewardData>> questRewardData =
            MasterAssetData.LoadAsync<int, QuestRewardData>(
                "QuestRewards/QuestRewardData.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, QuestScoreMissionRewardInfo>> questScoreMissionRewardInfo =
            MasterAssetData.LoadAsync<int, QuestScoreMissionRewardInfo>(
                "QuestRewards/QuestScoreMissionRewardInfo.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, QuestScoreMissionData>> questScoreMissionData =
            MasterAssetData.LoadAsync<int, QuestScoreMissionData>(
                "QuestRewards/QuestScoreMissionData.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventData>> eventData = MasterAssetData.LoadAsync<
            int,
            EventData
        >("Event/EventData.json", x => x.Id);

        ValueTask<MasterAssetData<int, EventTradeGroup>> eventTradeGroup =
            MasterAssetData.LoadAsync<int, EventTradeGroup>(
                "Event/EventTradeGroup.json",
                x => x.Id
            );

        ValueTask<MasterAssetGroup<int, int, BuildEventReward>> buildEventReward =
            MasterAssetGroup.LoadAsync<int, int, BuildEventReward>(
                "Event/BuildEventReward.json",
                x => x.Id
            );

        ValueTask<MasterAssetGroup<int, int, RaidEventReward>> raidEventReward =
            MasterAssetGroup.LoadAsync<int, int, RaidEventReward>(
                "Event/RaidEventReward.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, CombatEventLocation>> combatEventLocation =
            MasterAssetData.LoadAsync<int, CombatEventLocation>(
                "Event/CombatEventLocation.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, CombatEventLocationReward>> combatEventLocationReward =
            MasterAssetData.LoadAsync<int, CombatEventLocationReward>(
                "Event/CombatEventLocationReward.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<BuildEventItemType>>> buildEventItem =
            MasterAssetData.LoadAsync<int, EventItem<BuildEventItemType>>(
                "Event/BuildEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<CombatEventItemType>>> combatEventItem =
            MasterAssetData.LoadAsync<int, EventItem<CombatEventItemType>>(
                "Event/CombatEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, RaidEventItem>> raidEventItem = MasterAssetData.LoadAsync<
            int,
            RaidEventItem
        >("Event/RaidEventItem.json", x => x.Id);

        ValueTask<MasterAssetData<int, EventItem<SimpleEventItemType>>> simpleEventItem =
            MasterAssetData.LoadAsync<int, EventItem<SimpleEventItemType>>(
                "Event/SimpleEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<ExRushEventItemType>>> exRushEventItem =
            MasterAssetData.LoadAsync<int, EventItem<ExRushEventItemType>>(
                "Event/ExRushEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<ExHunterEventItemType>>> exHunterEventItem =
            MasterAssetData.LoadAsync<int, EventItem<ExHunterEventItemType>>(
                "Event/ExHunterEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<EarnEventItemType>>> earnEventItem =
            MasterAssetData.LoadAsync<int, EventItem<EarnEventItemType>>(
                "Event/EarnEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<CollectEventItemType>>> collectEventItem =
            MasterAssetData.LoadAsync<int, EventItem<CollectEventItemType>>(
                "Event/CollectEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<Clb01EventItemType>>> clb01EventItem =
            MasterAssetData.LoadAsync<int, EventItem<Clb01EventItemType>>(
                "Event/Clb01EventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventItem<BattleRoyalEventItemType>>> battleRoyalEventItem =
            MasterAssetData.LoadAsync<int, EventItem<BattleRoyalEventItemType>>(
                "Event/BattleRoyalEventItem.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, EventPassive>> eventPassive = MasterAssetData.LoadAsync<
            int,
            EventPassive
        >("Event/EventPassive.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestScoringEnemy>> questScoringEnemy =
            MasterAssetData.LoadAsync<int, QuestScoringEnemy>(
                "Event/QuestScoringEnemy.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeQuestFloor>> dmodeQuestFloor =
            MasterAssetData.LoadAsync<int, DmodeQuestFloor>(
                "Dmode/DmodeQuestFloor.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeDungeonArea>> dmodeDungeonArea =
            MasterAssetData.LoadAsync<int, DmodeDungeonArea>(
                "Dmode/DmodeDungeonArea.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeDungeonTheme>> dmodeDungeonTheme =
            MasterAssetData.LoadAsync<int, DmodeDungeonTheme>(
                "Dmode/DmodeDungeonTheme.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeEnemyTheme>> dmodeEnemyTheme =
            MasterAssetData.LoadAsync<int, DmodeEnemyTheme>(
                "Dmode/DmodeEnemyTheme.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<string, DmodeAreaInfo>> dmodeAreaInfo = MasterAssetData.LoadAsync<
            string,
            DmodeAreaInfo
        >("Dmode/DmodeAreaInfo.json", x => x.AreaName);

        ValueTask<MasterAssetData<int, DmodeEnemyParam>> dmodeEnemyParam =
            MasterAssetData.LoadAsync<int, DmodeEnemyParam>(
                "Dmode/DmodeEnemyParam.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeCharaLevel>> dmodeCharaLevel =
            MasterAssetData.LoadAsync<int, DmodeCharaLevel>(
                "Dmode/DmodeCharaLevel.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeWeapon>> dmodeWeapon = MasterAssetData.LoadAsync<
            int,
            DmodeWeapon
        >("Dmode/DmodeWeapon.json", x => x.Id);

        ValueTask<MasterAssetData<int, DmodeAbilityCrest>> dmodeAbilityCrest =
            MasterAssetData.LoadAsync<int, DmodeAbilityCrest>(
                "Dmode/DmodeAbilityCrest.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeStrengthParam>> dmodeStrengthParam =
            MasterAssetData.LoadAsync<int, DmodeStrengthParam>(
                "Dmode/DmodeStrengthParam.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeStrengthSkill>> dmodeStrengthSkill =
            MasterAssetData.LoadAsync<int, DmodeStrengthSkill>(
                "Dmode/DmodeStrengthSkill.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeStrengthAbility>> dmodeStrengthAbility =
            MasterAssetData.LoadAsync<int, DmodeStrengthAbility>(
                "Dmode/DmodeStrengthAbility.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeDungeonItemData>> dmodeDungeonItemData =
            MasterAssetData.LoadAsync<int, DmodeDungeonItemData>(
                "Dmode/DmodeDungeonItemData.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeServitorPassiveLevel>> dmodeServitorPassiveLevel =
            MasterAssetData.LoadAsync<int, DmodeServitorPassiveLevel>(
                "Dmode/DmodeServitorPassiveLevel.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, DmodeExpeditionFloor>> dmodeExpeditionFloor =
            MasterAssetData.LoadAsync<int, DmodeExpeditionFloor>(
                "Dmode/DmodeExpeditionFloor.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, UserLevel>> userLevel = MasterAssetData.LoadAsync<
            int,
            UserLevel
        >("User/UserLevel.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestScheduleInfo>> questScheduleInfo =
            MasterAssetData.LoadAsync<int, QuestScheduleInfo>(
                "QuestSchedule/QuestScheduleInfo.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, RankingData>> rankingData = MasterAssetData.LoadAsync<
            int,
            RankingData
        >("TimeAttack/RankingData.json", x => x.QuestId);

        ValueTask<MasterAssetData<int, RankingTierReward>> rankingTierReward =
            MasterAssetData.LoadAsync<int, RankingTierReward>(
                "TimeAttack/RankingTierReward.json",
                x => x.Id
            );

        ValueTask<MasterAssetData<int, QuestWallDetail>> questWallDetail =
            MasterAssetData.LoadAsync<int, QuestWallDetail>("Wall/QuestWallDetail.json", x => x.Id);

        ValueTask<MasterAssetData<int, QuestWallMonthlyReward>> questWallMonthlyReward =
            MasterAssetData.LoadAsync<int, QuestWallMonthlyReward>(
                "Wall/QuestWallMonthlyReward.json",
                x => x.TotalWallLevel
            );

        CharaData = await charaData;
        DragonData = await dragonData;
        DragonRarity = await dragonRarity;
        QuestData = await questData;
        MaterialData = await materialData;
        FortPlant = await fortPlant;
        WeaponBody = await weaponBody;
        WeaponBodyBuildupGroup = await weaponBodyBuildupGroup;
        WeaponBodyBuildupLevel = await weaponBodyBuildupLevel;
        WeaponPassiveAbility = await weaponPassiveAbility;
        WeaponBodyRarity = await weaponBodyRarity;
        WeaponSkin = await weaponSkin;
        AbilityCrestBuildupGroup = await abilityCrestBuildupGroup;
        AbilityCrestBuildupLevel = await abilityCrestBuildupLevel;
        AbilityCrestRarity = await abilityCrestRarity;
        AbilityCrest = await abilityCrest;
        QuestEventGroup = await questEventGroup;
        QuestEvent = await questEvent;
        QuestTreasureData = await questTreasureData;
        UseItem = await useItem;
        AbilityData = await abilityData;
        AbilityLimitedGroup = await abilityLimitedGroup;
        ExAbilityData = await exAbilityData;
        UnionAbility = await unionAbility;
        SkillData = await skillData;
        AlbumMission = await albumMission;
        BeginnerMission = await beginnerMission;
        DailyMission = await dailyMission;
        DrillMission = await drillMission;
        DrillMissionGroup = await drillMissionGroup;
        MainStoryMission = await mainStoryMission;
        MainStoryMissionGroup = await mainStoryMissionGroup;
        MemoryEventMission = await memoryEventMission;
        NormalMission = await normalMission;
        PeriodMission = await periodMission;
        SpecialMission = await specialMission;
        SpecialMissionGroup = await specialMissionGroup;
        MissionProgressionInfo = await missionProgressionInfo;
        MainStoryMissionGroupRewards = await mainStoryMissionGroupRewards;
        StampData = await stampData;
        NormalShop = await normalShop;
        SpecialShop = await specialShop;
        MaterialShopDaily = await materialShopDaily;
        MaterialShopWeekly = await materialShopWeekly;
        MaterialShopMonthly = await materialShopMonthly;
        AbilityCrestTrade = await abilityCrestTrade;
        TreasureTrade = await treasureTrade;
        EventTreasureTrade = await eventTreasureTrade;
        LoginBonusData = await loginBonusData;
        LoginBonusReward = await loginBonusReward;
        ManaNode = await manaNode;
        ManaPieceMaterial = await manaPieceMaterial;
        ManaPieceType = await manaPieceType;
        CharaLimitBreak = await charaLimitBreak;
        DragonStories = await dragonStories;
        CharaStories = await charaStories;
        UnitStory = await unitStory;
        QuestStory = await questStory;
        EventStory = await eventStory;
        QuestStoryRewardInfo = await questStoryRewardInfo;
        QuestEnemies = await questEnemies;
        EnemyParam = await enemyParam;
        EnemyData = await enemyData;
        QuestDrops = await questDrops;
        QuestBonusRewards = await questBonusRewards;
        QuestRewardData = await questRewardData;
        QuestScoreMissionRewardInfo = await questScoreMissionRewardInfo;
        QuestScoreMissionData = await questScoreMissionData;
        EventData = await eventData;
        EventTradeGroup = await eventTradeGroup;
        CombatEventLocation = await combatEventLocation;
        CombatEventLocationReward = await combatEventLocationReward;
        BuildEventItem = await buildEventItem;
        CombatEventItem = await combatEventItem;
        RaidEventItem = await raidEventItem;
        SimpleEventItem = await simpleEventItem;
        ExRushEventItem = await exRushEventItem;
        ExHunterEventItem = await exHunterEventItem;
        EarnEventItem = await earnEventItem;
        CollectEventItem = await collectEventItem;
        Clb01EventItem = await clb01EventItem;
        BattleRoyalEventItem = await battleRoyalEventItem;
        EventPassive = await eventPassive;
        QuestScoringEnemy = await questScoringEnemy;
        DmodeQuestFloor = await dmodeQuestFloor;
        DmodeDungeonArea = await dmodeDungeonArea;
        DmodeDungeonTheme = await dmodeDungeonTheme;
        DmodeEnemyTheme = await dmodeEnemyTheme;
        DmodeAreaInfo = await dmodeAreaInfo;
        DmodeEnemyParam = await dmodeEnemyParam;
        DmodeCharaLevel = await dmodeCharaLevel;
        DmodeWeapon = await dmodeWeapon;
        DmodeAbilityCrest = await dmodeAbilityCrest;
        DmodeStrengthParam = await dmodeStrengthParam;
        DmodeStrengthSkill = await dmodeStrengthSkill;
        DmodeStrengthAbility = await dmodeStrengthAbility;
        DmodeDungeonItemData = await dmodeDungeonItemData;
        DmodeServitorPassiveLevel = await dmodeServitorPassiveLevel;
        DmodeExpeditionFloor = await dmodeExpeditionFloor;
        UserLevel = await userLevel;
        QuestScheduleInfo = await questScheduleInfo;
        RankingData = await rankingData;
        RankingTierReward = await rankingTierReward;
        QuestWallDetail = await questWallDetail;
        QuestWallMonthlyReward = await questWallMonthlyReward;
        BuildEventReward = await buildEventReward;
        RaidEventReward = await raidEventReward;
    }
}
