/*using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Controllers.Nintendo;

[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class NintendoMarketsController : ControllerBase
{
    private const string MarketGoogle = "GOOGLE";
    private const string MarketIOS = "IOSMAYBEDUNNO";

    private ISessionService _sessionService;
    private ApiContext _apiContext;

    public NintendoMarketsController(ISessionService sessionService, ApiContext apiContext)
    {
        _sessionService = sessionService;
        _apiContext = apiContext;
    }

    [Route(
        $"/vcm/v1/users/{{userId=0}}/markets/{{storeType:regex({MarketGoogle}|{MarketIOS})={MarketGoogle}}}/wallets"
    )]
    [HttpGet]
    public async Task<DragaliaResult> GetDiamantium(
        [FromRoute(Name = "userId")] string userId,
        [FromRoute(Name = "storeType")] string storeType,
        [FromHeader(Name = "Authorization")] string auth
    )
    {
        if (
            userId.Equals("0")
            || (!storeType.Equals(MarketGoogle) && !storeType.Equals(MarketIOS))
            || string.IsNullOrWhiteSpace(auth)
            || !auth.TrimStart().StartsWith("bearer ", true, null)
        )
        {
            return BadRequest();
        }
        string idToken = auth.Substring("bearer ".Length);
        if (!idToken.Any())
        {
            return BadRequest();
        }
        string deviceAccountId = "";
        try
        {
            deviceAccountId = await _sessionService.GetDeviceAccountId_IdToken(idToken);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
        if (!string.Equals(userId, deviceAccountId))
        {
            return Unauthorized();
        }

        Dictionary<CurrencyTypes, long> dias = await _apiContext.PlayerWallet
            .Where(
                wallet =>
                    wallet.DeviceAccountId.Equals(deviceAccountId)
                    && (
                        wallet.CurrencyType == CurrencyTypes.FreeDiamantium
                        || wallet.CurrencyType == CurrencyTypes.PaidDiamantium
                    )
            )
            .ToDictionaryAsync(wallet => wallet.CurrencyType, wallet => wallet.Quantity);
        return Ok(
            new GetDiamantiumResponse(
                new()
                {
                    userId = userId,
                    market = storeType,
                    remittedBalances = new(),
                    balance = new(
                        dias[CurrencyTypes.FreeDiamantium],
                        new(),
                        dias[CurrencyTypes.FreeDiamantium] + dias[CurrencyTypes.PaidDiamantium]
                    )
                }
            )
        );
    }
}
*/
