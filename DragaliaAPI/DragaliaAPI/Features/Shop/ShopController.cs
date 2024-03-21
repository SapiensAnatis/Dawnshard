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
    public async Task<DragaliaResult> GetList(CancellationToken cancellationToken)
    {
        ILookup<PurchaseShopType, ShopPurchaseList> purchases =
            await this.shopService.GetPurchases();

        ShopGetListResponse response =
            new()
            {
                // i don't know what like half of these are for lmao
                IsQuestBonus = false,
                IsStoneBonus = false,
                IsStaminaBonus = false,
                MaterialShopPurchase = purchases[PurchaseShopType.Material],
                NormalShopPurchase = purchases[PurchaseShopType.Normal],
                SpecialShopPurchase = purchases[PurchaseShopType.Special],
                StoneBonus = new List<AtgenStoneBonus>(),
                StaminaBonus = new List<AtgenStaminaBonus>(),
                QuestBonus = new List<AtgenQuestBonus>(),
                ProductLockList = new List<AtgenProductLockList>(),
                ProductList = new List<ProductList>(),
                InfancyPaidDiamondLimit = 4800
            };

        response.UserItemSummon = await this.itemSummonService.GetItemSummon();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(response);
    }

    [HttpPost("item_summon_odd")]
    public DragaliaResult GetOdds()
    {
        return Ok(new ShopItemSummonOddResponse(itemSummonService.GetOdds()));
    }

    [HttpPost("item_summon_exec")]
    public async Task<DragaliaResult> ExecItemSummon(
        ShopItemSummonExecRequest request,
        CancellationToken cancellationToken
    )
    {
        ShopItemSummonExecResponse resp = new();

        resp.ItemSummonRewardList = await this.itemSummonService.DoSummon(request);
        resp.UserItemSummon = await this.itemSummonService.GetItemSummon();
        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        resp.EntityResult = this.rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("material_shop_purchase")]
    public async Task<DragaliaResult> MaterialShopPurchase(
        ShopMaterialShopPurchaseRequest request,
        CancellationToken cancellationToken
    )
    {
        ShopMaterialShopPurchaseResponse resp = new();

        resp.MaterialShopPurchase = await this.shopService.DoPurchase(
            request.ShopType.ToShopType(),
            request.PaymentType,
            request.GoodsId,
            request.Quantity
        );

        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("normal_shop_purchase")]
    public async Task<DragaliaResult> NormalShopPurchase(
        ShopNormalShopPurchaseRequest request,
        CancellationToken cancellationToken
    )
    {
        ShopNormalShopPurchaseResponse resp = new();

        resp.NormalShopPurchase = await this.shopService.DoPurchase(
            ShopType.Normal,
            request.PaymentType,
            request.GoodsId,
            request.Quantity
        );

        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("special_shop_purchase")]
    public async Task<DragaliaResult> SpecialShopPurchase(
        ShopSpecialShopPurchaseRequest request,
        CancellationToken cancellationToken
    )
    {
        ShopSpecialShopPurchaseResponse resp = new();

        resp.SpecialShopPurchase = await this.shopService.DoPurchase(
            ShopType.Special,
            request.PaymentType,
            request.GoodsId,
            request.Quantity
        );

        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }
}
