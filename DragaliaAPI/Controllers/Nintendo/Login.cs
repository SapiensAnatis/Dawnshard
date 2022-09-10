using System;
using DragaliaAPI.Models.Nintendo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Controllers.Nintendo
{
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
        public LoginResponse Post(LoginRequest request)
        {
            if (request.deviceAccount == null)
            {
                return _loginService.LoginResponseFactory();
            }

            throw new NotImplementedException();
        }
    }
}
