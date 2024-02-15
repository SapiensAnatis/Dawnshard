namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventPointCollectionRecordMission : Mission
{
    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;

    protected override bool UseTotalValue => true;
}
