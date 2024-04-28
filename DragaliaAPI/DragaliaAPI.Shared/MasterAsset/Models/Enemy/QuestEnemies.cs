using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Enemy;

public record QuestEnemies(string AreaName, IDictionary<VariationTypes, IEnumerable<int>> Enemies);
