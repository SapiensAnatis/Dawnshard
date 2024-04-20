namespace DragaliaAPI.Features.Quest;

public interface IStorySkipService
{
    Task IncreasePlayerLevel();
    Task<object> ProcessQuestCompletion(int questId);
    Task ReadStory(int questId, CancellationToken cancellationToken);
}
