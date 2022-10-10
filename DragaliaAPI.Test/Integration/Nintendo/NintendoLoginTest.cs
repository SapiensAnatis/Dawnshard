using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Nintendo;

public class NintendoLoginTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public NintendoLoginTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task PostLogin_NullDeviceAccount_ReturnsSuccessResponseAndCreatesDeviceAccount()
    {
        StringContent requestContent =
            new(
                """
            {
            }
            """
            );
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpResponseMessage response = await _client.PostAsync(
            "/core/v1/gateway/sdk/login",
            requestContent
        );

        response.IsSuccessStatusCode.Should().BeTrue();

        string jsonString = await response.Content.ReadAsStringAsync();
        PartialLoginResponse? deserializedResponse =
            JsonSerializer.Deserialize<PartialLoginResponse>(jsonString);
        deserializedResponse.Should().NotBeNull();
        DeviceAccount createdDeviceAccount = deserializedResponse!.createdDeviceAccount;

        // Ensure new DeviceAccount was registered against the DB, and will authenticate successfully
        using IServiceScope scope = _factory.Services.CreateScope();
        IDeviceAccountService deviceAccountService =
            scope.ServiceProvider.GetRequiredService<IDeviceAccountService>();
        (await deviceAccountService.AuthenticateDeviceAccount(createdDeviceAccount))
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task PostLogin_DeviceAccountCorrectCredentials_ReturnsSuccessResponse()
    {
        DeviceAccount deviceAccount = new("id", "password");
        StringContent requestContent =
            new(
                """
            {
                "deviceAccount": {
                    "id": "id",
                    "password": "password"
                }
            }
            """
            );
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpResponseMessage response = await _client.PostAsync(
            "/core/v1/gateway/sdk/login",
            requestContent
        );

        response.IsSuccessStatusCode.Should().BeTrue();

        string jsonString = await response.Content.ReadAsStringAsync();
        PartialLoginResponse? deserializedResponse =
            JsonSerializer.Deserialize<PartialLoginResponse>(jsonString);
        deserializedResponse.Should().NotBeNull();

        deserializedResponse!.user.deviceAccounts.Should().Contain(deviceAccount);
    }

    [Fact]
    public async Task PostLogin_DeviceAccountIncorrectCredentials_ReturnsUnauthorizedResponse()
    {
        StringContent requestContent =
            new(
                """
            {
                "deviceAccount": {
                    "id": "id",
                    "password": "wrong password"
                }
            }
            """
            );
        requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpResponseMessage response = await _client.PostAsync(
            "/core/v1/gateway/sdk/login",
            requestContent
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Only deserialize the fields of interest for testing -- the existing LoginResponse record contains a lot of useless data
    // and the deserializer doesn't really seem to like it for reasons that aren't worth figuring out
    public record PartialLoginResponse
    {
        public DeviceAccount createdDeviceAccount { get; init; }
        public PartialUser user { get; init; }

        [JsonConstructor]
        public PartialLoginResponse(DeviceAccount createdDeviceAccount, PartialUser user)
        {
            this.createdDeviceAccount = createdDeviceAccount;
            this.user = user;
        }

        public record PartialUser
        {
            public List<DeviceAccount> deviceAccounts { get; init; }

            [JsonConstructor]
            public PartialUser(List<DeviceAccount> deviceAccounts)
            {
                this.deviceAccounts = deviceAccounts;
            }
        }
    }
}
