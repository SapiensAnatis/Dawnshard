using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Nintendo;

[Obsolete(ObsoleteReasons.BaaS)]
[Collection("DragaliaIntegration")]
public class NintendoLoginTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public NintendoLoginTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );

        fixture.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = false });
    }

    [Fact]
    public async Task PostLogin_NullDeviceAccount_ReturnsSuccessResponseAndCreatesDeviceAccount()
    {
        HttpResponseMessage response = await client.PostAsync(
            "/core/v1/gateway/sdk/login",
            JsonContent.Create(new object())
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string jsonString = await response.Content.ReadAsStringAsync();
        PartialLoginResponse? deserializedResponse =
            JsonSerializer.Deserialize<PartialLoginResponse>(jsonString);
        deserializedResponse.Should().NotBeNull();
        DeviceAccount createdDeviceAccount = deserializedResponse!.CreatedDeviceAccount;

        // Ensure new DeviceAccount was registered against the DB, and will authenticate successfully
        using IServiceScope scope = fixture.Services.CreateScope();
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

        HttpResponseMessage response = await client.PostAsync(
            "/core/v1/gateway/sdk/login",
            JsonContent.Create(new LoginRequest(deviceAccount))
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string jsonString = await response.Content.ReadAsStringAsync();
        PartialLoginResponse? deserializedResponse =
            JsonSerializer.Deserialize<PartialLoginResponse>(jsonString);

        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.User.DeviceAccounts.Should().Contain(deviceAccount);
    }

    [Fact]
    public async Task PostLogin_ForeignDeviceAccount_CreatesAndReturnsSuccessResponse()
    {
        DeviceAccount deviceAccount = new("foreign id", "password");

        HttpResponseMessage response = await client.PostAsync(
            "/core/v1/gateway/sdk/login",
            JsonContent.Create(new LoginRequest(deviceAccount))
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string jsonString = await response.Content.ReadAsStringAsync();
        PartialLoginResponse? deserializedResponse =
            JsonSerializer.Deserialize<PartialLoginResponse>(jsonString);

        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.User.DeviceAccounts.Should().Contain(deviceAccount);
    }

    [Fact]
    public async Task PostLogin_DeviceAccountIncorrectCredentials_ReturnsUnauthorizedResponse()
    {
        HttpResponseMessage response = await client.PostAsync(
            "/core/v1/gateway/sdk/login",
            JsonContent.Create(new LoginRequest(new DeviceAccount("id", "wrong password")))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Only deserialize the fields of interest for testing -- the existing LoginResponse record contains a lot of useless data
    // and the deserializer doesn't really seem to like it for reasons that aren't worth figuring out
    public record PartialLoginResponse
    {
        [JsonPropertyName("createdDeviceAccount")]
        public DeviceAccount CreatedDeviceAccount { get; init; }

        [JsonPropertyName("user")]
        public PartialUser User { get; init; }

        [JsonConstructor]
        public PartialLoginResponse(DeviceAccount createdDeviceAccount, PartialUser user)
        {
            this.CreatedDeviceAccount = createdDeviceAccount;
            this.User = user;
        }

        public record PartialUser
        {
            [JsonPropertyName("deviceAccounts")]
            public List<DeviceAccount> DeviceAccounts { get; init; }

            [JsonConstructor]
            public PartialUser(List<DeviceAccount> deviceAccounts)
            {
                this.DeviceAccounts = deviceAccounts;
            }
        }
    }
}
