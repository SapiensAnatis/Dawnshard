namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventRegularBattleClearMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.EventRegularBattleClear;

    public required int EventId { get; init; }

    protected override int? Parameter => this.EventId;
}
