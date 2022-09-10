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
        private readonly ILoginFactory _loginFactory;

        public NintendoLoginController(ILogger<NintendoLoginController> logger, ILoginFactory loginFactory)
        {
            _logger = logger;
            _loginFactory = loginFactory;
        }

        [HttpPost]
        public LoginResponse Post(LoginRequest request)
        {
            if (request.deviceAccount == null)
            {
                return _loginFactory.LoginResponseFactory();
            }

            throw new NotImplementedException();
        }
    }
}
