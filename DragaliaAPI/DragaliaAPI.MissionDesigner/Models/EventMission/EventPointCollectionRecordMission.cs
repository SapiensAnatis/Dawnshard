namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by collecting a certain amount of event points within a single run.
/// Example: "Collect 4,000 Hype in One Go".
/// </summary>
public class EventPointCollectionRecordMission : Mission
{
    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;

    protected override bool UseTotalValue => true;
}
