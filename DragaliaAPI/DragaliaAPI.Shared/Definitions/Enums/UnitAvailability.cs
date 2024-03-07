namespace DragaliaAPI.Shared.Definitions.Enums;

/// <summary>
/// Represents a unit's availability from a summoning perspective.
/// </summary>
public enum UnitAvailability
{
    /// <summary>
    /// The unit is always able to be summoned.
    /// </summary>
    Permanent,

    /// <summary>
    /// The unit can only be summoned on Gala banners.
    /// </summary>
    Gala,

    /// <summary>
    /// The unit can only be summoned on particular limited banners.
    /// </summary>
    Limited,

    /// <summary>
    /// The unit can never be summoned, but is available from completing a main story quest.
    /// </summary>
    Story,

    /// <summary>
    /// The unit can never be summoned.
    /// </summary>
    Other
}
