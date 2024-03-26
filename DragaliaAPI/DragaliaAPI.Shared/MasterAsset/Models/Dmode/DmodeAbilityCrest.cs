using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeAbilityCrest(
    int Id,
    AbilityCrests AbilityCrestId,
    int StrengthParamGroupId,
    int StrengthAbilityGroupId
);
