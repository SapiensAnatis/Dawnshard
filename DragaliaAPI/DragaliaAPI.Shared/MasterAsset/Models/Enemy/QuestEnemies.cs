using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Enemy;

using MemoryPack;

[MemoryPackable]
public record QuestEnemies(string AreaName, IDictionary<VariationTypes, IEnumerable<int>> Enemies);
