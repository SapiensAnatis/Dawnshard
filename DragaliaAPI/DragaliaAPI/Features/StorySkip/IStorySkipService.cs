namespace DragaliaAPI.Features.Quest;

public interface IStorySkipService
{
    Task IncreaseFortLevels();
    Task IncreasePlayerLevel();
    Task OpenTreasure(int questTreasureId, CancellationToken cancellationToken);
    Task ProcessQuestCompletion(int questId);
    Task ReadStory(int questId, CancellationToken cancellationToken);
}
