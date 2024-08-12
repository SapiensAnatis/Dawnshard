namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

public class ReadQuestStoryMission : Mission
{
    public required int QuestStoryId { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.QuestStoryCleared;

    protected override int? Parameter => this.QuestStoryId;
}
