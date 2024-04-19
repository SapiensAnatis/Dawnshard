namespace DragaliaAPI.Features.Quest;

public interface IStorySkipService
{
    Task<object> ProcessQuestCompletion(int questId);
    Task ReadStory(int questId, CancellationToken cancellationToken);
}
