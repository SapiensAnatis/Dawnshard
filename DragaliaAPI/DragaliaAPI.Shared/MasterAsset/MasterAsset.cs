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
[GenerateMasterAsset<Charas, CharaData>("CharaData.json", nameof(Models.CharaData.Id))]
[GenerateMasterAsset<Dragons, DragonData>("DragonData.json", nameof(Models.DragonData.Id))]
[GenerateMasterAsset<int, DragonRarity>("DragonRarity.json", nameof(Models.DragonRarity.Id))]
[GenerateMasterAsset<int, QuestData>("QuestData.json", nameof(Models.QuestData.Id))]
[GenerateMasterAsset<Materials, MaterialData>("MaterialData.json", nameof(Models.MaterialData.Id))]
[GenerateMasterAsset<int, FortPlantDetail>("FortPlantDetail.json", nameof(Models.MaterialData.Id))]
[GenerateMasterAsset<WeaponBodies, WeaponBody>("WeaponBody.json", nameof(Models.WeaponBody.Id))]
[GenerateMasterAsset<int, WeaponBodyBuildupGroup>("WeaponBodyBuildupGroup.json", nameof(Models.WeaponBodyBuildupGroup.Id))]
[GenerateMasterAsset<int, WeaponBodyBuildupLevel>("WeaponBodyBuildupLevel.json", nameof(Models.WeaponBodyBuildupLevel.Id))]
[GenerateMasterAsset<int, WeaponPassiveAbility>("WeaponPassiveAbility.json", nameof(Models.WeaponPassiveAbility.Id))]
[GenerateMasterAsset<int, WeaponBodyRarity>("WeaponBodyRarity.json", nameof(Models.WeaponBodyRarity.Id))]
[GenerateMasterAsset<int, WeaponSkin>("WeaponSkin.json", nameof(Models.WeaponSkin.Id))]
[GenerateMasterAsset<int, AbilityCrestBuildupGroup>("AbilityCrestBuildupGroup.json", nameof(Models.AbilityCrestBuildupGroup.Id))]
[GenerateMasterAsset<int, AbilityCrestBuildupLevel>("AbilityCrestBuildupLevel.json", nameof(Models.AbilityCrestBuildupLevel.Id))]
[GenerateMasterAsset<int, AbilityCrestRarity>("AbilityCrestRarity.json", nameof(Models.AbilityCrestRarity.Id))]
[GenerateMasterAsset<AbilityCrests, AbilityCrest>("AbilityCrest.json", nameof(Models.AbilityCrest.Id))]
[GenerateMasterAsset<int, QuestEventGroup>("QuestEventGroup.json", nameof(Models.QuestEventGroup.Id))]
[GenerateMasterAsset<int, QuestEvent>("QuestEvent.json", nameof(Models.QuestEvent.Id))]
[GenerateMasterAsset<int, QuestTreasureData>("QuestTreasureData.json", nameof(Models.QuestTreasureData.Id))]
[GenerateMasterAsset<int, AbilityData>("AbilityData.json", nameof(Models.AbilityData.Id))]
[GenerateMasterAsset<int, AbilityLimitedGroup>("AbilityLimitedGroup.json", nameof(Models.AbilityLimitedGroup.Id))]
[GenerateMasterAsset<int, ExAbilityData>("ExAbilityData.json", nameof(Models.ExAbilityData.Id))]
[GenerateMasterAsset<int, UnionAbility>("UnionAbility.json", nameof(Models.UnionAbility.Id))]
[GenerateMasterAsset<int, SkillData>("SkillData.json", nameof(Models.SkillData.Id))]
[GenerateMasterAsset<int, StampData>("StampData.json", nameof(Models.StampData.Id))]
[GenerateMasterAsset<int, AlbumMission>("Missions/AlbumMission.json", nameof(Models.Missions.AlbumMission.Id))]
[GenerateMasterAsset<int, NormalMission>("Missions/BeginnerMission.json", nameof(Models.Missions.NormalMission.Id))]
[GenerateMasterAsset<int, DailyMission>("Missions/DailyMission.json", nameof(Models.Missions.DailyMission.Id))]
[GenerateMasterAsset<int, DrillMission>("Missions/DrillMission.json", nameof(Models.Missions.DrillMission.Id))]
[GenerateMasterAsset<int, DrillMissionGroup>("Missions/DrillMissionGroup.json", nameof(Models.Missions.DrillMissionGroup.Id))]
[GenerateMasterAsset<int, MainStoryMission>("Missions/MainStoryMission.json", nameof(Models.Missions.MainStoryMission.Id))]
[GenerateMasterAsset<int, MainStoryMissionGroup>("Missions/MainStoryMissionGroup.json", nameof(Models.Missions.MainStoryMissionGroup.Id))]
[GenerateMasterAsset<int, MemoryEventMission>("Missions/MemoryEventMission.json", nameof(Models.Missions.MemoryEventMission.Id))]
[GenerateMasterAsset<int, NormalMission>("Missions/NormalMission.json", nameof(Models.Missions.NormalMission.Id))]
[GenerateMasterAsset<int, PeriodMission>("Missions/PeriodMission.json", nameof(Models.Missions.PeriodMission.Id))]
[GenerateMasterAsset<int, SpecialMission>("Missions/SpecialMission.json", nameof(Models.Missions.SpecialMission.Id))]
[GenerateMasterAsset<int, SpecialMissionGroup>("Missions/SpecialMissionGroup.json", nameof(Models.Missions.SpecialMissionGroup.Id))]
[GenerateMasterAsset<int, MissionProgressionInfo>("Missions/MissionProgressionInfo.json", nameof(Models.Missions.MissionProgressionInfo.Id))]
[GenerateMasterAsset<int, MainStoryMissionGroupRewards>("Missions/MainStoryMissionGroupRewards.json", nameof(Models.Missions.MainStoryMissionGroupRewards.Id))]
[GenerateMasterAsset<int, NormalShop>("Shop/NormalShop.json", nameof(Models.Shop.NormalShop.Id))]
[GenerateMasterAsset<int, SpecialShop>("Shop/SpecialShop.json", nameof(Models.Shop.SpecialShop.Id))]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShop.json", nameof(Models.Shop.MaterialShop.Id))]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShopWeekly.json", nameof(Models.Shop.MaterialShop.Id))]
[GenerateMasterAsset<int, MaterialShop>("Shop/MaterialShopMonthly.json", nameof(Models.Shop.MaterialShop.Id))]
[GenerateMasterAsset<int, AbilityCrestTrade>("Trade/AbilityCrestTrade.json", nameof(Models.Trade.AbilityCrestTrade.Id))]
[GenerateMasterAsset<UseItem, UseItemData>("Trade/UseItemData.json", nameof(Models.Trade.UseItemData.Id))]
[GenerateMasterAsset<int, TreasureTrade>("Trade/TreasureTrade.json", nameof(Models.Trade.TreasureTrade.Id))]
[GenerateMasterAsset<int, LoginBonusData>("Login/LoginBonusData.json", nameof(Models.Login.LoginBonusData.Id))]
[GenerateMasterAsset<int, LoginBonusReward>("Login/LoginBonusReward.json", nameof(Models.Login.LoginBonusReward.Id))]
[GenerateMasterAsset<int, ManaNode>("ManaCircle/ManaNode.json", nameof(Models.ManaCircle.ManaNode.MC_0))]
[GenerateMasterAsset<int, ManaPieceMaterial>("ManaCircle/ManaPieceMaterial.json", nameof(Models.ManaCircle.ManaPieceMaterial.Id))]
[GenerateMasterAsset<ManaNodeTypes, ManaPieceType>("ManaCircle/ManaPieceType.json", nameof(Models.ManaCircle.ManaPieceType.Id))]
[GenerateMasterAsset<int, CharaLimitBreak>("ManaCircle/CharaLimitBreak.json", nameof(Models.ManaCircle.CharaLimitBreak.Id))]
[GenerateMasterAsset<int, StoryData>("Story/DragonStories.json", nameof(Models.Story.StoryData.Id))]
[GenerateMasterAsset<int, StoryData>("Story/CharaStories.json", nameof(Models.Story.StoryData.Id))]
[GenerateMasterAsset<int, UnitStory>("Story/UnitStory.json", nameof(Models.Story.UnitStory.Id))]
[GenerateMasterAsset<int, QuestStory>("Story/QuestStory.json", nameof(Models.Story.QuestStory.Id))]
[GenerateMasterAsset<int, EventStory>("Story/EventStory.json", nameof(Models.Story.EventStory.Id))]
[GenerateMasterAsset<int, QuestStoryRewardInfo>("Story/QuestStoryRewardInfo.json", nameof(Models.Story.QuestStoryRewardInfo.Id))]
[GenerateMasterAsset<string, QuestEnemies>("Enemy/QuestEnemies.json", nameof(Models.Enemy.QuestEnemies.AreaName))]
[GenerateMasterAsset<int, EnemyParam>("Enemy/EnemyParam.json", nameof(Models.Enemy.EnemyParam.Id))]
[GenerateMasterAsset<int, EnemyData>("Enemy/EnemyData.json", nameof(Models.Enemy.EnemyData.Id))]
[GenerateMasterAsset<int, QuestDropInfo>("QuestDrops/QuestDropInfo.json", nameof(Models.QuestDrops.QuestDropInfo.QuestId))]
[GenerateMasterAsset<int, QuestBonusReward>("QuestDrops/QuestBonusRewardInfo.json", nameof(Models.QuestDrops.QuestBonusReward.QuestId))]
[GenerateMasterAsset<int, QuestRewardData>("QuestRewards/QuestRewardData.json", nameof(Models.QuestRewards.QuestRewardData.Id))]
[GenerateMasterAsset<int, QuestScoreMissionRewardInfo>("QuestRewards/QuestScoreMissionRewardInfo.json", nameof(Models.QuestRewards.QuestScoreMissionRewardInfo.Id))]
[GenerateMasterAsset<int, QuestScoreMissionData>("QuestRewards/QuestScoreMissionData.json", nameof(Models.QuestRewards.QuestScoreMissionData.Id))]
[GenerateMasterAsset<int, EventData>("Event/EventData.json", nameof(Models.Event.EventData.Id))]
[GenerateMasterAsset<int, EventTradeGroup>("Event/EventTradeGroup.json", nameof(Models.Event.EventTradeGroup.Id))]
// todo group generator
[GenerateMasterAsset<int, CombatEventLocation>("Event/CombatEventLocation.json", nameof(Models.Event.CombatEventLocation.Id))]
[GenerateMasterAsset<int, CombatEventLocationReward>("Event/CombatEventLocationReward.json", nameof(Models.Event.CombatEventLocationReward.Id))]
[GenerateMasterAsset<int, EventItem<BuildEventItemType>>("Event/BuildEventItem.json", nameof(Models.Event.EventItem<BuildEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<CombatEventItemType>>("Event/CombatEventItem.json", nameof(Models.Event.EventItem<CombatEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<RaidEventItemType>>("Event/RaidEventItem.json", nameof(Models.Event.EventItem<RaidEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<SimpleEventItemType>>("Event/SimpleEventItem.json", nameof(Models.Event.EventItem<SimpleEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<ExRushEventItemType>>("Event/ExRushEventItem.json", nameof(Models.Event.EventItem<ExRushEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<ExHunterEventItemType>>("Event/ExHunterEventItem.json", nameof(Models.Event.EventItem<ExHunterEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<EarnEventItemType>>("Event/EarnEventItem.json", nameof(Models.Event.EventItem<EarnEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<CollectEventItemType>>("Event/CollectEventItem.json", nameof(Models.Event.EventItem<CollectEventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<Clb01EventItemType>>("Event/Clb01EventItem.json", nameof(Models.Event.EventItem<Clb01EventItemType>.Id))]
[GenerateMasterAsset<int, EventItem<BattleRoyalEventItemType>>("Event/BattleRoyalEventItem.json", nameof(Models.Event.EventItem<BattleRoyalEventItemType>.Id))]
[GenerateMasterAsset<int, EventPassive>("Event/EventPassive.json", nameof(Models.Event.EventPassive.Id))]
[GenerateMasterAsset<int, QuestScoringEnemy>("Event/QuestScoringEnemy.json", nameof(Models.Event.QuestScoringEnemy.Id))]
[GenerateMasterAsset<int, DmodeQuestFloor>("Dmode/DmodeQuestFloor.json", nameof(Models.Dmode.DmodeQuestFloor.Id))]
[GenerateMasterAsset<int, DmodeDungeonArea>("Dmode/DmodeDungeonArea.json", nameof(Models.Dmode.DmodeDungeonArea.Id))]
[GenerateMasterAsset<int, DmodeDungeonTheme>("Dmode/DmodeDungeonTheme.json", nameof(Models.Dmode.DmodeDungeonTheme.Id))]
[GenerateMasterAsset<int, DmodeEnemyTheme>("Dmode/DmodeEnemyTheme.json", nameof(Models.Dmode.DmodeEnemyTheme.Id))]
[GenerateMasterAsset<string, DmodeAreaInfo>("Dmode/DmodeAreaInfo.json", nameof(Models.Dmode.DmodeAreaInfo.AreaName))]
[GenerateMasterAsset<int, DmodeEnemyParam>("Dmode/DmodeEnemyParam.json", nameof(Models.Dmode.DmodeEnemyParam.Id))]
[GenerateMasterAsset<int, DmodeCharaLevel>("Dmode/DmodeCharaLevel.json", nameof(Models.Dmode.DmodeCharaLevel.Id))]
[GenerateMasterAsset<int, DmodeWeapon>("Dmode/DmodeWeapon.json", nameof(Models.Dmode.DmodeWeapon.Id))]
[GenerateMasterAsset<int, DmodeAbilityCrest>("Dmode/DmodeAbilityCrest.json", nameof(Models.Dmode.DmodeAbilityCrest.Id))]
[GenerateMasterAsset<int, DmodeStrengthParam>("Dmode/DmodeStrengthParam.json", nameof(Models.Dmode.DmodeStrengthParam.Id))]
[GenerateMasterAsset<int, DmodeStrengthSkill>("Dmode/DmodeStrengthSkill.json", nameof(Models.Dmode.DmodeStrengthSkill.Id))]
[GenerateMasterAsset<int, DmodeStrengthAbility>("Dmode/DmodeStrengthAbility.json", nameof(Models.Dmode.DmodeStrengthAbility.Id))]
[GenerateMasterAsset<int, DmodeDungeonItemData>("Dmode/DmodeDungeonItemData.json", nameof(Models.Dmode.DmodeDungeonItemData.Id))]
[GenerateMasterAsset<int, DmodeServitorPassiveLevel>("Dmode/DmodeServitorPassiveLevel.json", nameof(Models.Dmode.DmodeServitorPassiveLevel.Id))]
[GenerateMasterAsset<int, DmodeExpeditionFloor>("Dmode/DmodeExpeditionFloor.json", nameof(Models.Dmode.DmodeExpeditionFloor.Id))]
[GenerateMasterAsset<int, UserLevel>("User/UserLevel.json", nameof(Models.User.UserLevel.Id))]
[GenerateMasterAsset<int, QuestScheduleInfo>("User/QuestScheduleInfo.json", nameof(Models.QuestSchedule.QuestScheduleInfo.Id))]
[GenerateMasterAsset<int, RankingData>("TimeAttack/RankingData.json", nameof(Models.TimeAttack.RankingData.GroupId))]
[GenerateMasterAsset<int, RankingTierReward>("TimeAttack/RankingTierReward.json", nameof(Models.TimeAttack.RankingTierReward.Id))]
[GenerateMasterAsset<int, QuestWallDetail>("Wall/QuestWallDetail.json", nameof(Models.Wall.QuestWallDetail.Id))]
[GenerateMasterAsset<int, QuestWallMonthlyReward>("Wall/QuestWallMonthlyReward.json", nameof(Models.Wall.QuestWallMonthlyReward.Id))]
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
