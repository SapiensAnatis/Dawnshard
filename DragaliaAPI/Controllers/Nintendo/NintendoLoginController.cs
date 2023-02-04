using System.Net;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Controllers.Nintendo;

/// <summary>
/// This endpoint authenticates the user's DeviceAccount.
/// If a Nintendo account were linked, it would return information about the user, but that is not yet implemented.
/// </summary>
[ApiController]
[Obsolete(ObsoleteReasons.BaaS)]
[Consumes("application/json")]
[Produces("application/json")]
[Route("/core/v1/gateway/sdk/login")]
public class NintendoLoginController : ControllerBase
{
    private readonly ILogger<NintendoLoginController> logger;
    private readonly IConfiguration configuration;
    private readonly ISessionService sessionService;
    private readonly IDeviceAccountService deviceAccountService;
    private readonly IOptionsMonitor<LoginOptions> options;

    public NintendoLoginController(
        ILogger<NintendoLoginController> logger,
        IConfiguration configuration,
        ISessionService sessionService,
        IDeviceAccountService deviceAccountService,
        IOptionsMonitor<LoginOptions> options
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.sessionService = sessionService;
        this.deviceAccountService = deviceAccountService;
        this.options = options;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
    {
        if (this.options.CurrentValue.UseBaasLogin)
        {
            // If BaaS login is enabled, modern patched clients should not be calling this endpoint.
            // However if an outdated client does call it, the "token" it receives will fail to decode
            // and throw an exception at /tool/auth. So return an obvious error code before this can happen.
            this.logger.LogWarning(
                "Received Nintendo login request, but BaaS login is enabled. Returning 418 - I'm a teapot."
            );
            return this.StatusCode(418);
        }

        DeviceAccount? deviceAccount = request.deviceAccount;
        DeviceAccount? createdDeviceAccount = null;

        if (deviceAccount is null)
        {
            createdDeviceAccount = await this.deviceAccountService.RegisterDeviceAccount();
            deviceAccount = createdDeviceAccount;
        }

        bool authenticationSuccess = await this.deviceAccountService.AuthenticateDeviceAccount(
            deviceAccount
        );

        if (!authenticationSuccess)
        {
            this.logger.LogWarning("Access denied to DeviceAccount ID '{id}'", deviceAccount.id);
            return Unauthorized();
        }

        string token = Guid.NewGuid().ToString();
        await this.sessionService.PrepareSession(deviceAccount, token);

        TimeSpan reloginTime =
            TimeSpan.FromMinutes(this.configuration.GetValue<int>("Redis:SessionExpiryTimeMinutes"))
            / 2;

        LoginResponse response =
            new(token, deviceAccount, (int)reloginTime.TotalSeconds)
            {
                createdDeviceAccount = createdDeviceAccount
            };

        return Ok(response);
    }
}
