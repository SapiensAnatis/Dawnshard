using System;

namespace DragaliaAPI.Models.Nintendo
{
    public class LoginService : ILoginService
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<LoginService> _logger;

        public LoginService(ISessionService sessionService, ILogger<LoginService> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public LoginResponse LoginResponseFactory()
        {
            DeviceAccount newDeviceAccount = DeviceAccountFactory();

            LoginResponse result = LoginResponseFactory(newDeviceAccount);
            result.createdDeviceAccount = newDeviceAccount;
            _logger.LogInformation("Registered new user with deviceAccount id {id}", newDeviceAccount.id);

            return result;
        }
        public LoginResponse LoginResponseFactory(DeviceAccount deviceAccount)
        {
            // TODO: check against DB
            string sessionId = _sessionService.CreateNewSession(deviceAccount);

            LoginResponse result = new("accessToken", "idToken", sessionId, deviceAccount);
            _logger.LogInformation("Logged in user with deviceAccount id {id} and session id {session_id}", deviceAccount.id, sessionId);
            return result;
        }

        public DeviceAccount DeviceAccountFactory()
        {
            // TODO: register this in the backend
            return new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }
    }
}
