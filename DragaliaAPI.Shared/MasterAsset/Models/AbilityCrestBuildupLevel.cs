namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestBuildupLevel(
    int Id,
    int RarityGroup,
    int Level,
    int BuildupMaterialId1,
    int BuildupMaterialQuantity1,
    int BuildupMaterialId2,
    int BuildupMaterialQuantity2,
    int BuildupMaterialId3,
    int BuildupMaterialQuantity3,
    int UniqueBuildupMaterialCount
);
