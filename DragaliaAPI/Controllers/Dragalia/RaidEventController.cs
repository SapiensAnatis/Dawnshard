using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("raid_event")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class RaidEventController : DragaliaControllerBase
{
    public RaidEventController() { }

    private static class StubData
    {
        public static RaidEventGetEventDataData RaidEventData =
            new()
            {
                raid_event_user_data = new RaidEventUserList(),
                raid_event_reward_list = new List<RaidEventRewardList>(),
                chara_friendship_list = new List<CharaFriendshipList>(),
                event_trade_list = new List<EventTradeList>(),
                event_passive_list = new List<EventPassiveList>() { new() { } },
                is_receive_event_damage_reward = false,
                event_damage_data = new()
                {
                    user_damage_value = 0,
                    user_target_time = 0,
                    total_aggregate_time = 0,
                    total_damage_value = 0,
                    total_target_time = 0
                },
                event_ability_chara_list = new List<EventAbilityCharaList>(),
            };
    }

    [HttpPost("get_event_data")]
    public DragaliaResult GetData(RaidEventGetEventDataRequest request)
    {
        return Ok(StubData.RaidEventData);
    }
}
