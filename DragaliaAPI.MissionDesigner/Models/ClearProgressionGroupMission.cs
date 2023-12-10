namespace DragaliaAPI.MissionDesigner.Models;

public class ClearProgressionGroupMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.ProgressionGroupCleared;

    public required int ProgressionGroupToClear { get; init; }

    protected override int? Parameter => this.ProgressionGroupToClear;
}
