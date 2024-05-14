using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Login.Actions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

[Route("login")]
[BypassDailyReset]
public class LoginController(
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IEnumerable<IDailyResetAction> resetActions,
    ILogger<LoginController> logger,
    ILoginService loginService,
    IRewardService rewardService,
    TimeProvider dateTimeProvider,
    IDragonService dragonService
) : DragaliaControllerBase
{
    [HttpPost]
    public IActionResult Login()
    {
        // These fucking webcrawlers keep calling /login
        // and ASP.NET throws a 500 because of ambiguous route
        return this.NotFound();
    }

    [HttpPost]
    [Route("index")]
    public async Task<DragaliaResult> Index(CancellationToken cancellationToken)
    {
        LoginIndexResponse resp = new();

        // TODO: Implement daily login bonuses/notifications/resets (status: daily login bonus done)
        DbPlayerUserData userData =
            await userDataRepository.UserData.FirstOrDefaultAsync(cancellationToken)
            ?? throw new DragaliaException(ResultCode.CommonDataNotFoundError);

        if (userData.LastLoginTime < dateTimeProvider.GetLastDailyReset())
        {
            foreach (IDailyResetAction action in resetActions)
            {
                logger.LogInformation("Applying daily reset action: {$action}", action);
                await action.Apply();
            }

            resp.LoginBonusList = await loginService.RewardLoginBonus();
        }

        userData.LastLoginTime = dateTimeProvider.GetUtcNow();

        resp.MonthlyWallReceiveList = await loginService.GetWallMonthlyReceiveList();
        resp.DragonContactFreeGiftCount = await dragonService.GetFreeGiftCount();

        resp.PenaltyData = new AtgenPenaltyData();

        // NOTE: Cancelling the request + savefile updates may cause issues with request loops on debug builds,
        // but it should be fine on the actual server.
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        resp.EntityResult = rewardService.GetEntityResult();
        resp.ServerTime = dateTimeProvider.GetUtcNow();

        return this.Ok(resp);
    }

    [HttpPost]
    [Route("verify_jws")]
    public DragaliaResult VerifyJws()
    {
        return Code(ResultCode.Success);
    }
}
