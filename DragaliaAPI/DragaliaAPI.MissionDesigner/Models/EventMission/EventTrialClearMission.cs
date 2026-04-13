namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing the event's trial quest at a specified difficulty.
/// Supports targeting a specific quest via <see cref="QuestId"/> to distinguish between multiple
/// trial types at the same difficulty.
/// Example: "Clear a 'One Starry Dragonyule' Trial on Expert".
/// </summary>
public class EventTrialClearMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventTrialClear;

    public int EventId { get; set; }

    public required VariationTypes VariationType { get; init; }

    /// <remarks>
    /// Only use this when multiple trial types cannot be distinguished with
    /// <see cref="VariationType"/> alone.
    /// </remarks>
    public int? QuestId { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int)this.VariationType;

    protected override int? Parameter3 => this.QuestId;
}
