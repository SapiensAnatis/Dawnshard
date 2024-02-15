using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeWeapon(
    int Id,
    int WeaponSkinId,
    WeaponTypes WeaponType,
    UnitElement ElementalType,
    int StrengthParamGroupId,
    int StrengthAbilityGroupId,
    int StrengthSkillGroupId,
    [property: JsonConverter(typeof(BoolIntJsonConverter))] bool IsDefaultWeapon
);
