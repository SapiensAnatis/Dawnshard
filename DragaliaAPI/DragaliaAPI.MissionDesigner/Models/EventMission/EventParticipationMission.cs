namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventParticipationMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventParticipation;

    public int EventId { get; set; }

    protected override int? Parameter => this.EventId;
}
