//#define TEST

using DragaliaAPI.Features.Login.SavefileUpdate;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Login.Savefile;

[Route("load")]
public class LoadController : DragaliaControllerBase
{
    private readonly ILoadService loadService;
    private readonly ISavefileUpdateService savefileUpdateService;

    public LoadController(ILoadService loadService, ISavefileUpdateService savefileUpdateService)
    {
        this.loadService = loadService;
        this.savefileUpdateService = savefileUpdateService;
    }

    [Route("index")]
    [HttpPost]
    public async Task<DragaliaResult> Index()
    {
        await this.savefileUpdateService.UpdateSavefile();

        LoadIndexResponse data = await this.loadService.BuildIndexData();
        return this.Ok(data);
    }
}
