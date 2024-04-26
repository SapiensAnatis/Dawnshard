namespace DragaliaAPI.Features.Quest;

public interface IStorySkipService
{
    Task IncreaseBuildingLevel();
    Task IncreasePlayerLevel();
    Task OpenTreasure(int questTreasureId, CancellationToken cancellationToken);
    Task<object> ProcessQuestCompletion(int questId);
    Task ReadStory(int questId, CancellationToken cancellationToken);
}
