using System;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Models.Nintendo
{
    public class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> _logger;
        private readonly ISessionService _sessionService;

        public LoginService(ILogger<LoginService> logger, ISessionService sessionService)
        {
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<LoginResponse> Login(DeviceAccount deviceAccount)
        {
            // TODO: check credentials of deviceAccount against DB
            bool authenticationSuccess = await Task.FromResult(true);
            if (!authenticationSuccess) { throw new AuthenticationException("Could not authenticate DeviceAccount"); }

            string idToken = Guid.NewGuid().ToString();
            _sessionService.CreateNewSession(deviceAccount, idToken);

            _logger.LogInformation("Logged in user with deviceAccount id {id} and session id {session_id}", deviceAccount.id);
            return new LoginResponse(idToken, deviceAccount);
        }

        public async Task<DeviceAccount> DeviceAccountFactory()
        {
            // TODO: register this in the backend
            return new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }
    }
}
