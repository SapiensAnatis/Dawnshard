using System;
using System.Security.Authentication;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Controllers.Nintendo
{
    /// <summary>
    /// This endpoint authenticates the user's DeviceAccount. 
    /// If a Nintendo account were linked, it would return information about the user, but that is not yet implemented.
    /// </summary>
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("/core/v1/gateway/sdk/login")]
    public class NintendoLoginController : ControllerBase
    {
        private readonly ILogger<NintendoLoginController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IDeviceAccountService _deviceAccountService;

        public NintendoLoginController(
            ILogger<NintendoLoginController> logger,
            ISessionService sessionService,
            IDeviceAccountService deviceAccountService)
        {
            _logger = logger;
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

            bool authenticationSuccess = await _deviceAccountService.AuthenticateDeviceAccount(deviceAccount);
            if (!authenticationSuccess) return Unauthorized();

            string token = Guid.NewGuid().ToString();
            _sessionService.CreateNewSession(deviceAccount, token);
            LoginResponse response = new(token, deviceAccount)
            {
                createdDeviceAccount = createdDeviceAccount
            };

            return Ok(response);
        }
    }
}
