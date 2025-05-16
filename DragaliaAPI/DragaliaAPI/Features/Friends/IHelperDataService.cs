using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

internal interface IHelperDataService
{
    public Task<QuestGetSupportUserListResponse> GetHelperList(CancellationToken cancellationToken);

    public Task<UserSupportList?> GetHelper(
        long helperViewerId,
        CancellationToken cancellationToken
    );

    public Task<AtgenSupportUserDataDetail?> GetHelperDataDetail(
        long helperViewerId,
        CancellationToken cancellationToken = default
    );

    public Task UseHelper(long helperViewerId, CancellationToken cancellationToken);
}
