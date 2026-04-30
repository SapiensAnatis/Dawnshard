namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by clearing a specific level of the Mercurial Gauntlet (The Elemental Walls).
/// A <see cref="WallType"/> can be specified to target a particular elemental wall.
/// Example: "Clear The Mercurial Gauntlet (Flame): Lv. 10".
/// </summary>
public class ClearWallLevelMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.WallLevelCleared;

    public required int Level { get; init; }

    public QuestWallTypes? WallType { get; init; }

    protected override int? Parameter => this.Level;

    protected override int? Parameter2 => (int?)this.WallType;
}
