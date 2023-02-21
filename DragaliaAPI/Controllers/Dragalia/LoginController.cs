using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
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

    public LoginController(
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
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
            await userDataRepository.GetUserData(DeviceAccountId).FirstAsync()
            ?? throw new DragaliaException(ResultCode.CommonDataNotFoundError);

        if (userData.LastLoginTime < DateTimeOffset.UtcNow.Date.AddHours(6))
        {
            await inventoryRepository.RefreshPurchasableDragonGiftCounts(DeviceAccountId);
        }

        userData.LastLoginTime = DateTimeOffset.UtcNow;

        await userDataRepository.SaveChangesAsync();
        return Code(ResultCode.Success);
    }

    [HttpPost]
    [Route("verify_jws")]
    public DragaliaResult VerifyJws()
    {
        return Code(ResultCode.Success);
    }
}
