using DragaliaAPI.Shared.MasterAsset;

[assembly: ExtendMasterAsset(nameof(MasterAsset.EventData), "Event/EventData.fixes.json")]
[assembly: ExtendMasterAsset(
    nameof(MasterAsset.MissionDailyData),
    "Missions/MissionDailyData.rewards.json",
    FeatureFlag = "BoostedDailyEndeavourRewards"
)]
