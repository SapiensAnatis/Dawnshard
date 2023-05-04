using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestBuildupGroup(
    int Id,
    int AbilityCrestBuildupGroupId,
    BuildupPieceTypes BuildupPieceType,
    int Step,
    int BuildupDewPoint,
    Materials BuildupMaterialId1,
    int BuildupMaterialQuantity1,
    Materials BuildupMaterialId2,
    int BuildupMaterialQuantity2,
    Materials BuildupMaterialId3,
    int BuildupMaterialQuantity3,
    int UniqueBuildupMaterialCount
)
{
    public Dictionary<Materials, int> MaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(BuildupMaterialId1, BuildupMaterialQuantity1),
            new(BuildupMaterialId2, BuildupMaterialQuantity2),
            new(BuildupMaterialId3, BuildupMaterialQuantity3),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToDictionary(x => x.Key, x => x.Value);

    public bool IsUseUniqueMaterial => UniqueBuildupMaterialCount != 0;

    public bool IsUseDewpoint => BuildupDewPoint != 0;
};
