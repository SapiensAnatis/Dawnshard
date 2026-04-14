namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by finishing all other missions within a given progression group.
/// Used for "Clear All Daily Endeavors" style wrap-up missions.
/// Example: "Clear All Standard Daily Endeavors".
/// </summary>
public class ClearProgressionGroupMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.ProgressionGroupCleared;

    public required int ProgressionGroupToClear { get; init; }

    protected override int? Parameter => this.ProgressionGroupToClear;
}
