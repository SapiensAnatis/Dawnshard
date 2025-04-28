using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public class StaticHelperDataService : IHelperDataService
{
    public Task<QuestGetSupportUserListResponse> GetHelperList()
    {
        throw new NotImplementedException();
    }

    public Task UseHelper(long helperViewerId)
    {
        // No-op
        return Task.CompletedTask;
    }
}
