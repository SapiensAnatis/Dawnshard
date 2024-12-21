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
}
