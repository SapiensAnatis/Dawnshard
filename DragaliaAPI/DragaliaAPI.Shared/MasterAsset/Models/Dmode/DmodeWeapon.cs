using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeWeapon(
    int Id,
    int WeaponSkinId,
    WeaponTypes WeaponType,
    UnitElement ElementalType,
    int StrengthParamGroupId,
    int StrengthAbilityGroupId,
    int StrengthSkillGroupId,
    bool IsDefaultWeapon
);
