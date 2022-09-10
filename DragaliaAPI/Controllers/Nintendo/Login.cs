using System;
using DragaliaAPI.Models.Nintendo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Controllers.Nintendo
{
    [ApiController]
    [Route("/core/v1/gateway/sdk/login")]
    public class NintendoLoginController : ControllerBase
    {
        private readonly ILogger<NintendoLoginController> _logger;

        public NintendoLoginController(ILogger<NintendoLoginController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public LoginResponse Post(LoginRequest request)
        {
            if (request.deviceAccount == null)
            {
                return LoginFactories.LoginResponseFactory_CreateDeviceAccount();
            }

            throw new NotImplementedException();
        }
    }
}
