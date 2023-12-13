namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EarnEnemiesKilledMission : Mission
{
    public required int EventId { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EarnEnemiesKilled;

    protected override int? Parameter => this.EventId;
}
