using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.AutoRepeat;

public interface IAutoRepeatService
{
    /// <summary>
    /// Save a <see cref="RepeatSetting"/> to the cache.
    /// </summary>
    /// <param name="repeatSetting">Instance of <see cref="RepeatSetting"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetRepeatSetting(RepeatSetting repeatSetting);

    /// <summary>
    /// Gets an instance of <see cref="RepeatInfo"/> associated with the user by viewer ID lookup.
    /// </summary>
    /// <returns>An instance of <see cref="RepeatInfo"/>, or <see langword="null"/> if none was found.</returns>
    Task<RepeatInfo?> GetRepeatInfo();

    /// <summary>
    /// Clear the current player's repeat info, and return it if it was found.
    /// </summary>
    /// <returns>An instance of <see cref="RepeatInfo"/>, or <see langword="null"/> if none was found.</returns>
    Task<RepeatInfo?> ClearRepeatInfo();

    /// <summary>
    /// Record a clear during an auto-repeat, and either create or update a <see cref="RepeatInfo"/>.
    /// </summary>
    /// <param name="repeatKey">The repeat key.</param>
    /// <param name="ingameResultData">The <see cref="IngameResultData"/> for this clear.</param>
    /// <param name="updateDataList">The <see cref="UpdateDataList"/> for this clear.</param>
    /// <returns></returns>
    Task<RepeatData?> RecordRepeat(
        string? repeatKey,
        IngameResultData ingameResultData,
        UpdateDataList updateDataList
    );
}
