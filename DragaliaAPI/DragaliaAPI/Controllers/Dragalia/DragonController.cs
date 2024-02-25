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
                    DragonId = request.DragonId,
                    DragonGiftIdList = new List<DragonGifts>() { request.DragonGiftId }
                }
            );
        return Ok(
            new DragonBuyGiftToSendData()
            {
                DragonContactFreeGiftCount = resultData.DragonContactFreeGiftCount,
                EntityResult = resultData.EntityResult,
                IsFavorite = resultData.DragonGiftRewardList.First().IsFavorite,
                ReturnGiftList = resultData.DragonGiftRewardList.First().ReturnGiftList,
                RewardReliabilityList = resultData
                    .DragonGiftRewardList.First()
                    .RewardReliabilityList,
                ShopGiftList = resultData.ShopGiftList,
                UpdateDataList = resultData.UpdateDataList
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
                DragonId = request.DragonId,
                DragonGiftId = request.DragonGiftId,
                Quantity = 1
            }
        );
        return Ok(
            new DragonSendGiftData()
            {
                IsFavorite = resultData.IsFavorite,
                ReturnGiftList = resultData.ReturnGiftList,
                RewardReliabilityList = resultData.RewardReliabilityList,
                UpdateDataList = resultData.UpdateDataList
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
