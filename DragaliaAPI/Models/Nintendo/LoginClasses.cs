using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Models.Nintendo
{
    public record DeviceAccount
    {
        public string id { get; set; }
        public string? password { get; set; }  

        [JsonConstructor]
        public DeviceAccount(string id, string? password)
        {
            this.id = id;
            this.password = password;
        }
    }

    // We only need to deserialize deviceAccount from the request. The rest of it is useless
    public record LoginRequest(DeviceAccount? deviceAccount);

    // Conversely, we should probably fill in all the useless data in the response in case the client expects it
    public record LoginResponse
    {
        public string idToken;
        public User user;
        public DeviceAccount? createdDeviceAccount;

        // Junk fields
        public string accessToken = "";
        public string behaviourSettings = "";
        public Capability capability = new();
        public string? error = null;
        public int expiresIn = int.MaxValue;
        public string? market = null;

        public LoginResponse(string idToken, DeviceAccount deviceAccount)
        {
            this.idToken = idToken;
            this.user = new(deviceAccount);
        }

        [JsonConstructor]
        public LoginResponse()
        {
        }

        public record Capability
        {
            public string accountApiHost = "api.accounts.nintendo.com";
            public string accountHost = "accounts.nintendo.com";
            public string pointProgramHost = "my.nintendo.com";
            public long sessionUpdateInterval = long.MaxValue;
        }

        public record User
        {
            public User(DeviceAccount deviceAccount)
            {
                this.deviceAccounts = new() { deviceAccount };
            }

            [JsonConstructor]
            public User() { }

            public string birthday = "0000-00-00";
            public string country = "";
            public long createdAt = 0;
            public string gender = "Unknown";
            public bool hasUnreadCsComment = false;
            public string id = "";
            public string links = "";
            public string nickname = "";
            public long updatedAt = 0;
            public List<DeviceAccount> deviceAccounts;
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
