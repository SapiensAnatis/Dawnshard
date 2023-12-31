namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventQuestClearWithCrestMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventQuestClearWithCrest;

    public int EventId { get; set; }

    public required AbilityCrests Crest { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int)this.Crest;
}
