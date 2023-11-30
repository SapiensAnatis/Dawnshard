namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventPointCollectionMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    public required int EventId { get; init; }

    protected override int? Parameter => this.EventId;
}
