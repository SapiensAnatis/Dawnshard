using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrest(
    AbilityCrests Id,
    int AbilityCrestBuildupGroupId,
    int AbilityCrestLevelRarityGroupId,
    int Rarity,
    Materials UniqueBuildupMaterialId,
    Materials DuplicateEntityId,
    int DuplicateEntityQuantity,
    EntityTypes DuplicateEntityType
)
{
    public int GetBuildupGroupId(BuildupPieceTypes type, int step) =>
        int.Parse($"{this.AbilityCrestBuildupGroupId}{(int)type:00}{step:00}");

    public int GetBuildupLevelId(int level) => int.Parse($"{this.Rarity}010{level:00}");

    public Dictionary<Materials, int> DuplicateMaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>() { new(DuplicateEntityId, DuplicateEntityQuantity) }
            .Where(x => x.Key != Materials.Empty)
            .ToDictionary(x => x.Key, x => x.Value);
};
