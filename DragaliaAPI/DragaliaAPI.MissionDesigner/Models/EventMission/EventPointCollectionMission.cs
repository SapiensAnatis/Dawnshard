namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by accumulating a total amount of event points across all runs.
/// Used for daily/period missions where the target resets each day.
/// Example: "Collect 1,000 Heroism".
/// </summary>
public class EventPointCollectionMission : Mission
{
    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; } = null;

    protected override MissionCompleteType CompleteType => MissionCompleteType.EventPointCollection;

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;

    protected override bool UseTotalValue => false;
}
