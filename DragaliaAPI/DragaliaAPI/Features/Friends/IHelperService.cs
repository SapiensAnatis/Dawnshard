using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public interface IHelperService
{
    Task<QuestGetSupportUserListResponse> GetHelpers();

    AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    );

    Task<UserSupportList?> GetHelper(ulong viewerId);

    Task<UserSupportList> GetLeadUnit(int partyNo);

    Task<AtgenSupportUserDataDetail> BuildStaticSupportUserDataDetail(
        UserSupportList staticHelperInfo
    );

    Task<SettingSupport> GetPlayerHelper(CancellationToken cancellationToken);

    Task<SettingSupport> SetPlayerHelper(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    );

    Task<AtgenSupportUserDataDetail> GetSupportUserDataDetail(
        long viewerId,
        CancellationToken cancellationToken
    );
}
