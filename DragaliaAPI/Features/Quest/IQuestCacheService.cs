namespace DragaliaAPI.Features.Quest;

public interface IQuestCacheService
{
    Task SetQuestGroupQuestIdAsync(int questGroupId, int questId);
    Task<int?> GetQuestGroupQuestIdAsync(int questGroupId);
    Task RemoveQuestGroupQuestIdAsync(int questGroupId);
}
