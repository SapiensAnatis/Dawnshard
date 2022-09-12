using System;
using System.Security.Authentication;
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
        private readonly ILoginService _loginService;

        public NintendoLoginController(ILogger<NintendoLoginController> logger, ILoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
        {
            DeviceAccount? deviceAccount = request.deviceAccount;
            if (deviceAccount is null)
            {
                DeviceAccount createdDeviceAccount = await _loginService.DeviceAccountFactory();
                // Should never return unauthorized if just created, no need for try/catch
                LoginResponse response = await _loginService.Login(createdDeviceAccount);
                return Ok(response);
            }

            try
            {
                LoginResponse response = await _loginService.Login(deviceAccount);
                return Ok(response);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
        }
    }
}
