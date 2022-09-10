using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Models.Nintendo
{
    public record DeviceAccount(string id, string? password);

    // We only need to deserialize deviceAccount from the request. The rest of it is useless
    public record LoginRequest(DeviceAccount? deviceAccount);

    // Conversely, we should probably fill in all the useless data in the response in case the client expects it
    public record LoginResponse(string accessToken, string idToken, string sessionId, DeviceAccount deviceAccount)
    {
        public string behaviourSettings = "";
        public Capability capability = new();
        public DeviceAccount? createdDeviceAccount;
        public string? error = null;
        public int expiresIn = int.MaxValue;
        public string? market = null;
        public User user = new(deviceAccount);

        public record Capability()
        {
            public string accountApiHost = "api.accounts.nintendo.com";
            public string accountHost = "accounts.nintendo.com";
            public string pointProgramHost = "my.nintendo.com";
            public long sessionUpdateInterval = 180000;
        }

        public record User(DeviceAccount deviceAccount)
        {
            public string birthday = "0000-00-00";
            public string country = "";
            public long createdAt = 0;
            public string gender = "Unknown";
            public bool hasUnreadCsComment = false;
            public string id = "";
            public string links = "";
            public string nickname = "";
            public long updatedAt = 0;
            public List<DeviceAccount> deviceAccounts = new() { deviceAccount };
            public Permissions permissions = new();

            public record Permissions
            {
                public bool personalAnalytics = false;
                public long personalAnalyticsUpdatedAt = 0;
                public bool personalNotification = false;
                public long personalNotificationUpdatedAt = 0;
            }
        }
    }
}
