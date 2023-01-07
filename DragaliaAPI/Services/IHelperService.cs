using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IHelperService
{
    Task<QuestGetSupportUserListData> GetHelpers();
    AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    );
    UserSupportList? GetHelperInfo(QuestGetSupportUserListData helperList, ulong viewerId);
    AtgenSupportUserDetailList? GetHelperDetail(
        QuestGetSupportUserListData helperList,
        ulong viewerId
    );
}
