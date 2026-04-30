namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by opening the event for the first time.
/// Example: "Participate in the Event".
/// </summary>
public class EventParticipationMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventParticipation;

    public int EventId { get; set; }

    protected override int? Parameter => this.EventId;
}
