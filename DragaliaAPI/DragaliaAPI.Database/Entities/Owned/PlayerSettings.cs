namespace DragaliaAPI.Database.Entities.Owned;

/// <summary>
/// DTO for user settings.
/// </summary>
public class PlayerSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the player would like to receive daily material gifts.
    /// </summary>
    public bool DailyGifts { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the player would like to use the old static helper system.
    /// </summary>
    /// <remarks>
    /// The legacy system allows choosing from a static list of meta helpers, instead of a list drawn from your friends
    /// plus an assortment of random players,
    /// </remarks>
    public bool UseLegacyHelpers { get; init; }
}
