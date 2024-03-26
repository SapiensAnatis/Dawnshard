using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

[MemoryPackable]
public partial record QuestStoryRewardInfo(int Id, QuestStoryReward[] Rewards);

[MemoryPackable]
public partial record QuestStoryReward(EntityTypes Type, int Id, int Quantity);
