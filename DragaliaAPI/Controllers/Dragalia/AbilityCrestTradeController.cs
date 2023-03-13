using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("ability_crest_trade")]
public class AbilityCrestTradeController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUpdateDataService updateDataService;

    public AbilityCrestTradeController(
        IUserDataRepository userDataRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUpdateDataService updateDataService
    )
    {
        this.userDataRepository = userDataRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.updateDataService = updateDataService;
    }

    [Route("get_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetList(AbilityCrestTradeGetListRequest request)
    {
        IEnumerable<AbilityCrestTradeList> abilityCrestTradeList =
            await BuildAbilityCrestTradeList();

        AbilityCrestTradeGetListData response =
            new()
            {
                user_ability_crest_trade_list = new List<UserAbilityCrestTradeList>(),
                ability_crest_trade_list = abilityCrestTradeList,
                update_data_list = this.updateDataService.GetUpdateDataList(this.DeviceAccountId)
            };

        return Ok(response);
    }

    [Route("trade")]
    [HttpPost]
    public async Task<DragaliaResult> Trade(AbilityCrestTradeTradeRequest request)
    {
        IEnumerable<AbilityCrestTradeList> abilityCrestTradeList =
            await BuildAbilityCrestTradeList();

        AbilityCrests abilityCrestId = MasterAsset.AbilityCrestTrade.Enumerable
            .Where(x => x.Id == request.ability_crest_trade_id)
            .Select(x => x.AbilityCrestId)
            .FirstOrDefault();

        await abilityCrestRepository.Add(abilityCrestId);
        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        AbilityCrestTradeTradeData response =
            new()
            {
                user_ability_crest_trade_list = new List<UserAbilityCrestTradeList>()
                {
                    new()
                    {
                        ability_crest_trade_id = request.ability_crest_trade_id,
                        trade_count = request.trade_count
                    }
                },
                ability_crest_trade_list = abilityCrestTradeList,
                update_data_list = updateDataList
            };

        return Ok(response);
    }

    private async Task<IEnumerable<AbilityCrestTradeList>> BuildAbilityCrestTradeList()
    {
        IEnumerable<DbAbilityCrest> ownedAbilityCrests =
            await abilityCrestRepository.AbilityCrests.ToListAsync();

        IEnumerable<AbilityCrestTrade> abilityCrestTradeList = MasterAsset
            .AbilityCrestTrade
            .Enumerable;

        return abilityCrestTradeList
            .Where(
                x =>
                    x.AbilityCrestId != 0
                    && !ownedAbilityCrests.Any(y => y.AbilityCrestId == x.AbilityCrestId)
            )
            .Select(
                x =>
                    new AbilityCrestTradeList()
                    {
                        ability_crest_trade_id = x.Id,
                        ability_crest_id = x.AbilityCrestId,
                        need_dew_point = x.NeedDewPoint,
                        priority = x.Priority,
                        complete_date = 0,
                        pickup_view_start_date = 0,
                        pickup_view_end_date = 0,
                    }
            );
    }
}
