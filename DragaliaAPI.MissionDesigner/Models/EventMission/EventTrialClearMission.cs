namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventTrialClearMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventTrialClear;

    public int EventId { get; set; }

    public required VariationTypes VariationType { get; init; }

    protected override int? Parameter => this.EventId;

    protected override int? Parameter2 => (int)this.VariationType;
}
