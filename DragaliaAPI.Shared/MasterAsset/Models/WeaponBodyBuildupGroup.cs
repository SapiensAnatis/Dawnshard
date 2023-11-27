using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record WeaponBodyBuildupGroup(
    int Id,
    int WeaponBodyBuildupGroupId,
    BuildupPieceTypes BuildupPieceType,
    int Step,
    int UnlockConditionLimitBreakCount,
    int RewardWeaponSkinNo,
    long BuildupCoin,
    Materials BuildupMaterialId1,
    int BuildupMaterialQuantity1,
    Materials BuildupMaterialId2,
    int BuildupMaterialQuantity2,
    Materials BuildupMaterialId3,
    int BuildupMaterialQuantity3,
    Materials BuildupMaterialId4,
    int BuildupMaterialQuantity4,
    Materials BuildupMaterialId5,
    int BuildupMaterialQuantity5,
    Materials BuildupMaterialId6,
    int BuildupMaterialQuantity6,
    Materials BuildupMaterialId7,
    int BuildupMaterialQuantity7
)
{
    public FrozenDictionary<Materials, int> MaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(BuildupMaterialId1, BuildupMaterialQuantity1),
            new(BuildupMaterialId2, BuildupMaterialQuantity2),
            new(BuildupMaterialId3, BuildupMaterialQuantity3),
            new(BuildupMaterialId4, BuildupMaterialQuantity4),
            new(BuildupMaterialId5, BuildupMaterialQuantity5),
            new(BuildupMaterialId6, BuildupMaterialQuantity6),
            new(BuildupMaterialId7, BuildupMaterialQuantity7),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToFrozenDictionary(x => x.Key, x => x.Value);
};
