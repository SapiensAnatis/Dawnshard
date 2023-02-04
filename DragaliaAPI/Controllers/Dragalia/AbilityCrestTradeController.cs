using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("ability_crest_trade")]
public class AbilityCrestTradeController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUpdateDataService updateDataService;

    public AbilityCrestTradeController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IUpdateDataService updateDataService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.updateDataService = updateDataService;
    }

    [Route("get_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetList(AbilityCrestTradeGetListRequest request)
    {
        IEnumerable<DbAbilityCrest> ownedAbilityCrests = await unitRepository
            .GetAllAbilityCrestData(this.DeviceAccountId)
            .ToListAsync();

        IEnumerable<AbilityCrestTrade> abilityCrestTradeList = MasterAsset
            .AbilityCrestTrade
            .Enumerable;

        AbilityCrestTradeGetListData response =
            new()
            {
                user_ability_crest_trade_list = new List<UserAbilityCrestTradeList>(),
                ability_crest_trade_list = abilityCrestTradeList
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
                    ),
                update_data_list = this.updateDataService.GetUpdateDataList(this.DeviceAccountId)
            };

        return Ok(response);
    }
}
