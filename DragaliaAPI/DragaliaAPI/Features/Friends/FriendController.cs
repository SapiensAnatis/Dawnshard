using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Friends;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
internal sealed class FriendController(
    IHelperService helperService,
    FriendService friendService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost]
    [Route("get_support_chara")]
    public async Task<DragaliaResult<FriendGetSupportCharaResponse>> GetSupportChara(
        CancellationToken cancellationToken
    )
    {
        SettingSupport playerSupportChara = await helperService.GetPlayerHelper(cancellationToken);
        return new FriendGetSupportCharaResponse(0, playerSupportChara);
    }

    [HttpPost]
    [Route("get_support_chara_detail")]
    public async Task<DragaliaResult<FriendGetSupportCharaDetailResponse>> GetSupportCharaDetail(
        FriendGetSupportCharaDetailRequest request,
        CancellationToken cancellationToken
    )
    {
        QuestGetSupportUserListResponse helperList = await helperService.GetHelpers();

        UserSupportList? staticHelperInfo = helperList.SupportUserList.FirstOrDefault(helper =>
            helper.ViewerId == request.SupportViewerId
        );

        if (staticHelperInfo is not null)
        {
            return new FriendGetSupportCharaDetailResponse()
            {
                SupportUserDataDetail = await helperService.BuildStaticSupportUserDataDetail(
                    staticHelperInfo
                ),
            };
        }

        AtgenSupportUserDataDetail otherPlayerSupportDetail =
            await helperService.GetSupportUserDataDetail(
                (long)request.SupportViewerId,
                cancellationToken
            );

        return new FriendGetSupportCharaDetailResponse()
        {
            SupportUserDataDetail = otherPlayerSupportDetail,
        };
    }

    [HttpPost("set_support_chara")]
    public async Task<DragaliaResult<FriendSetSupportCharaResponse>> SetSupportChara(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    )
    {
        SettingSupport settingSupport = await helperService.SetPlayerHelper(
            request,
            cancellationToken
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new FriendSetSupportCharaResponse()
        {
            Result = 1,
            SettingSupport = settingSupport,
            UpdateDataList = updateDataList,
        };
    }

    [HttpPost("friend_index")]
    public async Task<DragaliaResult<FriendFriendIndexResponse>> FriendIndex()
    {
        int friendCount = await friendService.GetFriendCount();

        return new FriendFriendIndexResponse()
        {
            FriendCount = friendCount,
            EntityResult = new(),
            UpdateDataList = new(),
        };
    }

    [HttpPost("friend_list")]
    public async Task<DragaliaResult<FriendFriendListResponse>> FriendList()
    {
        List<UserSupportList> friendList = await friendService.GetFriendList();

        // todo: new indicator (will be annoying as fuck)

        return new FriendFriendListResponse()
        {
            FriendList = friendList,
            NewFriendViewerIdList = [],
        };
    }

    [HttpPost("auto_search")]
    public DragaliaResult<FriendAutoSearchResponse> AutoSearch() =>
        new FriendAutoSearchResponse() { Result = 1, SearchList = [] };

    [HttpPost("request_list")]
    public DragaliaResult<FriendRequestListResponse> RequestList() =>
        new FriendRequestListResponse() { Result = 1, RequestList = [] };

    [HttpPost("apply_list")]
    public DragaliaResult<FriendApplyListResponse> ApplyList() =>
        new FriendApplyListResponse()
        {
            Result = 1,
            NewApplyViewerIdList = [],
            FriendApply = [],
        };
}
