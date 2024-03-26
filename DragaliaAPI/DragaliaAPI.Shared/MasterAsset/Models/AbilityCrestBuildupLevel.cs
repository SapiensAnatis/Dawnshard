using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestBuildupLevel(
    int Id,
    int RarityGroup,
    int Level,
    Materials BuildupMaterialId1,
    int BuildupMaterialQuantity1,
    Materials BuildupMaterialId2,
    int BuildupMaterialQuantity2,
    Materials BuildupMaterialId3,
    int BuildupMaterialQuantity3,
    int UniqueBuildupMaterialCount
)
{
    public FrozenDictionary<Materials, int> MaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(BuildupMaterialId1, BuildupMaterialQuantity1),
            new(BuildupMaterialId2, BuildupMaterialQuantity2),
            new(BuildupMaterialId3, BuildupMaterialQuantity3),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToFrozenDictionary(x => x.Key, x => x.Value);

    public bool IsUseUniqueMaterial => UniqueBuildupMaterialCount != 0;
};
