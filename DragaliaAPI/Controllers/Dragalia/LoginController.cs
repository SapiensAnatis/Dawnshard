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
        // TODO: Implement daily login bonuses/notifications/resets
        DbPlayerUserData userData =
            await userDataRepository.UserData.FirstOrDefaultAsync()
            ?? throw new DragaliaException(ResultCode.CommonDataNotFoundError);

        if (userData.LastLoginTime < DateTimeOffset.UtcNow.Date.AddHours(6))
        {
            await inventoryRepository.RefreshPurchasableDragonGiftCounts();
        }

        userData.LastLoginTime = DateTimeOffset.UtcNow;

#if DEBUG
        this.presentService.AddPresent(
            new Present(PresentMessage.Maintenance, EntityTypes.Chara, (int)Charas.Addis)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );

        this.presentService.AddPresent(
            new Present(PresentMessage.DragonBond, EntityTypes.Dragon, (int)Dragons.Agni)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );


        this.presentService.AddPresent(
            new Present(PresentMessage.AdventurerStoryRead, EntityTypes.Dew, 0)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );

        this.presentService.AddPresent(
            new Present(PresentMessage.DragonBond, EntityTypes.HustleHammer, 0, 8_000)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );

        this.presentService.AddPresent(
            new Present(PresentMessage.FirstClear, EntityTypes.Rupies, 0, 100_000)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );

        this.presentService.AddPresent(
            new Present(PresentMessage.QuestGrandBounty, EntityTypes.Wyrmite, 1_200)
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );

        this.presentService.AddPresent(
            new Present(
                PresentMessage.TeamFormationTutorial,
                EntityTypes.Material,
                (int)Materials.Squishums
            )
            {
                ExpiryTime = TimeSpan.FromHours(1),
                MessageParamValues = new[] { 100000101 }
            }
        );
#endif

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
