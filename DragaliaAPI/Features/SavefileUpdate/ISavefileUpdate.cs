namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Provides a mechanism to update a savefile.
/// </summary>
/// <remarks>
/// Used to handle cases such as where we add a one-time reward to a story or quest, and then someone has completed
/// that quest prior to the change -- the porter can then add that reward to their save on login.
/// </remarks>
public interface ISavefileUpdate
{
    public int SavefileVersion { get; }

    public Task Apply();
}
