using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FriendController : DragaliaControllerBase
{
    private readonly IHelperService helperService;

    public FriendController(IHelperService helperService)
    {
        this.helperService = helperService;
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
            ?? new() { support_chara = new() { chara_id = Charas.ThePrince } };

        AtgenSupportUserDetailList helperDetail =
            helperList.support_user_detail_list
                .Where(helper => helper.viewer_id == request.support_viewer_id)
                .FirstOrDefault() ?? new() { is_friend = false };

        FriendGetSupportCharaDetailData response =
            new()
            {
                support_user_data_detail = new()
                {
                    user_support_data = helperInfo,
                    fort_bonus_list = StubData.EmptyBonusList,
                    mana_circle_piece_id_list = Enumerable.Range(
                        1,
                        helperInfo.support_chara.additional_max_level == 20 ? 70 : 50
                    ),
                    dragon_reliability_level = 30,
                    is_friend = helperDetail.is_friend,
                    apply_send_status = 0
                }
            };

        return Ok(response);
    }

    private static class StubData
    {
        private static readonly IEnumerable<AtgenParamBonus> WeaponBonus = Enumerable
            .Range(1, 9)
            .Select(
                x =>
                    new AtgenParamBonus()
                    {
                        weapon_type = x,
                        hp = 200,
                        attack = 200
                    }
            );

        private static readonly IEnumerable<AtgenElementBonus> EmptyElementBonus = Enumerable
            .Range(1, 5)
            .Select(
                x =>
                    new AtgenElementBonus()
                    {
                        elemental_type = x,
                        hp = 200,
                        attack = 200
                    }
            )
            .Append(
                new AtgenElementBonus()
                {
                    elemental_type = 99,
                    hp = 20,
                    attack = 20
                }
            );

        private static readonly IEnumerable<AtgenDragonBonus> EmptyDragonBonus = Enumerable
            .Range(1, 5)
            .Select(
                x =>
                    new AtgenDragonBonus()
                    {
                        elemental_type = x,
                        dragon_bonus = 200,
                        hp = 200,
                        attack = 200
                    }
            )
            .Append(
                new AtgenDragonBonus()
                {
                    elemental_type = 99,
                    hp = 200,
                    attack = 200
                }
            );

        public static readonly FortBonusList EmptyBonusList =
            new()
            {
                param_bonus = WeaponBonus,
                param_bonus_by_weapon = WeaponBonus,
                element_bonus = EmptyElementBonus,
                chara_bonus_by_album = EmptyElementBonus,
                all_bonus = new() { hp = 200, attack = 200 },
                dragon_bonus = EmptyDragonBonus,
                dragon_bonus_by_album = EmptyElementBonus,
                dragon_time_bonus = new() { dragon_time_bonus = 20 }
            };
    }
}
