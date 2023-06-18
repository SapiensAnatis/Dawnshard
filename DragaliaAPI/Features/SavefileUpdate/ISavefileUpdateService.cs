namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Service to apply <see cref="ISavefileUpdate"/> instances.
/// </summary>
public interface ISavefileUpdateService
{
    /// <summary>
    /// Update a savefile with all pending updates.
    /// </summary>
    /// <returns>The task.</returns>
    Task UpdateSavefile();
}
