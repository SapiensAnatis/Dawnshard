namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventBossBattleClearMission : Mission
{
    public override MissionCompleteType CompleteType => MissionCompleteType.EventBossBattleClear;

    public required int EventId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, this.EventId);
}
