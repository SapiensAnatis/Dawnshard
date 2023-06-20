using System;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Shop;

[Route("shop")]
public class ShopController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IItemSummonService itemSummonService;

    public ShopController(
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IItemSummonService itemSummonService,
        IShopRepository shopRepository
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.itemSummonService = itemSummonService;
    }

    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        ShopGetListData response =
            new()
            {
                // i don't know what like half of these are for lmao
                is_quest_bonus = 0,
                is_stone_bonus = 0,
                is_stamina_bonus = 0,
                material_shop_purchase = new List<ShopPurchaseList>(),
                normal_shop_purchase = new List<ShopPurchaseList>(),
                special_shop_purchase = new List<ShopPurchaseList>(),
                stone_bonus = new List<AtgenStoneBonus>(),
                stamina_bonus = new List<AtgenStaminaBonus>(),
                quest_bonus = new List<AtgenQuestBonus>(),
                product_lock_list = new List<AtgenProductLockList>(),
                product_list = new List<ProductList>(),
                infancy_paid_diamond_limit = 4800
            };

        response.user_item_summon = await this.itemSummonService.GetOrRefreshItemSummon();
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
        resp.user_item_summon = await this.itemSummonService.GetOrRefreshItemSummon();
        resp.update_data_list = await this.updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
