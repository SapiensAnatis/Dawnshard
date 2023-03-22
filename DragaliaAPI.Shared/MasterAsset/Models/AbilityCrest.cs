using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrest(
    AbilityCrests Id,
    int AbilityCrestBuildupGroupId,
    int AbilityCrestLevelRarityGroupId,
    int Rarity,
    Materials UniqueBuildupMaterialId
)
{
    public int GetBuildupGroupId(BuildupPieceTypes type, int step) =>
        int.Parse($"{this.AbilityCrestBuildupGroupId}{(int)type:00}{step:00}");

    public int GetBuildupLevelId(int level) => int.Parse($"{this.Rarity}010{level:00}");
};
