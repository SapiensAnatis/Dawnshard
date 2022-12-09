using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Nintendo;

/// <summary>
/// This endpoint authenticates the user's DeviceAccount.
/// If a Nintendo account were linked, it would return information about the user, but that is not yet implemented.
/// </summary>
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[NoSession]
[Route("/core/v1/gateway/sdk/login")]
public class NintendoLoginController : ControllerBase
{
    private readonly ILogger<NintendoLoginController> _logger;
    private readonly IConfiguration configuration;
    private readonly ISessionService _sessionService;
    private readonly IDeviceAccountService _deviceAccountService;

    public NintendoLoginController(
        ILogger<NintendoLoginController> logger,
        IConfiguration configuration,
        ISessionService sessionService,
        IDeviceAccountService deviceAccountService
    )
    {
        _logger = logger;
        this.configuration = configuration;
        _sessionService = sessionService;
        _deviceAccountService = deviceAccountService;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
    {
        DeviceAccount? deviceAccount = request.deviceAccount;
        DeviceAccount? createdDeviceAccount = null;
        if (deviceAccount is null)
        {
            createdDeviceAccount = await _deviceAccountService.RegisterDeviceAccount();
            deviceAccount = createdDeviceAccount;
        }

        bool authenticationSuccess = await _deviceAccountService.AuthenticateDeviceAccount(
            deviceAccount
        );
        if (!authenticationSuccess)
            return Unauthorized();

        string token = Guid.NewGuid().ToString();
        await _sessionService.PrepareSession(deviceAccount, token);

        TimeSpan reloginTime =
            TimeSpan.FromMinutes(configuration.GetValue<int>("SessionExpiryTimeMinutes"))
            - TimeSpan.FromSeconds(5);

        LoginResponse response =
            new(token, deviceAccount, (int)reloginTime.TotalSeconds)
            {
                createdDeviceAccount = createdDeviceAccount
            };

        return Ok(response);
    }
}
