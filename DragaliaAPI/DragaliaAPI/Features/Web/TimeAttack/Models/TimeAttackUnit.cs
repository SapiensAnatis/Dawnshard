namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed class TimeAttackUnit
{
    public required TimeAttackBaseIdEntity Chara { get; init; }

    public required TimeAttackBaseIdEntity? Dragon { get; init; }

    public required TimeAttackWeapon Weapon { get; init; }

    public required TimeAttackTalisman? Talisman { get; init; }

    public required IReadOnlyList<TimeAttackAbilityCrest?> Crests { get; init; }

    public required IReadOnlyList<TimeAttackSharedSkill> SharedSkills { get; init; }
}
