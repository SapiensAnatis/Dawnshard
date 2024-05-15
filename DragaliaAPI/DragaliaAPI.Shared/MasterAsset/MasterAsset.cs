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
using DragaliaAPI.Shared.MasterAsset.Models.Summon;
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
[GenerateMasterAsset<CharaData>("CharaData.json")]
[GenerateMasterAsset<DragonData>("DragonData.json")]
[GenerateMasterAsset<DragonRarity>("DragonRarity.json")]
[GenerateMasterAsset<QuestData>("QuestData.json")]
[GenerateMasterAsset<MaterialData>("MaterialData.json")]
[GenerateMasterAsset<FortPlantDetail>("FortPlantDetail.json")]
[GenerateMasterAsset<WeaponBody>("WeaponBody.json")]
[GenerateMasterAsset<WeaponBodyBuildupGroup>("WeaponBodyBuildupGroup.json")]
[GenerateMasterAsset<WeaponBodyBuildupLevel>("WeaponBodyBuildupLevel.json")]
[GenerateMasterAsset<WeaponPassiveAbility>("WeaponPassiveAbility.json")]
[GenerateMasterAsset<WeaponBodyRarity>("WeaponBodyRarity.json")]
[GenerateMasterAsset<WeaponSkin>("WeaponSkin.json")]
[GenerateMasterAsset<AbilityCrestBuildupGroup>("AbilityCrestBuildupGroup.json")]
[GenerateMasterAsset<AbilityCrestBuildupLevel>("AbilityCrestBuildupLevel.json")]
[GenerateMasterAsset<AbilityCrestRarity>("AbilityCrestRarity.json")]
[GenerateMasterAsset<AbilityCrest>("AbilityCrest.json")]
[GenerateMasterAsset<QuestEventGroup>("QuestEventGroup.json")]
[GenerateMasterAsset<QuestEvent>("QuestEvent.json")]
[GenerateMasterAsset<QuestTreasureData>("QuestTreasureData.json")]
[GenerateMasterAsset<AbilityData>("AbilityData.json")]
[GenerateMasterAsset<AbilityLimitedGroup>("AbilityLimitedGroup.json")]
[GenerateMasterAsset<ExAbilityData>("ExAbilityData.json")]
[GenerateMasterAsset<UnionAbility>("UnionAbility.json")]
[GenerateMasterAsset<SkillData>("SkillData.json")]
[GenerateMasterAsset<StampData>("StampData.json")]
[GenerateMasterAsset<UseItemData>("UseItem.json")]
[GenerateMasterAsset<EmblemData>("EmblemData.json")]
[GenerateMasterAsset<AlbumMission>("Missions/MissionAlbumData.json")]
[GenerateMasterAsset<NormalMission>("Missions/MissionBeginnerData.json")]
[GenerateMasterAsset<DailyMission>("Missions/MissionDailyData.json")]
[GenerateMasterAsset<DrillMission>("Missions/MissionDrillData.json")]
[GenerateMasterAsset<DrillMissionGroup>("Missions/MissionDrillGroup.json")]
[GenerateMasterAsset<MainStoryMission>("Missions/MissionMainStoryData.json")]
[GenerateMasterAsset<MainStoryMissionGroup>("Missions/MissionMainStoryGroup.json")]
[GenerateMasterAsset<MemoryEventMission>("Missions/MissionMemoryEventData.json")]
[GenerateMasterAsset<NormalMission>("Missions/MissionNormalData.json")]
[GenerateMasterAsset<PeriodMission>("Missions/MissionPeriodData.json")]
[GenerateMasterAsset<SpecialMission>("Missions/MissionSpecialData.json")]
[GenerateMasterAsset<SpecialMissionGroup>("Missions/MissionSpecialGroup.json")]
[GenerateMasterAsset<MissionProgressionInfo>("Missions/MissionProgressionInfo.json")]
[GenerateMasterAsset<MainStoryMissionGroupRewards>("Missions/MainStoryMissionGroupRewards.json")]
[GenerateMasterAsset<NormalShop>("Shop/NormalShop.json")]
[GenerateMasterAsset<SpecialShop>("Shop/SpecialShop.json")]
[GenerateMasterAsset<MaterialShop>("Shop/MaterialShopDaily.json")]
[GenerateMasterAsset<MaterialShop>("Shop/MaterialShopWeekly.json")]
[GenerateMasterAsset<MaterialShop>("Shop/MaterialShopMonthly.json")]
[GenerateMasterAsset<AbilityCrestTrade>("Trade/AbilityCrestTrade.json")]
[GenerateMasterAsset<TreasureTrade>("Trade/TreasureTrade.json")]
[GenerateMasterAsset<TreasureTrade>("Trade/EventTreasureTradeInfo.json")]
[GenerateMasterAsset<LoginBonusData>("Login/LoginBonusData.json")]
[GenerateMasterAsset<LoginBonusReward>("Login/LoginBonusReward.json")]
[GenerateMasterAsset<ManaNode>("ManaCircle/MC.json", Key = nameof(Models.ManaCircle.ManaNode.MC_0))]
[GenerateMasterAsset<ManaPieceMaterial>("ManaCircle/ManaPieceMaterial.json")]
[GenerateMasterAsset<ManaPieceType>("ManaCircle/ManaPieceType.json")]
[GenerateMasterAsset<CharaLimitBreak>("ManaCircle/CharaLimitBreak.json")]
[GenerateMasterAsset<StoryData>("Story/DragonStories.json")]
[GenerateMasterAsset<StoryData>("Story/CharaStories.json")]
[GenerateMasterAsset<UnitStory>("Story/UnitStory.json")]
[GenerateMasterAsset<QuestStory>("Story/QuestStory.json")]
[GenerateMasterAsset<EventStory>("Story/EventStory.json")]
[GenerateMasterAsset<QuestStoryRewardInfo>("Story/QuestStoryRewardInfo.json")]
[GenerateMasterAsset<QuestEnemies>("Enemy/QuestEnemies.json", Key = nameof(Models.Enemy.QuestEnemies.AreaName))]
[GenerateMasterAsset<EnemyParam>("Enemy/EnemyParam.json")]
[GenerateMasterAsset<EnemyData>("Enemy/EnemyData.json")]
[GenerateMasterAsset<QuestDropInfo>("QuestDrops/QuestDropInfo.json", Key = nameof(Models.QuestDrops.QuestDropInfo.QuestId))]
[GenerateMasterAsset<QuestBonusReward>("QuestDrops/QuestBonusRewardInfo.json", Key = nameof(Models.QuestDrops.QuestBonusReward.QuestId))]
[GenerateMasterAsset<QuestRewardData>("QuestRewards/QuestRewardData.json")]
[GenerateMasterAsset<QuestScoreMissionRewardInfo>("QuestRewards/QuestScoreMissionRewardInfo.json")]
[GenerateMasterAsset<QuestScoreMissionData>("QuestRewards/QuestScoreMissionData.json")]
[GenerateMasterAsset<EventData>("Event/EventData.json")]
[GenerateMasterAsset<EventTradeGroup>("Event/EventTradeGroup.json")]
[GenerateMasterAsset<BuildEventReward>("Event/BuildEventReward.json", Group = true)]
[GenerateMasterAsset<RaidEventReward>("Event/RaidEventReward.json", Group = true)]
[GenerateMasterAsset<CombatEventLocation>("Event/CombatEventLocation.json")]
[GenerateMasterAsset<CombatEventLocationReward>("Event/CombatEventLocationReward.json")]
[GenerateMasterAsset<EventItem<BuildEventItemType>>("Event/BuildEventItem.json")]
[GenerateMasterAsset<EventItem<CombatEventItemType>>("Event/CombatEventItem.json")]
[GenerateMasterAsset<RaidEventItem>("Event/RaidEventItem.json")]
[GenerateMasterAsset<EventItem<SimpleEventItemType>>("Event/SimpleEventItem.json")]
[GenerateMasterAsset<EventItem<ExRushEventItemType>>("Event/ExRushEventItem.json")]
[GenerateMasterAsset<EventItem<ExHunterEventItemType>>("Event/ExHunterEventItem.json")]
[GenerateMasterAsset<EventItem<EarnEventItemType>>("Event/EarnEventItem.json")]
[GenerateMasterAsset<EventItem<CollectEventItemType>>("Event/CollectEventItem.json")]
[GenerateMasterAsset<EventItem<Clb01EventItemType>>("Event/Clb01EventItem.json")]
[GenerateMasterAsset<EventItem<BattleRoyalEventItemType>>("Event/BattleRoyalEventItem.json")]
[GenerateMasterAsset<EventPassive>("Event/EventPassive.json")]
[GenerateMasterAsset<QuestScoringEnemy>("Event/QuestScoringEnemy.json")]
[GenerateMasterAsset<DmodeQuestFloor>("Dmode/DmodeQuestFloor.json")]
[GenerateMasterAsset<DmodeDungeonArea>("Dmode/DmodeDungeonArea.json")]
[GenerateMasterAsset<DmodeDungeonTheme>("Dmode/DmodeDungeonTheme.json")]
[GenerateMasterAsset<DmodeEnemyTheme>("Dmode/DmodeEnemyTheme.json")]
[GenerateMasterAsset<DmodeAreaInfo>("Dmode/DmodeAreaInfo.json", Key = nameof(Models.Dmode.DmodeAreaInfo.AreaName))]
[GenerateMasterAsset<DmodeEnemyParam>("Dmode/DmodeEnemyParam.json")]
[GenerateMasterAsset<DmodeCharaLevel>("Dmode/DmodeCharaLevel.json")]
[GenerateMasterAsset<DmodeWeapon>("Dmode/DmodeWeapon.json")]
[GenerateMasterAsset<DmodeAbilityCrest>("Dmode/DmodeAbilityCrest.json")]
[GenerateMasterAsset<DmodeStrengthParam>("Dmode/DmodeStrengthParam.json")]
[GenerateMasterAsset<DmodeStrengthSkill>("Dmode/DmodeStrengthSkill.json")]
[GenerateMasterAsset<DmodeStrengthAbility>("Dmode/DmodeStrengthAbility.json")]
[GenerateMasterAsset<DmodeDungeonItemData>("Dmode/DmodeDungeonItemData.json")]
[GenerateMasterAsset<DmodeServitorPassiveLevel>("Dmode/DmodeServitorPassiveLevel.json")]
[GenerateMasterAsset<DmodeExpeditionFloor>("Dmode/DmodeExpeditionFloor.json")]
[GenerateMasterAsset<UserLevel>("User/UserLevel.json")]
[GenerateMasterAsset<QuestScheduleInfo>("QuestSchedule/QuestScheduleInfo.json")]
[GenerateMasterAsset<RankingData>("TimeAttack/RankingData.json", Key = nameof(Models.TimeAttack.RankingData.QuestId))]
[GenerateMasterAsset<RankingTierReward>("TimeAttack/RankingTierReward.json")]
[GenerateMasterAsset<QuestWallDetail>("Wall/QuestWallDetail.json")]
[GenerateMasterAsset<QuestWallMonthlyReward>("Wall/QuestWallMonthlyReward.json")]
[GenerateMasterAsset<SummonTicket>("Summon/SummonTicket.json")]
[GenerateMasterAsset<SummonData>("Summon/SummonData.json")]
// csharpier-ignore-end
public static partial class MasterAsset
{
}
