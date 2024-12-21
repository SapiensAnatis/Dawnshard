using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dragons;
using DragaliaAPI.Features.Login.Actions;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Infrastructure.Middleware;
using DragaliaAPI.Mapping;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

[Route("login")]
[BypassDailyReset]
public class LoginController(
    ApiContext apiContext,
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
            await apiContext.PlayerUserData.FirstOrDefaultAsync(cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.CommonDataNotFoundError,
                "No player user data found"
            );

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

        // Hack to show diamantium on login. This is normally returned by a BaaS endpoint which we don't control, but
        // you can set it via the update_data_list.
        resp.UpdateDataList.DiamondData =
            await apiContext
                .PlayerDiamondData.ProjectToDiamondData()
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.CommonDataNotFoundError,
                "No diamond data found"
            );

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
