namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EarnEnemiesKilledMission : Mission
{
    public int EventId { get; set; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EarnEnemiesKilled;

    protected override int? Parameter => this.EventId;
}
