using System;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

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
        IItemSummonService itemSummonService
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.itemSummonService = itemSummonService;
    }

    [Route("get_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetList(ShopGetListRequest request)
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
                update_data_list = new UpdateDataList()
                {
                    functional_maintenance_list = new List<FunctionalMaintenanceList>()
                },
                user_item_summon = new AtgenUserItemSummon()
                {
                    daily_summon_count = 1,
                    last_summon_time = DateTime.UtcNow // getting rid of tempting red exclamation mark on daily item summon
                },
                infancy_paid_diamond_limit = 4800,
            };

        return Ok(response);
    }

    [Route("item_summon_odd")]
    public DragaliaResult GetOdds()
    {
        return this.Ok(new ShopItemSummonOddData(itemSummonService.GetOdds()));
    }
}
