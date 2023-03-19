using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestBuildupGroup(
    int Id,
    int AbilityCrestBuildupGroupId,
    BuildupPieceTypes BuildupPieceType,
    int Step,
    int BuildupDewPoint,
    int BuildupMaterialId1,
    int BuildupMaterialQuantity1,
    int BuildupMaterialId2,
    int BuildupMaterialQuantity2,
    int BuildupMaterialId3,
    int BuildupMaterialQuantity3,
    int UniqueBuildupMaterialCount
);
