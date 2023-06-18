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
    private readonly ILogger<StampController> logger;

    public StampController(
        IStampService stampService,
        IUpdateDataService updateDataService,
        ILogger<StampController> logger
    )
    {
        this.stampService = stampService;
        this.updateDataService = updateDataService;
        this.logger = logger;
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
        this.logger.LogDebug("Updating stamp list to: {@stampList}", request.stamp_list);

        IEnumerable<EquipStampList> newStampList = await this.stampService.SetEquipStampList(
            request.stamp_list
        );

        await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new StampSetEquipStampData() { equip_stamp_list = newStampList.OrderBy(x => x.slot) }
        );
    }
}
