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
        List<Task> tasks = [];

        Task<MasterAssetData<Charas, CharaData>> charaData = GetAndAddTask<Charas, CharaData>(
            "CharaData.json",
            x => x.Id
        );

        Task<MasterAssetData<Dragons, DragonData>> dragonData = GetAndAddTask<Dragons, DragonData>(
            "DragonData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, DragonRarity>> dragonRarity = GetAndAddTask<int, DragonRarity>(
            "DragonRarity.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestData>> questData = GetAndAddTask<int, QuestData>(
            "QuestData.json",
            x => x.Id
        );

        Task<MasterAssetData<Materials, MaterialData>> materialData = GetAndAddTask<
            Materials,
            MaterialData
        >("MaterialData.json", x => x.Id);

        Task<MasterAssetData<int, FortPlantDetail>> fortPlant = GetAndAddTask<int, FortPlantDetail>(
            "FortPlantDetail.json",
            x => x.Id
        );

        Task<MasterAssetData<WeaponBodies, WeaponBody>> weaponBody = GetAndAddTask<
            WeaponBodies,
            WeaponBody
        >("WeaponBody.json", x => x.Id);

        Task<MasterAssetData<int, WeaponBodyBuildupGroup>> weaponBodyBuildupGroup = GetAndAddTask<
            int,
            WeaponBodyBuildupGroup
        >("WeaponBodyBuildupGroup.json", x => x.Id);

        Task<MasterAssetData<int, WeaponBodyBuildupLevel>> weaponBodyBuildupLevel = GetAndAddTask<
            int,
            WeaponBodyBuildupLevel
        >("WeaponBodyBuildupLevel.json", x => x.Id);

        Task<MasterAssetData<int, WeaponPassiveAbility>> weaponPassiveAbility = GetAndAddTask<
            int,
            WeaponPassiveAbility
        >("WeaponPassiveAbility.json", x => x.Id);

        Task<MasterAssetData<int, WeaponBodyRarity>> weaponBodyRarity = GetAndAddTask<
            int,
            WeaponBodyRarity
        >("WeaponBodyRarity.json", x => x.Id);

        Task<MasterAssetData<int, WeaponSkin>> weaponSkin = GetAndAddTask<int, WeaponSkin>(
            "WeaponSkin.json",
            x => x.Id
        );

        Task<MasterAssetData<int, AbilityCrestBuildupGroup>> abilityCrestBuildupGroup =
            GetAndAddTask<int, AbilityCrestBuildupGroup>(
                "AbilityCrestBuildupGroup.json",
                x => x.Id
            );

        Task<MasterAssetData<int, AbilityCrestBuildupLevel>> abilityCrestBuildupLevel =
            GetAndAddTask<int, AbilityCrestBuildupLevel>(
                "AbilityCrestBuildupLevel.json",
                x => x.Id
            );

        Task<MasterAssetData<int, AbilityCrestRarity>> abilityCrestRarity = GetAndAddTask<
            int,
            AbilityCrestRarity
        >("AbilityCrestRarity.json", x => x.Id);

        Task<MasterAssetData<AbilityCrests, AbilityCrest>> abilityCrest = GetAndAddTask<
            AbilityCrests,
            AbilityCrest
        >("AbilityCrest.json", x => x.Id);

        Task<MasterAssetData<int, QuestEventGroup>> questEventGroup = GetAndAddTask<
            int,
            QuestEventGroup
        >("QuestEventGroup.json", x => x.Id);

        Task<MasterAssetData<int, QuestEvent>> questEvent = GetAndAddTask<int, QuestEvent>(
            "QuestEvent.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestTreasureData>> questTreasureData = GetAndAddTask<
            int,
            QuestTreasureData
        >("QuestTreasureData.json", x => x.Id);

        Task<MasterAssetData<UseItem, UseItemData>> useItem = GetAndAddTask<UseItem, UseItemData>(
            "UseItem.json",
            x => x.Id
        );

        Task<MasterAssetData<int, AbilityData>> abilityData = GetAndAddTask<int, AbilityData>(
            "AbilityData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, AbilityLimitedGroup>> abilityLimitedGroup = GetAndAddTask<
            int,
            AbilityLimitedGroup
        >("AbilityLimitedGroup.json", x => x.Id);

        Task<MasterAssetData<int, ExAbilityData>> exAbilityData = GetAndAddTask<int, ExAbilityData>(
            "ExAbilityData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, UnionAbility>> unionAbility = GetAndAddTask<int, UnionAbility>(
            "UnionAbility.json",
            x => x.Id
        );

        Task<MasterAssetData<int, SkillData>> skillData = GetAndAddTask<int, SkillData>(
            "SkillData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, AlbumMission>> albumMission = GetAndAddTask<int, AlbumMission>(
            "Missions/MissionAlbumData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, NormalMission>> beginnerMission = GetAndAddTask<
            int,
            NormalMission
        >("Missions/MissionBeginnerData.json", x => x.Id);

        Task<MasterAssetData<int, DailyMission>> dailyMission = GetAndAddTask<int, DailyMission>(
            "Missions/MissionDailyData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, DrillMission>> drillMission = GetAndAddTask<int, DrillMission>(
            "Missions/MissionDrillData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, DrillMissionGroup>> drillMissionGroup = GetAndAddTask<
            int,
            DrillMissionGroup
        >("Missions/MissionDrillGroup.json", x => x.Id);

        Task<MasterAssetData<int, MainStoryMission>> mainStoryMission = GetAndAddTask<
            int,
            MainStoryMission
        >("Missions/MissionMainStoryData.json", x => x.Id);

        Task<MasterAssetData<int, MainStoryMissionGroup>> mainStoryMissionGroup = GetAndAddTask<
            int,
            MainStoryMissionGroup
        >("Missions/MissionMainStoryGroup.json", x => x.Id);

        Task<MasterAssetData<int, MemoryEventMission>> memoryEventMission = GetAndAddTask<
            int,
            MemoryEventMission
        >("Missions/MissionMemoryEventData.json", x => x.Id);

        Task<MasterAssetData<int, NormalMission>> normalMission = GetAndAddTask<int, NormalMission>(
            "Missions/MissionNormalData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, PeriodMission>> periodMission = GetAndAddTask<int, PeriodMission>(
            "Missions/MissionPeriodData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, SpecialMission>> specialMission = GetAndAddTask<
            int,
            SpecialMission
        >("Missions/MissionSpecialData.json", x => x.Id);

        Task<MasterAssetData<int, SpecialMissionGroup>> specialMissionGroup = GetAndAddTask<
            int,
            SpecialMissionGroup
        >("Missions/MissionSpecialGroup.json", x => x.Id);

        Task<MasterAssetData<int, MissionProgressionInfo>> missionProgressionInfo = GetAndAddTask<
            int,
            MissionProgressionInfo
        >("Missions/MissionProgressionInfo.json", x => x.Id);

        Task<MasterAssetData<int, MainStoryMissionGroupRewards>> mainStoryMissionGroupRewards =
            GetAndAddTask<int, MainStoryMissionGroupRewards>(
                "Missions/MainStoryMissionGroupRewards.json",
                x => x.Id
            );

        Task<MasterAssetData<int, Stamp>> stampData = GetAndAddTask<int, Stamp>(
            "StampData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, NormalShop>> normalShop = GetAndAddTask<int, NormalShop>(
            "Shop/NormalShop.json",
            x => x.Id
        );

        Task<MasterAssetData<int, SpecialShop>> specialShop = GetAndAddTask<int, SpecialShop>(
            "Shop/SpecialShop.json",
            x => x.Id
        );

        Task<MasterAssetData<int, MaterialShop>> materialShopDaily = GetAndAddTask<
            int,
            MaterialShop
        >("Shop/MaterialShopDaily.json", x => x.Id);

        Task<MasterAssetData<int, MaterialShop>> materialShopWeekly = GetAndAddTask<
            int,
            MaterialShop
        >("Shop/MaterialShopWeekly.json", x => x.Id);

        Task<MasterAssetData<int, MaterialShop>> materialShopMonthly = GetAndAddTask<
            int,
            MaterialShop
        >("Shop/MaterialShopMonthly.json", x => x.Id);

        Task<MasterAssetData<int, AbilityCrestTrade>> abilityCrestTrade = GetAndAddTask<
            int,
            AbilityCrestTrade
        >("Trade/AbilityCrestTrade.json", x => x.Id);

        Task<MasterAssetData<int, TreasureTrade>> treasureTrade = GetAndAddTask<int, TreasureTrade>(
            "Trade/TreasureTrade.json",
            x => x.Id
        );

        Task<MasterAssetData<int, TreasureTrade>> eventTreasureTrade = GetAndAddTask<
            int,
            TreasureTrade
        >("Trade/EventTreasureTradeInfo.json", x => x.Id);

        Task<MasterAssetData<int, LoginBonusData>> loginBonusData = GetAndAddTask<
            int,
            LoginBonusData
        >("Login/LoginBonusData.json", x => x.Id);

        Task<MasterAssetData<int, LoginBonusReward>> loginBonusReward = GetAndAddTask<
            int,
            LoginBonusReward
        >("Login/LoginBonusReward.json", x => x.Id);

        Task<MasterAssetData<int, ManaNode>> manaNode = GetAndAddTask<int, ManaNode>(
            "ManaCircle/MC.json",
            x => x.MC_0
        );

        Task<MasterAssetData<int, ManaPieceMaterial>> manaPieceMaterial = GetAndAddTask<
            int,
            ManaPieceMaterial
        >("ManaCircle/ManaPieceMaterial.json", x => x.Id);

        Task<MasterAssetData<ManaNodeTypes, ManaPieceType>> manaPieceType = GetAndAddTask<
            ManaNodeTypes,
            ManaPieceType
        >("ManaCircle/ManaPieceType.json", x => x.Id);

        Task<MasterAssetData<int, CharaLimitBreak>> charaLimitBreak = GetAndAddTask<
            int,
            CharaLimitBreak
        >("ManaCircle/CharaLimitBreak.json", x => x.Id);

        Task<MasterAssetData<int, StoryData>> dragonStories = GetAndAddTask<int, StoryData>(
            "Story/DragonStories.json",
            x => x.id
        );

        Task<MasterAssetData<int, StoryData>> charaStories = GetAndAddTask<int, StoryData>(
            "Story/CharaStories.json",
            x => x.id
        );

        Task<MasterAssetData<int, UnitStory>> unitStory = GetAndAddTask<int, UnitStory>(
            "Story/UnitStory.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestStory>> questStory = GetAndAddTask<int, QuestStory>(
            "Story/QuestStory.json",
            x => x.Id
        );

        Task<MasterAssetData<int, EventStory>> eventStory = GetAndAddTask<int, EventStory>(
            "Story/EventStory.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestStoryRewardInfo>> questStoryRewardInfo = GetAndAddTask<
            int,
            QuestStoryRewardInfo
        >("Story/QuestStoryRewardInfo.json", x => x.Id);

        Task<MasterAssetData<string, QuestEnemies>> questEnemies = GetAndAddTask<
            string,
            QuestEnemies
        >("Enemy/QuestEnemies.json", x => x.AreaName);

        Task<MasterAssetData<int, EnemyParam>> enemyParam = GetAndAddTask<int, EnemyParam>(
            "Enemy/EnemyParam.json",
            x => x.Id
        );

        Task<MasterAssetData<int, EnemyData>> enemyData = GetAndAddTask<int, EnemyData>(
            "Enemy/EnemyData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestDropInfo>> questDrops = GetAndAddTask<int, QuestDropInfo>(
            "QuestDrops/QuestDrops.json",
            x => x.QuestId
        );

        Task<MasterAssetData<int, QuestBonusReward>> questBonusRewards = GetAndAddTask<
            int,
            QuestBonusReward
        >("QuestDrops/QuestBonusRewards.json", x => x.QuestId);

        Task<MasterAssetData<int, QuestRewardData>> questRewardData = GetAndAddTask<
            int,
            QuestRewardData
        >("QuestRewards/QuestRewardData.json", x => x.Id);

        Task<MasterAssetData<int, QuestScoreMissionRewardInfo>> questScoreMissionRewardInfo =
            GetAndAddTask<int, QuestScoreMissionRewardInfo>(
                "QuestRewards/QuestScoreMissionRewardInfo.json",
                x => x.Id
            );

        Task<MasterAssetData<int, QuestScoreMissionData>> questScoreMissionData = GetAndAddTask<
            int,
            QuestScoreMissionData
        >("QuestRewards/QuestScoreMissionData.json", x => x.Id);

        Task<MasterAssetData<int, EventData>> eventData = GetAndAddTask<int, EventData>(
            "Event/EventData.json",
            x => x.Id
        );

        Task<MasterAssetData<int, EventTradeGroup>> eventTradeGroup = GetAndAddTask<
            int,
            EventTradeGroup
        >("Event/EventTradeGroup.json", x => x.Id);

        Task<MasterAssetGroup<int, int, BuildEventReward>> buildEventReward =
            MasterAssetGroup.LoadAsync<int, int, BuildEventReward>(
                "Event/BuildEventReward.json",
                x => x.Id
            );

        Task<MasterAssetGroup<int, int, RaidEventReward>> raidEventReward =
            MasterAssetGroup.LoadAsync<int, int, RaidEventReward>(
                "Event/RaidEventReward.json",
                x => x.Id
            );

        Task<MasterAssetData<int, CombatEventLocation>> combatEventLocation = GetAndAddTask<
            int,
            CombatEventLocation
        >("Event/CombatEventLocation.json", x => x.Id);

        Task<MasterAssetData<int, CombatEventLocationReward>> combatEventLocationReward =
            GetAndAddTask<int, CombatEventLocationReward>(
                "Event/CombatEventLocationReward.json",
                x => x.Id
            );

        Task<MasterAssetData<int, EventItem<BuildEventItemType>>> buildEventItem = GetAndAddTask<
            int,
            EventItem<BuildEventItemType>
        >("Event/BuildEventItem.json", x => x.Id);

        Task<MasterAssetData<int, EventItem<CombatEventItemType>>> combatEventItem = GetAndAddTask<
            int,
            EventItem<CombatEventItemType>
        >("Event/CombatEventItem.json", x => x.Id);

        Task<MasterAssetData<int, RaidEventItem>> raidEventItem = GetAndAddTask<int, RaidEventItem>(
            "Event/RaidEventItem.json",
            x => x.Id
        );

        Task<MasterAssetData<int, EventItem<SimpleEventItemType>>> simpleEventItem = GetAndAddTask<
            int,
            EventItem<SimpleEventItemType>
        >("Event/SimpleEventItem.json", x => x.Id);

        Task<MasterAssetData<int, EventItem<ExRushEventItemType>>> exRushEventItem = GetAndAddTask<
            int,
            EventItem<ExRushEventItemType>
        >("Event/ExRushEventItem.json", x => x.Id);

        Task<MasterAssetData<int, EventItem<ExHunterEventItemType>>> exHunterEventItem =
            GetAndAddTask<int, EventItem<ExHunterEventItemType>>(
                "Event/ExHunterEventItem.json",
                x => x.Id
            );

        Task<MasterAssetData<int, EventItem<EarnEventItemType>>> earnEventItem = GetAndAddTask<
            int,
            EventItem<EarnEventItemType>
        >("Event/EarnEventItem.json", x => x.Id);

        Task<MasterAssetData<int, EventItem<CollectEventItemType>>> collectEventItem =
            GetAndAddTask<int, EventItem<CollectEventItemType>>(
                "Event/CollectEventItem.json",
                x => x.Id
            );

        Task<MasterAssetData<int, EventItem<Clb01EventItemType>>> clb01EventItem = GetAndAddTask<
            int,
            EventItem<Clb01EventItemType>
        >("Event/Clb01EventItem.json", x => x.Id);

        Task<MasterAssetData<int, EventItem<BattleRoyalEventItemType>>> battleRoyalEventItem =
            GetAndAddTask<int, EventItem<BattleRoyalEventItemType>>(
                "Event/BattleRoyalEventItem.json",
                x => x.Id
            );

        Task<MasterAssetData<int, EventPassive>> eventPassive = GetAndAddTask<int, EventPassive>(
            "Event/EventPassive.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestScoringEnemy>> questScoringEnemy = GetAndAddTask<
            int,
            QuestScoringEnemy
        >("Event/QuestScoringEnemy.json", x => x.Id);

        Task<MasterAssetData<int, DmodeQuestFloor>> dmodeQuestFloor = GetAndAddTask<
            int,
            DmodeQuestFloor
        >("Dmode/DmodeQuestFloor.json", x => x.Id);

        Task<MasterAssetData<int, DmodeDungeonArea>> dmodeDungeonArea = GetAndAddTask<
            int,
            DmodeDungeonArea
        >("Dmode/DmodeDungeonArea.json", x => x.Id);

        Task<MasterAssetData<int, DmodeDungeonTheme>> dmodeDungeonTheme = GetAndAddTask<
            int,
            DmodeDungeonTheme
        >("Dmode/DmodeDungeonTheme.json", x => x.Id);

        Task<MasterAssetData<int, DmodeEnemyTheme>> dmodeEnemyTheme = GetAndAddTask<
            int,
            DmodeEnemyTheme
        >("Dmode/DmodeEnemyTheme.json", x => x.Id);

        Task<MasterAssetData<string, DmodeAreaInfo>> dmodeAreaInfo = GetAndAddTask<
            string,
            DmodeAreaInfo
        >("Dmode/DmodeAreaInfo.json", x => x.AreaName);

        Task<MasterAssetData<int, DmodeEnemyParam>> dmodeEnemyParam = GetAndAddTask<
            int,
            DmodeEnemyParam
        >("Dmode/DmodeEnemyParam.json", x => x.Id);

        Task<MasterAssetData<int, DmodeCharaLevel>> dmodeCharaLevel = GetAndAddTask<
            int,
            DmodeCharaLevel
        >("Dmode/DmodeCharaLevel.json", x => x.Id);

        Task<MasterAssetData<int, DmodeWeapon>> dmodeWeapon = GetAndAddTask<int, DmodeWeapon>(
            "Dmode/DmodeWeapon.json",
            x => x.Id
        );

        Task<MasterAssetData<int, DmodeAbilityCrest>> dmodeAbilityCrest = GetAndAddTask<
            int,
            DmodeAbilityCrest
        >("Dmode/DmodeAbilityCrest.json", x => x.Id);

        Task<MasterAssetData<int, DmodeStrengthParam>> dmodeStrengthParam = GetAndAddTask<
            int,
            DmodeStrengthParam
        >("Dmode/DmodeStrengthParam.json", x => x.Id);

        Task<MasterAssetData<int, DmodeStrengthSkill>> dmodeStrengthSkill = GetAndAddTask<
            int,
            DmodeStrengthSkill
        >("Dmode/DmodeStrengthSkill.json", x => x.Id);

        Task<MasterAssetData<int, DmodeStrengthAbility>> dmodeStrengthAbility = GetAndAddTask<
            int,
            DmodeStrengthAbility
        >("Dmode/DmodeStrengthAbility.json", x => x.Id);

        Task<MasterAssetData<int, DmodeDungeonItemData>> dmodeDungeonItemData = GetAndAddTask<
            int,
            DmodeDungeonItemData
        >("Dmode/DmodeDungeonItemData.json", x => x.Id);

        Task<MasterAssetData<int, DmodeServitorPassiveLevel>> dmodeServitorPassiveLevel =
            GetAndAddTask<int, DmodeServitorPassiveLevel>(
                "Dmode/DmodeServitorPassiveLevel.json",
                x => x.Id
            );

        Task<MasterAssetData<int, DmodeExpeditionFloor>> dmodeExpeditionFloor = GetAndAddTask<
            int,
            DmodeExpeditionFloor
        >("Dmode/DmodeExpeditionFloor.json", x => x.Id);

        Task<MasterAssetData<int, UserLevel>> userLevel = GetAndAddTask<int, UserLevel>(
            "User/UserLevel.json",
            x => x.Id
        );

        Task<MasterAssetData<int, QuestScheduleInfo>> questScheduleInfo = GetAndAddTask<
            int,
            QuestScheduleInfo
        >("QuestSchedule/QuestScheduleInfo.json", x => x.Id);

        Task<MasterAssetData<int, RankingData>> rankingData = GetAndAddTask<int, RankingData>(
            "TimeAttack/RankingData.json",
            x => x.QuestId
        );

        Task<MasterAssetData<int, RankingTierReward>> rankingTierReward = GetAndAddTask<
            int,
            RankingTierReward
        >("TimeAttack/RankingTierReward.json", x => x.Id);

        Task<MasterAssetData<int, QuestWallDetail>> questWallDetail = GetAndAddTask<
            int,
            QuestWallDetail
        >("Wall/QuestWallDetail.json", x => x.Id);

        Task<MasterAssetData<int, QuestWallMonthlyReward>> questWallMonthlyReward = GetAndAddTask<
            int,
            QuestWallMonthlyReward
        >("Wall/QuestWallMonthlyReward.json", x => x.TotalWallLevel);

        await Task.WhenAll(tasks);

        CharaData = charaData.Result;
        DragonData = dragonData.Result;
        DragonRarity = dragonRarity.Result;
        QuestData = questData.Result;
        MaterialData = materialData.Result;
        FortPlant = fortPlant.Result;
        WeaponBody = weaponBody.Result;
        WeaponBodyBuildupGroup = weaponBodyBuildupGroup.Result;
        WeaponBodyBuildupLevel = weaponBodyBuildupLevel.Result;
        WeaponPassiveAbility = weaponPassiveAbility.Result;
        WeaponBodyRarity = weaponBodyRarity.Result;
        WeaponSkin = weaponSkin.Result;
        AbilityCrestBuildupGroup = abilityCrestBuildupGroup.Result;
        AbilityCrestBuildupLevel = abilityCrestBuildupLevel.Result;
        AbilityCrestRarity = abilityCrestRarity.Result;
        AbilityCrest = abilityCrest.Result;
        QuestEventGroup = questEventGroup.Result;
        QuestEvent = questEvent.Result;
        QuestTreasureData = questTreasureData.Result;
        UseItem = useItem.Result;
        AbilityData = abilityData.Result;
        AbilityLimitedGroup = abilityLimitedGroup.Result;
        ExAbilityData = exAbilityData.Result;
        UnionAbility = unionAbility.Result;
        SkillData = skillData.Result;
        AlbumMission = albumMission.Result;
        BeginnerMission = beginnerMission.Result;
        DailyMission = dailyMission.Result;
        DrillMission = drillMission.Result;
        DrillMissionGroup = drillMissionGroup.Result;
        MainStoryMission = mainStoryMission.Result;
        MainStoryMissionGroup = mainStoryMissionGroup.Result;
        MemoryEventMission = memoryEventMission.Result;
        NormalMission = normalMission.Result;
        PeriodMission = periodMission.Result;
        SpecialMission = specialMission.Result;
        SpecialMissionGroup = specialMissionGroup.Result;
        MissionProgressionInfo = missionProgressionInfo.Result;
        MainStoryMissionGroupRewards = mainStoryMissionGroupRewards.Result;
        StampData = stampData.Result;
        NormalShop = normalShop.Result;
        SpecialShop = specialShop.Result;
        MaterialShopDaily = materialShopDaily.Result;
        MaterialShopWeekly = materialShopWeekly.Result;
        MaterialShopMonthly = materialShopMonthly.Result;
        AbilityCrestTrade = abilityCrestTrade.Result;
        TreasureTrade = treasureTrade.Result;
        EventTreasureTrade = eventTreasureTrade.Result;
        LoginBonusData = loginBonusData.Result;
        LoginBonusReward = loginBonusReward.Result;
        ManaNode = manaNode.Result;
        ManaPieceMaterial = manaPieceMaterial.Result;
        ManaPieceType = manaPieceType.Result;
        CharaLimitBreak = charaLimitBreak.Result;
        DragonStories = dragonStories.Result;
        CharaStories = charaStories.Result;
        UnitStory = unitStory.Result;
        QuestStory = questStory.Result;
        EventStory = eventStory.Result;
        QuestStoryRewardInfo = questStoryRewardInfo.Result;
        QuestEnemies = questEnemies.Result;
        EnemyParam = enemyParam.Result;
        EnemyData = enemyData.Result;
        QuestDrops = questDrops.Result;
        QuestBonusRewards = questBonusRewards.Result;
        QuestRewardData = questRewardData.Result;
        QuestScoreMissionRewardInfo = questScoreMissionRewardInfo.Result;
        QuestScoreMissionData = questScoreMissionData.Result;
        EventData = eventData.Result;
        EventTradeGroup = eventTradeGroup.Result;
        CombatEventLocation = combatEventLocation.Result;
        CombatEventLocationReward = combatEventLocationReward.Result;
        BuildEventItem = buildEventItem.Result;
        CombatEventItem = combatEventItem.Result;
        RaidEventItem = raidEventItem.Result;
        SimpleEventItem = simpleEventItem.Result;
        ExRushEventItem = exRushEventItem.Result;
        ExHunterEventItem = exHunterEventItem.Result;
        EarnEventItem = earnEventItem.Result;
        CollectEventItem = collectEventItem.Result;
        Clb01EventItem = clb01EventItem.Result;
        BattleRoyalEventItem = battleRoyalEventItem.Result;
        EventPassive = eventPassive.Result;
        QuestScoringEnemy = questScoringEnemy.Result;
        DmodeQuestFloor = dmodeQuestFloor.Result;
        DmodeDungeonArea = dmodeDungeonArea.Result;
        DmodeDungeonTheme = dmodeDungeonTheme.Result;
        DmodeEnemyTheme = dmodeEnemyTheme.Result;
        DmodeAreaInfo = dmodeAreaInfo.Result;
        DmodeEnemyParam = dmodeEnemyParam.Result;
        DmodeCharaLevel = dmodeCharaLevel.Result;
        DmodeWeapon = dmodeWeapon.Result;
        DmodeAbilityCrest = dmodeAbilityCrest.Result;
        DmodeStrengthParam = dmodeStrengthParam.Result;
        DmodeStrengthSkill = dmodeStrengthSkill.Result;
        DmodeStrengthAbility = dmodeStrengthAbility.Result;
        DmodeDungeonItemData = dmodeDungeonItemData.Result;
        DmodeServitorPassiveLevel = dmodeServitorPassiveLevel.Result;
        DmodeExpeditionFloor = dmodeExpeditionFloor.Result;
        UserLevel = userLevel.Result;
        QuestScheduleInfo = questScheduleInfo.Result;
        RankingData = rankingData.Result;
        RankingTierReward = rankingTierReward.Result;
        QuestWallDetail = questWallDetail.Result;
        QuestWallMonthlyReward = questWallMonthlyReward.Result;
        BuildEventReward = buildEventReward.Result;
        RaidEventReward = raidEventReward.Result;

        return;

        Task<MasterAssetData<TKey, TItem>> GetAndAddTask<TKey, TItem>(
            string jsonFilename,
            Func<TItem, TKey> keySelector
        )
            where TItem : class
            where TKey : notnull
        {
            Task<MasterAssetData<TKey, TItem>> task = MasterAssetData.LoadAsync(
                jsonFilename,
                keySelector
            );

            tasks.Add(task);

            return task;
        }
    }
}
