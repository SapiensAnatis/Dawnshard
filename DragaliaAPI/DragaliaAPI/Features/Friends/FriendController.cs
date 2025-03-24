using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
internal sealed class FriendController(
    IHelperService helperService,
    FriendService friendService,
    IFriendNotificationService friendNotificationService,
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
    public async Task<DragaliaResult<FriendFriendIndexResponse>> FriendIndex(
        CancellationToken cancellationToken
    )
    {
        int friendCount = await friendService.GetFriendCount();
        FriendNotice? notice = await friendNotificationService.GetFriendNotice(cancellationToken);

        return new FriendFriendIndexResponse()
        {
            FriendCount = friendCount,
            UpdateDataList = new() { FriendNotice = notice },
        };
    }

    [HttpPost("friend_list")]
    public async Task<DragaliaResult<FriendFriendListResponse>> FriendList()
    {
        List<UserSupportList> friendList = await friendService.GetFriendList();
        List<ulong> newFriendList = await friendNotificationService.GetNewFriendViewerIdList();

        return new FriendFriendListResponse()
        {
            FriendList = friendList,
            NewFriendViewerIdList = newFriendList,
        };
    }

    [HttpPost("auto_search")]
    public async Task<DragaliaResult<FriendAutoSearchResponse>> AutoSearch(
        CancellationToken cancellationToken
    )
    {
        List<UserSupportList> list = await friendService.GetRecommendedFriendsList(
            cancellationToken
        );

        /*
         * Strange behaviour: sometimes the game will ignore what we send here and refuse
         * to show users. Appears to be related to sending a friend request to a list entry:
         * even if you subsequently cancel it, that entry will never show in the list ever again.
         */

        return new FriendAutoSearchResponse() { Result = 1, SearchList = list };
    }

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
        List<ulong> newApplyList =
            await friendNotificationService.GetNewFriendRequestViewerIdList();

        return new FriendApplyListResponse()
        {
            Result = 1,
            FriendApply = requestList,
            NewApplyViewerIdList = newApplyList,
        };
    }

    [HttpPost("id_search")]
    public async Task<DragaliaResult<FriendIdSearchResponse>> IdSearch(
        FriendIdSearchRequest request,
        CancellationToken cancellationToken
    )
    {
        long searchId = (long)request.SearchId;

        /*
         * The inner result fields appear unused, it seems that we have to return actual
         * result_code values (of the data_headers variety) to get the game to do anything.
         */

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

        await friendService.SendRequest(friendId, cancellationToken);

        UpdateDataList updateDataList;

        try
        {
            updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new DragaliaException(
                ResultCode.FriendApplyExists,
                "Cannot send friend request - one already exists",
                ex
            );
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            throw new DragaliaException(
                ResultCode.FriendApplyError,
                "Cannot send friend request - foreign key violation",
                ex
            );
        }

        return this.Ok(new FriendRequestResponse() { Result = 1, UpdateDataList = updateDataList });
    }

    [HttpPost("request_cancel")]
    public async Task<DragaliaResult<FriendRequestCancelResponse>> RequestCancel(
        FriendRequestCancelRequest request,
        CancellationToken cancellationToken
    )
    {
        await friendService.CancelRequest((long)request.FriendId, cancellationToken);

        return new FriendRequestCancelResponse() { Result = 1 };
    }

    [HttpPost("reply")]
    public async Task<DragaliaResult<FriendReplyResponse>> Reply(
        FriendReplyRequest request,
        CancellationToken cancellationToken
    )
    {
        await friendService.ReplyToRequest(
            (long)request.FriendId,
            request.Reply,
            cancellationToken
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new FriendReplyResponse() { Result = 1, UpdateDataList = updateDataList };
    }

    [HttpPost("delete")]
    public async Task<DragaliaResult<FriendDeleteResponse>> Delete(
        FriendDeleteRequest request,
        CancellationToken cancellationToken
    )
    {
        await friendService.DeleteFriend((long)request.FriendId, cancellationToken);

        _ = await updateDataService.SaveChangesAsync(cancellationToken);

        return new FriendDeleteResponse() { Result = 1 };
    }
}
