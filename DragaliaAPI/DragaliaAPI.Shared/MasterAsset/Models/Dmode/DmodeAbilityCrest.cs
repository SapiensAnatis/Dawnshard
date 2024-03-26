using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeAbilityCrest(
    int Id,
    AbilityCrests AbilityCrestId,
    int StrengthParamGroupId,
    int StrengthAbilityGroupId
);
