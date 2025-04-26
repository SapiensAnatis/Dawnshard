namespace DragaliaAPI.Database.Entities.Owned;

/// <summary>
/// DTO for user settings.
/// </summary>
public class PlayerSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the player would like to receive daily material gifts.
    /// </summary>
    public bool DailyGifts { get; init; }
}
