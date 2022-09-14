using System;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly DeviceAccountContext _deviceAccountContext;

        public NintendoLoginController(
            ILogger<NintendoLoginController> logger,
            ISessionService sessionService,
            DeviceAccountContext context)
        {
            _logger = logger;
            _sessionService = sessionService;
            _deviceAccountContext = context;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
        {
            DeviceAccount? deviceAccount = request.deviceAccount;
            DeviceAccount? createdDeviceAccount = null;
            if (deviceAccount is null)
            {
                createdDeviceAccount = await _deviceAccountContext.RegisterDeviceAccount();
                deviceAccount = createdDeviceAccount;
            }

            bool authenticationSuccess = await _deviceAccountContext.AuthenticateDeviceAccount(deviceAccount);
            if (!authenticationSuccess) return Unauthorized();

            string token = Guid.NewGuid().ToString();
            _sessionService.CreateNewSession(deviceAccount, token);
            LoginResponse response = new(token, deviceAccount)
            {
                createdDeviceAccount = createdDeviceAccount
            };

            return response;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            return Ok(new List<string>() { "a", "b" });
        }
    }
}
