using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Shop;

[Route("shop")]
public class ShopController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IItemSummonService itemSummonService;
    private readonly IShopService shopService;
    private readonly IRewardService rewardService;

    public ShopController(
        IUpdateDataService updateDataService,
        IItemSummonService itemSummonService,
        IShopService shopService,
        IRewardService rewardService
    )
    {
        this.updateDataService = updateDataService;
        this.itemSummonService = itemSummonService;
        this.shopService = shopService;
        this.rewardService = rewardService;
    }

    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        ILookup<PurchaseShopType, ShopPurchaseList> purchases =
            await this.shopService.GetPurchases();

        ShopGetListData response =
            new()
            {
                // i don't know what like half of these are for lmao
                is_quest_bonus = 0,
                is_stone_bonus = 0,
                is_stamina_bonus = 0,
                material_shop_purchase = purchases[PurchaseShopType.Material],
                normal_shop_purchase = purchases[PurchaseShopType.Normal],
                special_shop_purchase = purchases[PurchaseShopType.Special],
                stone_bonus = new List<AtgenStoneBonus>(),
                stamina_bonus = new List<AtgenStaminaBonus>(),
                quest_bonus = new List<AtgenQuestBonus>(),
                product_lock_list = new List<AtgenProductLockList>(),
                product_list = new List<ProductList>(),
                infancy_paid_diamond_limit = 4800
            };

        response.user_item_summon = await this.itemSummonService.GetItemSummon();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();

        return Ok(response);
    }

    [HttpPost("item_summon_odd")]
    public DragaliaResult GetOdds()
    {
        return Ok(new ShopItemSummonOddData(itemSummonService.GetOdds()));
    }

    [HttpPost("item_summon_exec")]
    public async Task<DragaliaResult> ExecItemSummon(ShopItemSummonExecRequest request)
    {
        ShopItemSummonExecData resp = new();

        resp.item_summon_reward_list = await this.itemSummonService.DoSummon(request);
        resp.user_item_summon = await this.itemSummonService.GetItemSummon();
        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.entity_result = this.rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("material_shop_purchase")]
    public async Task<DragaliaResult> MaterialShopPurchase(ShopMaterialShopPurchaseRequest request)
    {
        ShopMaterialShopPurchaseData resp = new();

        resp.material_shop_purchase = await this.shopService.DoPurchase(
            request.shop_type.ToShopType(),
            request.payment_type,
            request.goods_id,
            request.quantity
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("normal_shop_purchase")]
    public async Task<DragaliaResult> NormalShopPurchase(ShopNormalShopPurchaseRequest request)
    {
        ShopNormalShopPurchaseData resp = new();

        resp.normal_shop_purchase = await this.shopService.DoPurchase(
            ShopType.Normal,
            request.payment_type,
            request.goods_id,
            request.quantity
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("special_shop_purchase")]
    public async Task<DragaliaResult> SpecialShopPurchase(ShopSpecialShopPurchaseRequest request)
    {
        ShopSpecialShopPurchaseData resp = new();

        resp.special_shop_purchase = await this.shopService.DoPurchase(
            ShopType.Special,
            request.payment_type,
            request.goods_id,
            request.quantity
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
