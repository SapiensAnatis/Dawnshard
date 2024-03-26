using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeWeapon(
    int Id,
    int WeaponSkinId,
    WeaponTypes WeaponType,
    UnitElement ElementalType,
    int StrengthParamGroupId,
    int StrengthAbilityGroupId,
    int StrengthSkillGroupId,
    bool IsDefaultWeapon
);
