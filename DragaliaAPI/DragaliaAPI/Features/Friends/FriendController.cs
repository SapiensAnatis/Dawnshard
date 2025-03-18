using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Friends;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
internal sealed class FriendController(
    IHelperService helperService,
    FriendService friendService,
    FriendNotificationService friendNotificationService,
    IPlayerIdentityService playerIdentityService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost]
    [Route("get_support_chara")]
    public async Task<DragaliaResult<FriendGetSupportCharaResponse>> GetSupportChara(
        CancellationToken cancellationToken
    )
    {
        SettingSupport playerSupportChara =
            await helperService.GetPlayerHelper(cancellationToken)
            ?? throw new InvalidOperationException("Failed to find current player's helper");
        ;
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
        List<long> newFriendList = await friendNotificationService.GetNewFriendViewerIdList();

        return new FriendFriendListResponse()
        {
            FriendList = friendList,
            NewFriendViewerIdList = newFriendList.Select(x => (ulong)x),
        };
    }

    [HttpPost("auto_search")]
    public DragaliaResult<FriendAutoSearchResponse> AutoSearch() =>
        new FriendAutoSearchResponse() { Result = 1, SearchList = [] };

    [HttpPost("request_list")]
    public async Task<DragaliaResult<FriendRequestListResponse>> RequestList()
    {
        List<UserSupportList> requestList = await friendService.GetSentRequestList();

        return new FriendRequestListResponse() { Result = 1, RequestList = requestList };
    }

    [HttpPost("apply_list")]
    public async Task<DragaliaResult<FriendApplyListResponse>> ApplyList()
    {
        List<UserSupportList> requestList = await friendService.GetReceivedRequestList();
        List<long> newApplyList = await friendNotificationService.GetNewFriendRequestViewerIdList();

        return new FriendApplyListResponse()
        {
            Result = 1,
            FriendApply = requestList,
            NewApplyViewerIdList = newApplyList.Select(x => (ulong)x),
        };
    }

    [HttpPost("id_search")]
    public async Task<DragaliaResult<FriendIdSearchResponse>> IdSearch(
        FriendIdSearchRequest request,
        CancellationToken cancellationToken
    )
    {
        long searchId = (long)request.SearchId;

        if (searchId == playerIdentityService.ViewerId)
        {
            // Shows "You searched for your own ID" popup
            return this.Code(ResultCode.FriendIdsearchError);
        }

        bool alreadyFriends = await friendService.CheckIfFriendshipExists(
            searchId,
            cancellationToken
        );

        if (alreadyFriends)
        {
            return this.Code(ResultCode.FriendTargetAlready);
        }

        UserSupportList? result = await helperService.GetUserSupportList(
            searchId,
            cancellationToken
        );

        if (result is null)
        {
            // Shows "Unable to locate player" popup
            return this.Code(ResultCode.FriendTargetNone);
        }

        return new FriendIdSearchResponse() { Result = 1, SearchUser = result };
    }

    [HttpPost("request")]
    public async Task<DragaliaResult<FriendRequestResponse>> SendRequest(
        FriendRequestRequest request,
        CancellationToken cancellationToken
    )
    {
        long friendId = (long)request.FriendId;

        if (friendId == playerIdentityService.ViewerId)
        {
            return this.Code(ResultCode.FriendApplyError);
        }

        bool alreadyFriends = await friendService.CheckIfFriendshipExists(
            friendId,
            cancellationToken
        );

        if (alreadyFriends)
        {
            return this.Code(ResultCode.FriendTargetAlready);
        }

        ResultCode result = await friendService.SendRequest(friendId);

        if (result != ResultCode.Success)
        {
            return this.Code(result);
        }

        return this.Ok(new FriendRequestResponse() { Result = 1 });
    }
}
