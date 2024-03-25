using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

using MemoryPack;

[MemoryPackable]
public record QuestStoryRewardInfo(int Id, QuestStoryReward[] Rewards);

[MemoryPackable]
public record QuestStoryReward(EntityTypes Type, int Id, int Quantity);
