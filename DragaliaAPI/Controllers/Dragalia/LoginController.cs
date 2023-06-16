using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.SavefilePorter;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("login")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class LoginController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IEnumerable<ISavefilePorter> savefilePorters;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<LoginController> logger;

    public LoginController(
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IEnumerable<ISavefilePorter> savefilePorters,
        IUpdateDataService updateDataService,
        ILogger<LoginController> logger
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.savefilePorters = savefilePorters;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost]
    public IActionResult Login()
    {
        // These fucking webcrawlers keep calling /login
        // and ASP.NET throws a 500 because of ambiguous route
        return this.NotFound();
    }

    [HttpPost]
    [Route("index")]
    public async Task<DragaliaResult> Index()
    {
        // TODO: Implement daily login bonuses/notifications/resets
        DbPlayerUserData userData =
            await userDataRepository.UserData.FirstAsync()
            ?? throw new DragaliaException(ResultCode.CommonDataNotFoundError);

        if (userData.LastLoginTime < DateTimeOffset.UtcNow.Date.AddHours(6))
        {
            await inventoryRepository.RefreshPurchasableDragonGiftCounts();
        }

        foreach (ISavefilePorter porter in savefilePorters)
        {
            if (porter.ModificationDate <= userData.LastLoginTime)
                continue;

            this.logger.LogInformation("Applying savefile porter {$porter}", porter);
            await porter.Port();
        }

        userData.LastLoginTime = DateTimeOffset.UtcNow;

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(new LoginIndexData() { update_data_list = updateDataList });
    }

    [HttpPost]
    [Route("verify_jws")]
    public DragaliaResult VerifyJws()
    {
        return Code(ResultCode.Success);
    }
}
