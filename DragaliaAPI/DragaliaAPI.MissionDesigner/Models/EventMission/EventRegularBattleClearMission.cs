namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing the event's regular (boss) battle.
/// A <see cref="VariationType"/> can be specified to target a specific difficulty.
/// Example: "Clear an Invasion on Expert".
/// </summary>
public class EventRegularBattleClearMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventRegularBattleClear;

    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;
}
