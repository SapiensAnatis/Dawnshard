namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventRegularBattleClearMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventRegularBattleClear;

    public int EventId { get; set; }

    public VariationTypes? VariationType { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int?)this.VariationType;
}
