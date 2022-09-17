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
        public string idToken { get; init; }
        public User user { get; init; }
        public DeviceAccount? createdDeviceAccount { get; set; }
        // Junk fields
        public string accessToken { get; } = "";
        public string behaviourSettings { get; } = "";
        public Capability capability { get; } = new();
        public string? error { get; } = null;
        public int expiresIn { get; } = int.MaxValue;
        public string? market { get; } = null;

        public LoginResponse(string idToken, DeviceAccount deviceAccount)
        {
            this.idToken = idToken;
            this.user = new(deviceAccount);
        }

        public record Capability
        {
            public string accountApiHost { get; } = "api.accounts.nintendo.com";
            public string accountHost { get; } = "accounts.nintendo.com";
            public string pointProgramHost { get; } = "my.nintendo.com";
            public long sessionUpdateInterval { get; } = long.MaxValue;
        }

        public record User
        {
            public User(DeviceAccount deviceAccount)
            {
                this.deviceAccounts = new() { deviceAccount };
            }

            public string birthday { get; } = "0000-00-00";
            public string country { get; } = "";
            public long createdAt { get; } = 0;
            public string gender { get; } = "Unknown";
            public bool hasUnreadCsComment { get; } = false;
            public string id { get; } = "";
            public string links { get; } = "";
            public string nickname { get; } = "";
            public long updatedAt { get; } = 0;
            public List<DeviceAccount> deviceAccounts { get; init; }
            public Permissions permissions { get; } = new();
            public record Permissions
            {
                public bool personalAnalytics { get; } = false;
                public long personalAnalyticsUpdatedAt { get; } = 0;
                public bool personalNotification { get; } = false;
                public long personalNotificationUpdatedAt { get; } = 0;
            }
        }
    }
}
