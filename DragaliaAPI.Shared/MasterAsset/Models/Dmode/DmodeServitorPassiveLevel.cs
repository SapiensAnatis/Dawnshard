using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeServitorPassiveLevel(
    int Id,
    DmodeServitorPassiveType PassiveNum,
    int Level,
    float UpValue,
    EntityTypes ReleaseEntityType1,
    int ReleaseEntityId1,
    int ReleaseEntityQuantity1,
    EntityTypes ReleaseEntityType2,
    int ReleaseEntityId2,
    int ReleaseEntityQuantity2
)
{
    public readonly (EntityTypes Type, int Id, int Quantity)[] NeededMaterials =
    {
        (ReleaseEntityType1, ReleaseEntityId1, ReleaseEntityQuantity1),
        (ReleaseEntityType2, ReleaseEntityId2, ReleaseEntityQuantity2)
    };
};
