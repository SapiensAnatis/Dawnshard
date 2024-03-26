using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeServitorPassiveLevel(
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
    [MemoryPackIgnore]
    public readonly (EntityTypes Type, int Id, int Quantity)[] NeededMaterials =
    {
        (ReleaseEntityType1, ReleaseEntityId1, ReleaseEntityQuantity1),
        (ReleaseEntityType2, ReleaseEntityId2, ReleaseEntityQuantity2)
    };
};
