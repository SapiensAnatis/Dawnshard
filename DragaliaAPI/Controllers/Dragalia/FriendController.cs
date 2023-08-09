using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FriendController : DragaliaControllerBase
{
    private readonly IHelperService helperService;
    private readonly IBonusService bonusService;

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
        return Ok(
            new FriendGetSupportCharaData(
                0,
                new SettingSupport(Charas.ThePrince, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            )
        );
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
            helperList.support_user_list
                .Where(helper => helper.viewer_id == request.support_viewer_id)
                .FirstOrDefault()
            ?? HelperService.StubData.SupportListData.support_user_list.First();

        AtgenSupportUserDetailList helperDetail =
            helperList.support_user_detail_list
                .Where(helper => helper.viewer_id == request.support_viewer_id)
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
}
