namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventChallengeBattleFullClearMission : Mission
{
    public override MissionCompleteType CompleteType =>
        MissionCompleteType.EventChallengeBattleFullClear;

    public required int EventId { get; init; }

    public required int QuestId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            true,
            this.EventId,
            this.QuestId
        );
}
