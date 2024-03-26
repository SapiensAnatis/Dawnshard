using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Enemy;

[MemoryPackable]
public partial record QuestEnemies(
    string AreaName,
    IDictionary<VariationTypes, IEnumerable<int>> Enemies
);
