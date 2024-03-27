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
using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using DragaliaAPI.Shared.MasterAsset.Models.User;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;

namespace DragaliaAPI.Shared.MasterAsset;

/// <summary>
/// Provides access to instances of <see cref="MasterAssetData{TKey,TItem}"/> to retrieve internal game data.
/// </summary>
// csharpier-ignore-start
// ReSharper disable RedundantNameQualifier
[GenerateMasterAsset<Charas, CharaData>("CharaData.json")]
[GenerateMasterAsset<Dragons, DragonData>("DragonData.json")]
[GenerateMasterAsset<int, DragonRarity>("DragonRarity.json")]
[GenerateMasterAsset<int, QuestData>("QuestData.json")]
[GenerateMasterAsset<Materials, MaterialData>("MaterialData.json")]
[GenerateMasterAsset<int, FortPlantDetail>("FortPlantDetail.json")]
[GenerateMasterAsset<WeaponBodies, WeaponBody>("WeaponBody.json")]
[GenerateMasterAsset<int, WeaponBodyBuildupGroup>("WeaponBodyBuildupGroup.json")]
[GenerateMasterAsset<int, WeaponBodyBuildupLevel>("WeaponBodyBuildupLevel.json")]
[GenerateMasterAsset<int, WeaponPassiveAbility>("WeaponPassiveAbility.json")]
[GenerateMasterAsset<int, WeaponBodyRarity>("WeaponBodyRarity.json")]
[GenerateMasterAsset<int, WeaponSkin>("WeaponSkin.json")]
[GenerateMasterAsset<int, AbilityCrestBuildupGroup>("AbilityCrestBuildupGroup.json")]
[GenerateMasterAsset<int, AbilityCrestBuildupLevel>("AbilityCrestBuildupLevel.json")]
[GenerateMasterAsset<int, AbilityCrestRarity>("AbilityCrestRarity.json")]
[GenerateMasterAsset<AbilityCrests, AbilityCrest>("AbilityCrest.json")]
[GenerateMasterAsset<int, QuestEventGroup>("QuestEventGroup.json")]
[GenerateMasterAsset<int, QuestEvent>("QuestEvent.json")]
[GenerateMasterAsset<int, QuestTreasureData>("QuestTreasureData.json")]
[GenerateMasterAsset<int, AbilityData>("AbilityData.json")]
[GenerateMasterAsset<int, AbilityLimitedGroup>("AbilityLimitedGroup.json")]
[GenerateMasterAsset<int, ExAbilityData>("ExAbilityData.json")]
[GenerateMasterAsset<int, UnionAbility>("UnionAbility.json")]
[GenerateMasterAsset<int, SkillData>("SkillData.json")]
[GenerateMasterAsset<int, StampData>("StampData.json")]
[GenerateMasterAsset<int, AlbumMission>("Missions/AlbumMission.json")]
[GenerateMasterAsset<int, NormalMission>("Missions/BeginnerMission.json")]
[GenerateMasterAsset<int, DailyMission>("Missions/DailyMission.json")]
[GenerateMasterAsset<int, DrillMission>("Missions/DrillMission.json")]
[GenerateMasterAsset<int, DrillMissionGroup>("Missions/DrillMissionGroup.json")]
[GenerateMasterAsset<int, MainStoryMission>("Missions/MainStoryMission.json")]
[GenerateMasterAsset<int, MainStoryMissionGroup>("Missions/MainStoryMissionGroup.json")]
[GenerateMasterAsset<int, MemoryEventMission>("Missions/MemoryEventMission.json")]
[GenerateMasterAsset<int, NormalMission>("Missions/NormalMission.json")]
[GenerateMasterAsset<int, PeriodMission>("Missions/PeriodMission.json")]
[GenerateMasterAsset<int, SpecialMission>("Missions/SpecialMission.json")]
[GenerateMasterAsset<int, SpecialMissionGroup>("Missions/SpecialMissionGroup.json")]
[GenerateMasterAsset<int, MissionProgressionInfo>("Missions/MissionProgressionInfo.json")]
[GenerateMasterAsset<int, MainStoryMissionGroupRewards>("Missions/MainStoryMissionGroupRewards.json")]
[GenerateMasterAsset<int, NormalShop>("Shop/NormalShop.json")]
[GenerateMasterAsset<int, SpecialShop>("Shop/SpecialShop.json")]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShop.json")]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShopWeekly.json")]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShopMonthly.json")]
[GenerateMasterAsset<int, AbilityCrestTrade>("Trade/AbilityCrestTrade.json")]
[GenerateMasterAsset<UseItem, UseItemData>("Trade/UseItemData.json")]
[GenerateMasterAsset<int, TreasureTrade>("Trade/TreasureTrade.json")]
[GenerateMasterAsset<int, LoginBonusData>("Login/LoginBonusData.json")]
[GenerateMasterAsset<int, LoginBonusReward>("Login/LoginBonusReward.json")]
[GenerateMasterAsset<int, ManaNode>("ManaCircle/ManaNode.json", nameof(Models.ManaCircle.ManaNode.MC_0))]
[GenerateMasterAsset<int, ManaPieceMaterial>("ManaCircle/ManaPieceMaterial.json")]
[GenerateMasterAsset<ManaNodeTypes, ManaPieceType>("ManaCircle/ManaPieceType.json")]
[GenerateMasterAsset<int, CharaLimitBreak>("ManaCircle/CharaLimitBreak.json")]
[GenerateMasterAsset<int, StoryData>("Story/DragonStories.json")]
[GenerateMasterAsset<int, StoryData>("Story/CharaStories.json")]
[GenerateMasterAsset<int, UnitStory>("Story/UnitStory.json")]
[GenerateMasterAsset<int, QuestStory>("Story/QuestStory.json")]
[GenerateMasterAsset<int, EventStory>("Story/EventStory.json")]
[GenerateMasterAsset<int, QuestStoryRewardInfo>("Story/QuestStoryRewardInfo.json")]
[GenerateMasterAsset<string, QuestEnemies>("Enemy/QuestEnemies.json", nameof(Models.Enemy.QuestEnemies.AreaName))]
[GenerateMasterAsset<int, EnemyParam>("Enemy/EnemyParam.json")]
[GenerateMasterAsset<int, EnemyData>("Enemy/EnemyData.json")]
[GenerateMasterAsset<int, QuestDropInfo>("QuestDrops/QuestDropInfo.json", nameof(Models.QuestDrops.QuestDropInfo.QuestId))]
[GenerateMasterAsset<int, QuestBonusReward>("QuestDrops/QuestBonusRewardInfo.json", nameof(Models.QuestDrops.QuestBonusReward.QuestId))]
[GenerateMasterAsset<int, QuestRewardData>("QuestRewards/QuestRewardData.json")]
[GenerateMasterAsset<int, QuestScoreMissionRewardInfo>("QuestRewards/QuestScoreMissionRewardInfo.json")]
[GenerateMasterAsset<int, QuestScoreMissionData>("QuestRewards/QuestScoreMissionData.json")]
[GenerateMasterAsset<int, EventData>("Event/EventData.json")]
[GenerateMasterAsset<int, EventTradeGroup>("Event/EventTradeGroup.json")]
// todo group generator
[GenerateMasterAsset<int, CombatEventLocation>("Event/CombatEventLocation.json")]
[GenerateMasterAsset<int, CombatEventLocationReward>("Event/CombatEventLocationReward.json")]
[GenerateMasterAsset<int, EventItem<BuildEventItemType>>("Event/BuildEventItem.json")]
[GenerateMasterAsset<int, EventItem<CombatEventItemType>>("Event/CombatEventItem.json")]
[GenerateMasterAsset<int, EventItem<RaidEventItemType>>("Event/RaidEventItem.json")]
[GenerateMasterAsset<int, EventItem<SimpleEventItemType>>("Event/SimpleEventItem.json")]
[GenerateMasterAsset<int, EventItem<ExRushEventItemType>>("Event/ExRushEventItem.json")]
[GenerateMasterAsset<int, EventItem<ExHunterEventItemType>>("Event/ExHunterEventItem.json")]
[GenerateMasterAsset<int, EventItem<EarnEventItemType>>("Event/EarnEventItem.json")]
[GenerateMasterAsset<int, EventItem<CollectEventItemType>>("Event/CollectEventItem.json")]
[GenerateMasterAsset<int, EventItem<Clb01EventItemType>>("Event/Clb01EventItem.json")]
[GenerateMasterAsset<int, EventItem<BattleRoyalEventItemType>>("Event/BattleRoyalEventItem.json")]
[GenerateMasterAsset<int, EventPassive>("Event/EventPassive.json")]
[GenerateMasterAsset<int, QuestScoringEnemy>("Event/QuestScoringEnemy.json")]
[GenerateMasterAsset<int, DmodeQuestFloor>("Dmode/DmodeQuestFloor.json")]
[GenerateMasterAsset<int, DmodeDungeonArea>("Dmode/DmodeDungeonArea.json")]
[GenerateMasterAsset<int, DmodeDungeonTheme>("Dmode/DmodeDungeonTheme.json")]
[GenerateMasterAsset<int, DmodeEnemyTheme>("Dmode/DmodeEnemyTheme.json")]
[GenerateMasterAsset<string, DmodeAreaInfo>("Dmode/DmodeAreaInfo.json", nameof(Models.Dmode.DmodeAreaInfo.AreaName))]
[GenerateMasterAsset<int, DmodeEnemyParam>("Dmode/DmodeEnemyParam.json")]
[GenerateMasterAsset<int, DmodeCharaLevel>("Dmode/DmodeCharaLevel.json")]
[GenerateMasterAsset<int, DmodeWeapon>("Dmode/DmodeWeapon.json")]
[GenerateMasterAsset<int, DmodeAbilityCrest>("Dmode/DmodeAbilityCrest.json")]
[GenerateMasterAsset<int, DmodeStrengthParam>("Dmode/DmodeStrengthParam.json")]
[GenerateMasterAsset<int, DmodeStrengthSkill>("Dmode/DmodeStrengthSkill.json")]
[GenerateMasterAsset<int, DmodeStrengthAbility>("Dmode/DmodeStrengthAbility.json")]
[GenerateMasterAsset<int, DmodeDungeonItemData>("Dmode/DmodeDungeonItemData.json")]
[GenerateMasterAsset<int, DmodeServitorPassiveLevel>("Dmode/DmodeServitorPassiveLevel.json")]
[GenerateMasterAsset<int, DmodeExpeditionFloor>("Dmode/DmodeExpeditionFloor.json")]
[GenerateMasterAsset<int, UserLevel>("User/UserLevel.json")]
[GenerateMasterAsset<int, QuestScheduleInfo>("User/QuestScheduleInfo.json")]
[GenerateMasterAsset<int, RankingData>("TimeAttack/RankingData.json", nameof(Models.TimeAttack.RankingData.GroupId))]
[GenerateMasterAsset<int, RankingTierReward>("TimeAttack/RankingTierReward.json")]
[GenerateMasterAsset<int, QuestWallDetail>("Wall/QuestWallDetail.json")]
[GenerateMasterAsset<int, QuestWallMonthlyReward>("Wall/QuestWallMonthlyReward.json")]
// csharpier-ignore-end
public static partial class MasterAsset
{
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
}
