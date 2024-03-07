//#define TEST

using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

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

        LoadIndexResponse data = await loadService.BuildIndexData();
        return this.Ok(data);
    }
}
