using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Stamp;

[Route("stamp")]
public class StampController : DragaliaControllerBase
{
    private readonly IStampService stampService;
    private readonly IUpdateDataService updateDataService;

    public StampController(IStampService stampService, IUpdateDataService updateDataService)
    {
        this.stampService = stampService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("get_stamp")]
    public async Task<DragaliaResult> GetStamp()
    {
        IEnumerable<StampList> list = (await this.stampService.GetStampList()).ToList();

        return this.Ok(new StampGetStampData() { stamp_list = list, });
    }

    [HttpPost("set_equip_stamp")]
    public async Task<DragaliaResult> SetEquipStamp(StampSetEquipStampRequest request)
    {
        IEnumerable<EquipStampList> newStampList = await this.stampService.SetEquipStampList(
            request.stamp_list
        );

        await this.updateDataService.SaveChangesAsync();

        return this.Ok(new StampSetEquipStampData() { equip_stamp_list = newStampList });
    }
}
