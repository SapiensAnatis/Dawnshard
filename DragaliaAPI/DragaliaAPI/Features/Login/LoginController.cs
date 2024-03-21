using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
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
[BypassDailyReset]
public class LoginController(
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IEnumerable<IDailyResetAction> resetActions,
    IResetHelper resetHelper,
    ILogger<LoginController> logger,
    ILoginBonusService loginBonusService,
    IRewardService rewardService,
    TimeProvider dateTimeProvider
) : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository = userDataRepository;
    private readonly IUpdateDataService updateDataService = updateDataService;
    private readonly IEnumerable<IDailyResetAction> resetActions = resetActions;
    private readonly IResetHelper resetHelper = resetHelper;
    private readonly ILogger<LoginController> logger = logger;
    private readonly ILoginBonusService loginBonusService = loginBonusService;
    private readonly IRewardService rewardService = rewardService;
    private readonly TimeProvider dateTimeProvider = dateTimeProvider;

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

        if (userData.LastLoginTime < resetHelper.LastDailyReset)
        {
            foreach (IDailyResetAction action in resetActions)
            {
                this.logger.LogDebug("Applying daily reset action: {$action}", action);
                await action.Apply();
            }

            resp.LoginBonusList = await this.loginBonusService.RewardLoginBonus();
        }

        userData.LastLoginTime = this.dateTimeProvider.GetUtcNow();

        resp.PenaltyData = new AtgenPenaltyData();
        resp.DragonContactFreeGiftCount = 1;
        resp.LoginLotteryRewardList = Enumerable.Empty<AtgenLoginLotteryRewardList>();
        resp.ExchangeSummomPointList = Enumerable.Empty<AtgenExchangeSummomPointList>();
        resp.MonthlyWallReceiveList = Enumerable.Empty<AtgenMonthlyWallReceiveList>();

        // NOTE: This may cause issues on debug builds but should be fine on the actual server.
        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        resp.EntityResult = this.rewardService.GetEntityResult();
        resp.ServerTime = this.dateTimeProvider.GetUtcNow();

        return this.Ok(resp);
    }

    [HttpPost]
    [Route("verify_jws")]
    public DragaliaResult VerifyJws()
    {
        return Code(ResultCode.Success);
    }
}
