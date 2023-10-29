namespace DragaliaAPI.MissionDesigner.Models;

public class EventParticipationMission : Mission
{
    public override MissionCompleteType CompleteType => MissionCompleteType.EventParticipation;

    public int EventId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, this.EventId);
}
