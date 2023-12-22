using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FriendController : DragaliaControllerBase
{
    private readonly IHelperService helperService;
    private readonly IBonusService bonusService;

    private static readonly SettingSupport StubSupportCharacter = new SettingSupport(
        Charas.ThePrince,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    );

    public FriendController(IHelperService helperService, IBonusService bonusService)
    {
        this.helperService = helperService;
        this.bonusService = bonusService;
    }

    [HttpPost]
    [Route("get_support_chara")]
    public DragaliaResult GetSupportChara()
    {
        // i think this method needs to be utilized by HelperService in the future? any friends' helpers
        // should be retrieved from this method while the gaps are filled by other ppl in the database
        return Ok(new FriendGetSupportCharaData(0, StubSupportCharacter));
    }

    [HttpPost]
    [Route("get_support_chara_detail")]
    public async Task<DragaliaResult> GetSupportCharaDetail(
        FriendGetSupportCharaDetailRequest request
    )
    {
        // this eventually needs to pull from the database from another user's account based on viewer id
        QuestGetSupportUserListData helperList = await this.helperService.GetHelpers();

        UserSupportList helperInfo =
            helperList
                .support_user_list.Where(helper => helper.viewer_id == request.support_viewer_id)
                .FirstOrDefault()
            ?? HelperService.StubData.SupportListData.support_user_list.First();

        AtgenSupportUserDetailList helperDetail =
            helperList
                .support_user_detail_list.Where(
                    helper => helper.viewer_id == request.support_viewer_id
                )
                .FirstOrDefault()
            ?? new()
            {
                is_friend = false,
                viewer_id = request.support_viewer_id,
                gettable_mana_point = 50,
            };

        // TODO: when helpers are converted to use other account ids, get the bonuses of that account id
        FortBonusList bonusList = await bonusService.GetBonusList();

        FriendGetSupportCharaDetailData response =
            new()
            {
                support_user_data_detail = new()
                {
                    user_support_data = helperInfo,
                    fort_bonus_list = bonusList,
                    mana_circle_piece_id_list = Enumerable.Range(
                        1,
                        helperInfo.support_chara.additional_max_level == 20 ? 70 : 50
                    ),
                    dragon_reliability_level = 30,
                    is_friend = helperDetail.is_friend,
                    apply_send_status = 0,
                }
            };

        return Ok(response);
    }

    [HttpPost("friend_index")]
    public DragaliaResult<FriendFriendIndexData> FriendIndex() =>
        new FriendFriendIndexData()
        {
            friend_count = 0,
            entity_result = new(),
            update_data_list = new()
        };

    [HttpPost("friend_list")]
    public DragaliaResult<FriendFriendListData> FriendList() =>
        new FriendFriendListData() { friend_list = [], new_friend_viewer_id_list = [] };

    [HttpPost("auto_search")]
    public DragaliaResult<FriendAutoSearchData> AutoSearch() =>
        new FriendAutoSearchData() { result = 1, search_list = [], };

    [HttpPost("request_list")]
    public DragaliaResult<FriendRequestListData> RequestList() =>
        new FriendRequestListData() { result = 1, request_list = [] };

    [HttpPost("apply_list")]
    public DragaliaResult<FriendApplyListData> ApplyList() =>
        new FriendApplyListData()
        {
            result = 1,
            new_apply_viewer_id_list = [],
            friend_apply = []
        };

    [HttpPost("set_support_chara")]
    public DragaliaResult<FriendSetSupportCharaData> SetSupportChara() =>
        new FriendSetSupportCharaData()
        {
            result = 1,
            update_data_list = new(),
            setting_support = StubSupportCharacter,
        };
}
