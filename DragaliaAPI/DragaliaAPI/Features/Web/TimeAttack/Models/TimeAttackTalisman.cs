using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed class TimeAttackTalisman
{
    public Talismans Id { get; init; }

    public UnitElement Element { get; init; }

    public WeaponTypes WeaponType { get; init; }

    public int Ability1Id { get; init; }

    public int Ability2Id { get; init; }
}
