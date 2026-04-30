namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by reading (clearing) a specific quest story.
/// When used to track a multi-chapter story, set <see cref="QuestStoryId"/> to the final chapter's
/// ID, as the completion value is 1.
/// Example: "Clear All of the Event's Story Quests".
/// </summary>
public class ReadQuestStoryMission : Mission
{
    public required int QuestStoryId { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.QuestStoryCleared;

    protected override int? Parameter => this.QuestStoryId;
}
