namespace DragaliaAPI.Features.Web.TimeAttack.Models;

/// <summary>
/// Represents an entity with an ID, base ID, and VariationType.
/// This includes characters, dragons, and weapons.
/// </summary>
internal sealed class TimeAttackBaseIdEntity
{
    public int Id { get; init; }

    public int BaseId { get; init; }

    public int VariationId { get; init; }
}
