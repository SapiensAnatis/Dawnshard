using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed class TimeAttackWeapon
{
    public WeaponBodies Id { get; init; }

    public int BaseId { get; init; }

    public int VariationId { get; init; }

    public int FormId { get; init; }

    public int ChangeSkillId1 { get; init; }
}
