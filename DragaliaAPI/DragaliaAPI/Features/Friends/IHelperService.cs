using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public interface IHelperService
{
    Task<QuestGetSupportUserListResponse> GetHelperList(
        CancellationToken cancellationToken = default
    );

    Task<UserSupportList?> GetHelper(long viewerId, CancellationToken cancellationToken = default);

    Task<AtgenSupportUserDataDetail?> GetHelperDetail(
        long viewerId,
        CancellationToken cancellationToken = default
    );

    Task<UserSupportList> GetLeadUnit(int partyNo);

    Task<SettingSupport> GetOwnHelper(CancellationToken cancellationToken = default);

    Task<SettingSupport> SetOwnHelper(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    );
}
