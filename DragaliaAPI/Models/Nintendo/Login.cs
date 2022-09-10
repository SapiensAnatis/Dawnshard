#nullable enable
using System;
using System.Collections.Generic;
using System.Security;
using DragaliaAPI.Models.Nintendo;
using MessagePack;
using System.Security.Cryptography;

namespace DragaliaAPI.Models.Nintendo
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DeviceAccount
    {
        public string id { get; set; }
        public string? password { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class LoginRequest
    {
        public string appVersion { get; set; }
        public string assertion { get; set; }
        public string carrier { get; set; }
        public string deviceAnalyticsId { get; set; }
        public string deviceName { get; set; }
        public string locale { get; set; }
        public string manufacturer { get; set; }
        public string networkType { get; set; }
        public string osType { get; set; }
        public string osVersion { get; set; }
        public string sdkVersion { get; set; }
        public string sessionId { get; set; }
        public string timeZone { get; set; }
        public long timeZoneOffset { get; set; }
        public DeviceAccount? deviceAccount { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
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

        [MessagePackObject(keyAsPropertyName: true)]
        public class Capability
        {
            public string accountApiHost { get; set; }
            public string accountHost { get; set; }
            public string pointProgramHost { get; set; }
            public long sessionUpdateInterval { get; set; }
        }

        [MessagePackObject(keyAsPropertyName: true)]
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

            [MessagePackObject(keyAsPropertyName: true)]
            public class Permissions
            {
                public bool personalAnalytics { get; set; }
                public long personalAnalyticsUpdatedAt { get; set; }
                public bool personalNotification { get; set; }
                public long personalNotificationUpdatedAt { get; set; }
            }
        }
    }

    public static class LoginFactories
    {
        public static LoginResponse LoginResponseFactory_CreateDeviceAccount()
        {
            DeviceAccount newDeviceAccount = DeviceAccountFactory();

            long currentUnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            LoginResponse result = new()
            {
                accessToken = "ACCESS TOKEN PLACEHOLDER",
                behaviourSettings = "",
                capability = new()
                {
                    accountApiHost = "api.accounts.nintendo.com",
                    accountHost = "accounts.nintendo.com",
                    pointProgramHost = "my.nintendo.com",
                    sessionUpdateInterval = 180000,
                },
                createdDeviceAccount = newDeviceAccount,
                error = null,
                expiresIn = 900,
                idToken = "ID TOKEN PLACEHOLDER",
                market = null,
                sessionId = "",
                user = new()
                {
                    birthday = "0000-00-00",
                    country = "",
                    createdAt = currentUnixTime,
                    deviceAccounts = new()
                    {
                        newDeviceAccount,
                    },
                    gender =  "unknown",
                    hasUnreadCsComment =  false,
                    id =  "USER ID PLACEHOLDER",
                    links =  "",
                    nickname =  "",
                    permissions = new()
                    { 
                        personalAnalytics =  true,
                        personalAnalyticsUpdatedAt = currentUnixTime,
                        personalNotification =  true,
                        personalNotificationUpdatedAt = currentUnixTime,
                    },
                    updatedAt = currentUnixTime,
                },
            };

            return result;
        }

        public static DeviceAccount DeviceAccountFactory()
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
