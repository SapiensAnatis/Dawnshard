using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

internal interface IHelperDataService
{
    public Task<QuestGetSupportUserListResponse> GetHelperList();

    public Task UseHelper(long helperViewerId);
}
