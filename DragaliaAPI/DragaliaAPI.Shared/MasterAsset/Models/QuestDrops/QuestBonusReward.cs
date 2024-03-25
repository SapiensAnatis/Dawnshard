using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

using MemoryPack;

[MemoryPackable]
public record QuestBonusReward(int QuestId, IEnumerable<QuestBonusDrop> Bonuses);

// TODO: Extend with random quantity variation
[MemoryPackable]
public record QuestBonusDrop(EntityTypes EntityType, int Id, int Quantity);
