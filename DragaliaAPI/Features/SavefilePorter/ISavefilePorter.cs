namespace DragaliaAPI.Features.SavefilePorter;

/// <summary>
/// Provides a mechanism to update a savefile if the last login time is before <see cref="ModificationDate"/>.
/// </summary>
/// <remarks>
/// Used to handle cases such as where we add a one-time reward to a story or quest, and then someone has completed
/// that quest prior to the change -- the porter can then add that reward to their save on login.
/// </remarks>
public interface ISavefilePorter
{
    public DateTime ModificationDate { get; }

    public Task Port();
}
