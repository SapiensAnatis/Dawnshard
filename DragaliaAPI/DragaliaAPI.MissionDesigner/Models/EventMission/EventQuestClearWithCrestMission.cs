namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing an event quest with a specific wyrmprint (ability crest) equipped.
/// Example: "Clear an 'A Crescendo of Courage' Quest with Surfing Siblings Equipped".
/// </summary>
public class EventQuestClearWithCrestMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventQuestClearWithCrest;

    public int EventId { get; set; }

    public required AbilityCrestId Crest { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int)this.Crest;
}
