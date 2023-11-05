namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventTrialClearMission : Mission
{
    public override MissionCompleteType CompleteType => MissionCompleteType.EventTrialClear;

    public required int EventId { get; init; }

    public required int QuestId { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            false,
            this.EventId,
            this.QuestId
        );
}
