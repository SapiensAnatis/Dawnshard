namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventPointCollectionMission : Mission
{
    public override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    public required int EventId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, false, this.EventId);
}
