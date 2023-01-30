using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
public class MypageController : DragaliaControllerBase
{
    [HttpPost("info")]
    public ActionResult<object> Info()
    {
        MypageInfoData data =
            new()
            {
                user_summon_list = new List<UserSummonList>(),
                is_shop_notification = false,
                is_view_start_dash = false,
                is_receive_event_damage_reward = false,
                is_view_dream_select = false,
                quest_event_schedule_list = new List<QuestEventScheduleList>(),
                quest_schedule_detail_list = new List<QuestScheduleDetailList>(),
                update_data_list = new()
                {
                    mission_notice = new()
                    {
                        drill_mission_notice = new()
                        {
                            is_update = true,
                            receivable_reward_count = 0,
                            new_complete_mission_id_list = new List<int>(),
                            all_mission_count = 54,
                            completed_mission_count = 0,
                            current_mission_id = 100100
                        }
                    },
                    shop_notice = new() { is_shop_notification = false, },
                    guild_notice = new(),
                }
            };

        return this.Ok(data);
    }
}
