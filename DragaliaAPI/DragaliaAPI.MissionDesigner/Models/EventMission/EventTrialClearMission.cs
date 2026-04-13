namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing the event's trial quest at a specified difficulty.
/// Example: "Clear a 'One Starry Dragonyule' Trial on Expert".
/// </summary>
public class EventTrialClearMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventTrialClear;

    public int EventId { get; set; }

    public required VariationTypes VariationType { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int)this.VariationType;
}
