using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

[Route("login")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class LoginController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IEnumerable<IDailyResetAction> resetActions;
    private readonly IResetHelper resetHelper;
    private readonly ILogger<LoginController> logger;

    public LoginController(
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        IEnumerable<IDailyResetAction> resetActions,
        IResetHelper resetHelper,
        ILogger<LoginController> logger
    )
    {
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.resetActions = resetActions;
        this.resetHelper = resetHelper;
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

        if (userData.LastLoginTime < resetHelper.LastDailyReset)
        {
            foreach (IDailyResetAction action in resetActions)
            {
                this.logger.LogDebug("Applying daily reset action: {$action}", action);
                await action.Apply();
            }
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
