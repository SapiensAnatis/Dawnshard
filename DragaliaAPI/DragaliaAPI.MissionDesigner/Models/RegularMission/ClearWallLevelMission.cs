namespace DragaliaAPI.MissionDesigner.Models;

public class ClearWallLevelMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.WallLevelCleared;

    public required int Level { get; init; }

    public QuestWallTypes? WallType { get; init; }

    protected override int? Parameter => this.Level;

    protected override int? Parameter2 => (int?)this.WallType;
}
