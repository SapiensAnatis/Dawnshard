using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DragaliaAPI.Test.Integration
{
    public class NintendoLoginTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly DeviceAccountContext _deviceAccountContext;

        public NintendoLoginTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            
        }

        [Fact]
        public async Task PostLogin_NullDeviceAccount_ReturnsSuccessResponseAndCreatesDeviceAccount()
        {
            StringContent requestContent = new("""
            {
                "appVersion": "2.19.0",
                "assertion": "eyJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJjb20ubmludGVuZG8uemFnYTowYzNkNzg5ZjVlZDIzZjJiMzRjNzk2NjBhMzcxOTBkMWM4NzNhM2YyIiwiaWF0IjoxNjYyODIwODQ4LCJhdWQiOiJodHRwczpcL1wvNDhjYzgxY2RiOGRlMzBlMDYxOTI4ZjU2ZTliZDRiNGQuYmFhcy5uaW50ZW5kby5jb20ifQ==.brVDUSGZpnYugei0UHmTt_YEA3P6WPqr0id29TM8SM4=",
                "carrier": "giffgaff",
                "deviceAnalyticsId": "a2J1YmFhYWFERG1NamZtckpNTmVqSHZ6UGJWUE9FUwA=",
                "deviceName": "ONEPLUS A6003",
                "locale": "en-GB",
                "manufacturer": "OnePlus",
                "networkType": "wifi",
                "osType": "Android",
                "osVersion": "11",
                "sdkVersion": "Unity-2.33.0-0a4be7c8",
                "timeZone": "Europe/London",
                "timeZoneOffset": 3600000
            }
            """);
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = await _client.PostAsync("/core/v1/gateway/sdk/login", requestContent);
            
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            LoginResponse? deserializedResponse = JsonSerializer.Deserialize<LoginResponse>(jsonString);

            deserializedResponse.Should().NotBeNull();
            deserializedResponse!.createdDeviceAccount.Should().NotBeNull();

            using (var scope = _factory.Services.CreateScope())
            {
                var _deviceAccountContext = scope.ServiceProvider.GetRequiredService<DeviceAccountContext>();
                (await _deviceAccountContext.AuthenticateDeviceAccount(deserializedResponse!.createdDeviceAccount!)).Should().BeTrue();
            }
        }

        [Fact]
        public async Task PostLogin_DeviceAccountCorrectCredentials_ReturnsSuccessResponse()
        {
            DeviceAccount deviceAccount = new("584cbbad-0f20-4891-997e-1836435fc610", "password");
            StringContent requestContent = new("""
            {
                "appVersion": "2.19.0",
                "assertion": "eyJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJjb20ubmludGVuZG8uemFnYTowYzNkNzg5ZjVlZDIzZjJiMzRjNzk2NjBhMzcxOTBkMWM4NzNhM2YyIiwiaWF0IjoxNjYyODIwODQ4LCJhdWQiOiJodHRwczpcL1wvNDhjYzgxY2RiOGRlMzBlMDYxOTI4ZjU2ZTliZDRiNGQuYmFhcy5uaW50ZW5kby5jb20ifQ==.brVDUSGZpnYugei0UHmTt_YEA3P6WPqr0id29TM8SM4=",
                "carrier": "giffgaff",
                "deviceAnalyticsId": "a2J1YmFhYWFERG1NamZtckpNTmVqSHZ6UGJWUE9FUwA=",
                "deviceName": "ONEPLUS A6003",
                "locale": "en-GB",
                "manufacturer": "OnePlus",
                "networkType": "wifi",
                "osType": "Android",
                "osVersion": "11",
                "sdkVersion": "Unity-2.33.0-0a4be7c8",
                "timeZone": "Europe/London",
                "timeZoneOffset": 3600000,
                "deviceAccount": {
                    "id": "584cbbad-0f20-4891-997e-1836435fc610",
                    "password": "password",
                }
            }
            """);
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = await _client.PostAsync("/core/v1/gateway/sdk/login", requestContent);

            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            LoginResponse? deserializedResponse = JsonSerializer.Deserialize<LoginResponse>(jsonString);

            deserializedResponse.Should().NotBeNull();
            deserializedResponse!.user.deviceAccounts.Should().Contain(deviceAccount);
        }

        [Fact]
        public async Task PostLogin_DeviceAccountIncorrectCredentials_ReturnsUnauthorizedResponse()
        {
            StringContent requestContent = new("""
            {
                "appVersion": "2.19.0",
                "assertion": "eyJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJjb20ubmludGVuZG8uemFnYTowYzNkNzg5ZjVlZDIzZjJiMzRjNzk2NjBhMzcxOTBkMWM4NzNhM2YyIiwiaWF0IjoxNjYyODIwODQ4LCJhdWQiOiJodHRwczpcL1wvNDhjYzgxY2RiOGRlMzBlMDYxOTI4ZjU2ZTliZDRiNGQuYmFhcy5uaW50ZW5kby5jb20ifQ==.brVDUSGZpnYugei0UHmTt_YEA3P6WPqr0id29TM8SM4=",
                "carrier": "giffgaff",
                "deviceAnalyticsId": "a2J1YmFhYWFERG1NamZtckpNTmVqSHZ6UGJWUE9FUwA=",
                "deviceName": "ONEPLUS A6003",
                "locale": "en-GB",
                "manufacturer": "OnePlus",
                "networkType": "wifi",
                "osType": "Android",
                "osVersion": "11",
                "sdkVersion": "Unity-2.33.0-0a4be7c8",
                "timeZone": "Europe/London",
                "timeZoneOffset": 3600000,
                "deviceAccount": {
                    "id": "584cbbad-0f20-4891-997e-1836435fc610",
                    "password": "wrong password",
                }
            }
            """);
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = await _client.PostAsync("/core/v1/gateway/sdk/login", requestContent);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
