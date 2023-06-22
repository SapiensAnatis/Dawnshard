using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
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
    private readonly IUpdateDataService updateDataService;
    private readonly IPresentService presentService;
    private readonly ILogger<LoginController> logger;

    public LoginController(
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IPresentService presentService,
        ILogger<LoginController> logger
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.presentService = presentService;
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
        DateTimeOffset reset = DateTimeOffset.UtcNow.Date.AddHours(6);

        // TODO: Implement daily login bonuses/notifications/resets
        DbPlayerUserData userData =
            await userDataRepository.UserData.FirstOrDefaultAsync()
            ?? throw new DragaliaException(ResultCode.CommonDataNotFoundError);

        if (DateTimeOffset.UtcNow > reset && userData.LastLoginTime < reset)
        {
            await inventoryRepository.RefreshPurchasableDragonGiftCounts();

            this.presentService.AddPresent(
                new Present(
                    PresentMessage.DragaliaLostTeam,
                    EntityTypes.Material,
                    (int)Materials.ChampionsTestament,
                    5
                )
            );

            this.presentService.AddPresent(
                new Present(
                    PresentMessage.DragaliaLostTeam,
                    EntityTypes.Material,
                    (int)Materials.KnightsTestament,
                    5
                )
            );

            this.presentService.AddPresent(
                new Present(
                    PresentMessage.DragaliaLostTeam,
                    EntityTypes.Material,
                    (int)Materials.Omnicite
                )
            );

            this.presentService.AddPresent(
                new Present(
                    PresentMessage.DragaliaLostTeam,
                    EntityTypes.Material,
                    (int)Materials.TwinklingSand,
                    10
                )
            );

            this.presentService.AddPresent(
                new Materials[]
                {
                    Materials.FlameTome,
                    Materials.WindTome,
                    Materials.WaterTome,
                    Materials.ShadowTome,
                    Materials.LightTome
                }.Select(
                    x =>
                        new Present(
                            PresentMessage.DragaliaLostTeam,
                            EntityTypes.Material,
                            (int)x,
                            5
                        )
                )
            );
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
