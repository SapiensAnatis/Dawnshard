﻿using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Nintendo;

/// <summary>
/// This endpoint authenticates the user's DeviceAccount.
/// If a Nintendo account were linked, it would return information about the user, but that is not yet implemented.
/// </summary>
[ApiController]
[Obsolete("An external login server is now used, except for in the development environment.")]
[Consumes("application/json")]
[Produces("application/json")]
[Route("/core/v1/gateway/sdk/login")]
public class NintendoLoginController : ControllerBase
{
    private readonly ILogger<NintendoLoginController> logger;
    private readonly IConfiguration configuration;
    private readonly ISessionService sessionService;
    private readonly IDeviceAccountService deviceAccountService;

    public NintendoLoginController(
        ILogger<NintendoLoginController> logger,
        IConfiguration configuration,
        ISessionService sessionService,
        IDeviceAccountService deviceAccountService
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.sessionService = sessionService;
        this.deviceAccountService = deviceAccountService;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
    {
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
            TimeSpan.FromMinutes(this.configuration.GetValue<int>("SessionExpiryTimeMinutes")) / 2;

        LoginResponse response =
            new(token, deviceAccount, (int)reloginTime.TotalSeconds)
            {
                createdDeviceAccount = createdDeviceAccount
            };

        return Ok(response);
    }
}
