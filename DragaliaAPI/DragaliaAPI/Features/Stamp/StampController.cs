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

        return this.Ok(new StampGetStampResponse() { StampList = list, });
    }

    [HttpPost("set_equip_stamp")]
    public async Task<DragaliaResult> SetEquipStamp(
        StampSetEquipStampRequest request,
        CancellationToken cancellationToken
    )
    {
        this.logger.LogDebug("Updating stamp list to: {@stampList}", request.StampList);

        IEnumerable<EquipStampList> newStampList = await this.stampService.SetEquipStampList(
            request.StampList
        );

        await this.updateDataService.SaveChangesAsync(cancellationToken);
        ;

        return this.Ok(
            new StampSetEquipStampResponse() { EquipStampList = newStampList.OrderBy(x => x.Slot) }
        );
    }
}
