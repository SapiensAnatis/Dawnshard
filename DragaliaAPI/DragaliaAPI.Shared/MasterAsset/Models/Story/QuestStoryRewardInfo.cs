using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

public record QuestStoryRewardInfo(int Id, QuestStoryReward[] Rewards);

public record QuestStoryReward(EntityTypes Type, int Id, int Quantity);
