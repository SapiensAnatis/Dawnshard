using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
        AbilityCrestTradeGetListData response = new();

        return Ok(response);
    }
}
