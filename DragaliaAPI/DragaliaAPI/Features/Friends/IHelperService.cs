using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public interface IHelperService
{
    Task<QuestGetSupportUserListResponse> GetHelpers(CancellationToken cancellationToken = default);

    AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    );

    Task<UserSupportList?> GetHelper(long viewerId, CancellationToken cancellationToken = default);

    Task<UserSupportList> GetLeadUnit(int partyNo);

    Task<AtgenSupportUserDataDetail> BuildStaticSupportUserDataDetail(
        UserSupportList staticHelperInfo
    );

    Task<SettingSupport> GetPlayerHelper(CancellationToken cancellationToken = default);

    Task<SettingSupport> SetPlayerHelper(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    );

    Task<AtgenSupportUserDataDetail> GetSupportUserDataDetail(
        long viewerId,
        CancellationToken cancellationToken
    );

    Task<UserSupportList?> GetUserSupportList(long viewerId, CancellationToken cancellationToken);
}
