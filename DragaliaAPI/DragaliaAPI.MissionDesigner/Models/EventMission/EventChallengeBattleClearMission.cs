namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing the event's challenge battle or raid.
/// Supports targeting a specific difficulty via <see cref="VariationType"/>, requiring a full clear
/// via <see cref="FullClear"/>, and targeting a specific quest via <see cref="QuestId"/>.
/// Example: "Completely Clear a Challenge Battle on Master".
/// </summary>
public class EventChallengeBattleClearMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventChallengeBattleClear;

    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; }

    public bool? FullClear { get; init; }

    /// <remarks>
    /// Only use this when expert / master challenge battles cannot be distinguished
    /// with <see cref="VariationType"/>.
    /// </remarks>
    public int? QuestId { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;

    protected override int? Parameter3 => this.FullClear == true ? 1 : null;

    protected override int? Parameter4 => this.QuestId;
}
