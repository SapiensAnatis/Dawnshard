using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Friends;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
internal sealed class FriendController(
    HelperService helperService,
    IBonusService bonusService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost]
    [Route("get_support_chara")]
    public async Task<DragaliaResult<FriendGetSupportCharaResponse>> GetSupportChara()
    {
        SettingSupport playerSupportChara = await helperService.GetPlayerHelper();
        return new FriendGetSupportCharaResponse(0, playerSupportChara);
    }

    [HttpPost]
    [Route("get_support_chara_detail")]
    public async Task<DragaliaResult> GetSupportCharaDetail(
        FriendGetSupportCharaDetailRequest request
    )
    {
        // this eventually needs to pull from the database from another user's account based on viewer id
        QuestGetSupportUserListResponse helperList = await helperService.GetHelpers();

        UserSupportList helperInfo =
            helperList
                .SupportUserList.Where(helper => helper.ViewerId == request.SupportViewerId)
                .FirstOrDefault() ?? HelperService.StubData.SupportListData.SupportUserList.First();

        AtgenSupportUserDetailList helperDetail =
            helperList
                .SupportUserDetailList.Where(helper => helper.ViewerId == request.SupportViewerId)
                .FirstOrDefault()
            ?? new()
            {
                IsFriend = false,
                ViewerId = request.SupportViewerId,
                GettableManaPoint = 50,
            };

        // TODO: when helpers are converted to use other account ids, get the bonuses of that account id
        FortBonusList bonusList = await bonusService.GetBonusList();

        FriendGetSupportCharaDetailResponse response = new()
        {
            SupportUserDataDetail = new()
            {
                UserSupportData = helperInfo,
                FortBonusList = bonusList,
                ManaCirclePieceIdList = Enumerable.Range(
                    1,
                    helperInfo.SupportChara.AdditionalMaxLevel == 20 ? 70 : 50
                ),
                DragonReliabilityLevel = 30,
                IsFriend = helperDetail.IsFriend,
                ApplySendStatus = 0,
            },
        };

        return this.Ok(response);
    }

    [HttpPost("friend_index")]
    public DragaliaResult<FriendFriendIndexResponse> FriendIndex() =>
        new FriendFriendIndexResponse()
        {
            FriendCount = 0,
            EntityResult = new(),
            UpdateDataList = new(),
        };

    [HttpPost("friend_list")]
    public DragaliaResult<FriendFriendListResponse> FriendList() =>
        new FriendFriendListResponse() { FriendList = [], NewFriendViewerIdList = [] };

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

    [HttpPost("set_support_chara")]
    public async Task<DragaliaResult<FriendSetSupportCharaResponse>> SetSupportChara(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    )
    {
        SettingSupport settingSupport = await helperService.SetPlayerHelper(request);
        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new FriendSetSupportCharaResponse()
        {
            Result = 1,
            SettingSupport = settingSupport,
            UpdateDataList = updateDataList,
        };
    }
}
