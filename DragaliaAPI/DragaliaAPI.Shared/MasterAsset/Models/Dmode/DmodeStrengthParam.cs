using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeStrengthParam(
    int Id,
    int StrengthParamGroupId,
    int Scarcity,
    int Hp,
    int Atk
);
