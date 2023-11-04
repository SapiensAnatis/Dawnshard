namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventChallengeBattleClearMission : Mission
{
    public override MissionCompleteType CompleteType =>
        MissionCompleteType.EventChallengeBattleClear;

    public required int EventId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, this.EventId);
}
