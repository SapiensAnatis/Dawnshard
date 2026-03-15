using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.CoOp.Stamps;

[Route("stamp")]
public partial class StampController : DragaliaControllerBase
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

        return this.Ok(new StampGetStampResponse() { StampList = list });
    }

    [HttpPost("set_equip_stamp")]
    public async Task<DragaliaResult> SetEquipStamp(
        StampSetEquipStampRequest request,
        CancellationToken cancellationToken
    )
    {
        Log.UpdatingStampListTo(this.logger, request.StampList);

        IEnumerable<EquipStampList> newStampList = await this.stampService.SetEquipStampList(
            request.StampList
        );

        await this.updateDataService.SaveChangesAsync(cancellationToken);
        ;

        return this.Ok(
            new StampSetEquipStampResponse() { EquipStampList = newStampList.OrderBy(x => x.Slot) }
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Updating stamp list to: {@stampList}")]
        public static partial void UpdatingStampListTo(
            ILogger logger,
            IEnumerable<EquipStampList> stampList
        );
    }
}
