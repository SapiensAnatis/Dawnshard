namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventPointCollectionRecordMission : Mission
{
    public required int EventId { get; init; }

    public int? QuestId { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => this.QuestId;

    protected override bool UseTotalValue => true;
}
