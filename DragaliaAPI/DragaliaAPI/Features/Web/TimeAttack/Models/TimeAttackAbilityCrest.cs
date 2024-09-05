
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed record TimeAttackAbilityCrest
{
    public AbilityCrestId Id { get; init; }

    public int BaseId { get; init; }

    public int ImageNum { get; init; }
}
