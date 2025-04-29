using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public class RealHelperDataService : IHelperDataService
{
    public Task<QuestGetSupportUserListResponse> GetHelperList(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<UserSupportList?> GetHelper(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task UseHelper(long helperViewerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
