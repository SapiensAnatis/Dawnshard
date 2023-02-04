using DragaliaAPI.Shared;

namespace DragaliaAPI.Models.Nintendo;

// Conversely, we should probably fill in all the useless data in the response in case the client expects it
[Obsolete(ObsoleteReasons.BaaS)]
public record LoginResponse
{
    public string idToken { get; init; }
    public User user { get; init; }
    public DeviceAccount? createdDeviceAccount { get; set; }

    // Junk fields
    public string accessToken { get; init; }
    public object behaviorSettings { get; } = new { };
    public Capability capability { get; } = new();
    public string? error { get; } = null;

    /// <summary>
    /// Session expiry time in seconds, after which /sdk/login will be called again and a new ID token issued.
    /// </summary>
    public int expiresIn { get; init; }
    public string? market { get; } = null;

    public LoginResponse(string idToken, DeviceAccount deviceAccount, int expiresIn)
    {
        this.idToken = idToken;
        this.accessToken = idToken;
        this.user = new(deviceAccount);
        this.expiresIn = expiresIn;
    }

    public record Capability
    {
        public string accountApiHost { get; } = "api.accounts.nintendo.com";
        public string accountHost { get; } = "accounts.nintendo.com";
        public string pointProgramHost { get; } = "my.nintendo.com";
        public long sessionUpdateInterval { get; } = 1000;
    }

    public record User
    {
        public User(DeviceAccount deviceAccount)
        {
            this.deviceAccounts = new() { deviceAccount };
            id = deviceAccount.id;
        }

        public string birthday { get; } = "0000-00-00";
        public string country { get; } = "";
        public long createdAt { get; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string gender { get; } = "Unknown";
        public bool hasUnreadCsComment { get; } = false;
        public string id { get; } = "20f92082aa3997e9";
        public object links { get; } = new { };
        public string nickname { get; } = "";
        public long updatedAt { get; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public List<DeviceAccount> deviceAccounts { get; init; }
        public Permissions permissions { get; } = new();

        public record Permissions
        {
            public bool personalAnalytics { get; } = false;
            public long personalAnalyticsUpdatedAt { get; } =
                DateTimeOffset.Now.ToUnixTimeSeconds();
            public bool personalNotification { get; } = false;
            public long personalNotificationUpdatedAt { get; } =
                DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
