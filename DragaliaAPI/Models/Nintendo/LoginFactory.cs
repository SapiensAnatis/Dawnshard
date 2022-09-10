using System;

namespace DragaliaAPI.Models.Nintendo
{
    public class LoginFactory : ILoginFactory
    {
        private readonly ISessionService _sessionService;

        public LoginFactory(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public LoginResponse LoginResponseFactory()
        {
            DeviceAccount newDeviceAccount = DeviceAccountFactory();

            LoginResponse result = LoginResponseFactory(newDeviceAccount);
            result.createdDeviceAccount = newDeviceAccount;

            return result;
        }
        public LoginResponse LoginResponseFactory(DeviceAccount deviceAccount)
        {
            // TODO: check against DB
            string sessionId = _sessionService.CreateNewSession(deviceAccount);

            LoginResponse result = new("accessToken", "idToken", sessionId, deviceAccount);
            return result;
        }

        public DeviceAccount DeviceAccountFactory()
        {
            // TODO: register this in the backend
            return new()
            {
                id = Guid.NewGuid().ToString(),
                password = Guid.NewGuid().ToString(),
            };
        }
    }
}
