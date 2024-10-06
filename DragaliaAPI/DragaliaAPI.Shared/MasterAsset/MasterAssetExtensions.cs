using DragaliaAPI.Shared.MasterAsset;

[assembly: ExtendMasterAsset(nameof(MasterAsset.EventData), "Event/EventData.fixes.json")]
[assembly: ExtendMasterAsset(
    nameof(MasterAsset.MissionDailyData),
    "Missions/MissionDailyData.rewards.json",
    FeatureFlag = "BoostedDailyEndeavourRewards"
)]
[assembly: ExtendMasterAsset(
    nameof(MasterAsset.AbilityCrest),
    "AbilityCrest.custom.json",
    FeatureFlag = "CustomAbilityCrests"
)]
[assembly: ExtendMasterAsset(
    nameof(MasterAsset.QuestDropInfo),
    "QuestDrops/QuestDropInfo.customabilitycrests.json",
    FeatureFlag = "CustomAbilityCrests"
)]
