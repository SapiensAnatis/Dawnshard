using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("dragon")]
public class DragonController : DragaliaControllerBase
{
    private readonly IDragonService dragonService;

    public DragonController(IDragonService dragonService)
    {
        this.dragonService = dragonService;
    }

    [Route("buildup")]
    [HttpPost]
    public async Task<DragaliaResult> Buildup([FromBody] DragonBuildupRequest request)
    {
        return Ok(await dragonService.DoBuildup(request));
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> DragonResetPlusCount(
        [FromBody] DragonResetPlusCountRequest request
    )
    {
        return Ok(await dragonService.DoDragonResetPlusCount(request));
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> DragonLimitBreak([FromBody] DragonLimitBreakRequest request)
    {
        return Ok(await dragonService.DoDragonLimitBreak(request));
    }

    [Route("get_contact_data")]
    [HttpPost]
    public async Task<DragaliaResult> DragonGetContactData(
        [FromBody] DragonGetContactDataRequest request
    )
    {
        return Ok(await dragonService.DoDragonGetContactData(request));
    }

    [Route("buy_gift_to_send_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSendMultiple(
        [FromBody] DragonBuyGiftToSendMultipleRequest request
    )
    {
        return Ok(await dragonService.DoDragonBuyGiftToSendMultiple(request));
    }

    [Route("buy_gift_to_send")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSend(
        [FromBody] DragonBuyGiftToSendRequest request
    )
    {
        DragonBuyGiftToSendMultipleData resultData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = request.dragon_id,
                    dragon_gift_id_list = new List<DragonGifts>() { request.dragon_gift_id }
                }
            );
        return Ok(
            new DragonBuyGiftToSendData()
            {
                dragon_contact_free_gift_count = resultData.dragon_contact_free_gift_count,
                entity_result = resultData.entity_result,
                is_favorite = resultData.dragon_gift_reward_list.First().is_favorite,
                return_gift_list = resultData.dragon_gift_reward_list.First().return_gift_list,
                reward_reliability_list = resultData
                    .dragon_gift_reward_list.First()
                    .reward_reliability_list,
                shop_gift_list = resultData.shop_gift_list,
                update_data_list = resultData.update_data_list
            }
        );
    }

    [Route("send_gift_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSentGiftMultiple(
        [FromBody] DragonSendGiftMultipleRequest request
    )
    {
        return Ok(await dragonService.DoDragonSendGiftMultiple(request));
    }

    [Route("send_gift")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSendGift([FromBody] DragonSendGiftRequest request)
    {
        DragonSendGiftMultipleData resultData = await dragonService.DoDragonSendGiftMultiple(
            new DragonSendGiftMultipleRequest()
            {
                dragon_id = request.dragon_id,
                dragon_gift_id = request.dragon_gift_id,
                quantity = 1
            }
        );
        return Ok(
            new DragonSendGiftData()
            {
                is_favorite = resultData.is_favorite,
                return_gift_list = resultData.return_gift_list,
                reward_reliability_list = resultData.reward_reliability_list,
                update_data_list = resultData.update_data_list
            }
        );
    }

    [Route("set_lock")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSetLock([FromBody] DragonSetLockRequest request)
    {
        return Ok(await dragonService.DoDragonSetLock(request));
    }

    [Route("sell")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSell([FromBody] DragonSellRequest request)
    {
        return Ok(await dragonService.DoDragonSell(request));
    }
}
