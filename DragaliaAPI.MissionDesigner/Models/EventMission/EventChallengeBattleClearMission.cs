namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventChallengeBattleClearMission : Mission
{
    public override MissionCompleteType CompleteType =>
        MissionCompleteType.EventChallengeBattleClear;

    public required int EventId { get; init; }

    public int? QuestId { get; init; }

    public bool? FullClear { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            false,
            this.EventId,
            this.QuestId,
            this.FullClear == true ? 1 : null
        );
}
