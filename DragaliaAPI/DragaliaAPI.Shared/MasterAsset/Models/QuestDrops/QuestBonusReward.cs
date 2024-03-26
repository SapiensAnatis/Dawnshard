using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

[MemoryPackable]
public partial record QuestBonusReward(int QuestId, IEnumerable<QuestBonusDrop> Bonuses);

// TODO: Extend with random quantity variation
[MemoryPackable]
public partial record QuestBonusDrop(EntityTypes EntityType, int Id, int Quantity);
