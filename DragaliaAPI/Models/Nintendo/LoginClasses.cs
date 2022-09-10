using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Models.Nintendo
{
    public class DeviceAccount
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

    public class LoginRequest
    {
        public DeviceAccount? deviceAccount { get; set; }

        [JsonConstructor]
        public LoginRequest(DeviceAccount? deviceAccount)
        {
            this.deviceAccount = deviceAccount;
        }
    }

    // The constructor for this class fills in many irrelevant fields, and takes parameters for the important ones.
    // It is intended to be called via a factory, so that DeviceAccounts can be validated against the DB.
    public class LoginResponse
    {
        public string accessToken { get; set; }
        public string behaviourSettings { get; set; }
        public Capability capability { get; set; }
        public DeviceAccount? createdDeviceAccount { get; set; }
        public string? error { get; set; }
        public int expiresIn { get; set; }
        public string idToken { get; set; }
        public string? market { get; set; }
        public string sessionId { get; set; }
        public User user { get; set; }

        public class Capability
        {
            public string accountApiHost { get; set; }
            public string accountHost { get; set; }
            public string pointProgramHost { get; set; }
            public long sessionUpdateInterval { get; set; }

            public Capability()
            {
                accountApiHost = "api.accounts.nintendo.com";
                accountHost = "accounts.nintendo.com";
                pointProgramHost = "my.nintendo.com";
                sessionUpdateInterval = 180000;
            }
        }

        public class User
        {
            public string birthday { get; set; }
            public string country { get; set; }
            public long createdAt { get; set; }
            public string gender { get; set; }
            public bool hasUnreadCsComment { get; set; }
            public string id { get; set; }
            public string links { get; set; }
            public string nickname { get; set; }
            public long updatedAt { get; set; }
            public List<DeviceAccount>  deviceAccounts { get; set; }
            public Permissions permissions { get; set; }

            public class Permissions
            {
                public bool personalAnalytics { get; set; }
                public long personalAnalyticsUpdatedAt { get; set; }
                public bool personalNotification { get; set; }
                public long personalNotificationUpdatedAt { get; set; }

                public Permissions()
                {
                    this.personalAnalytics = false;
                    this.personalAnalyticsUpdatedAt = 0;
                    this.personalNotification = false;
                    this.personalNotificationUpdatedAt = 0;
                }
            }

            public User(DeviceAccount deviceAccount)
            {
                this.birthday = "0000-00-00";
                this.country = "";
                this.createdAt = 0;
                this.gender = "Unknown";
                this.hasUnreadCsComment = false;
                this.id = "";
                this.links = "";
                this.nickname = "";
                this.updatedAt = 0;
                // This parameter is an array in the JSON response, despite the fact that it never seems to contain >1 account.
                this.deviceAccounts = new() { deviceAccount };
                this.permissions = new Permissions();
            }
        }

        public LoginResponse(string accessToken, string idToken, string sessionId, DeviceAccount deviceAccount)
        {
            this.accessToken = accessToken;
            this.behaviourSettings = "";
            this.capability = new Capability();
            this.error = null;
            this.expiresIn = int.MaxValue;
            this.idToken = idToken;
            this.market = null;
            this.sessionId = sessionId;
            this.user = new User(deviceAccount);
        }
    }
}
