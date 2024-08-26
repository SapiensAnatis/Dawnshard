using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Features.Web.Users;

namespace DragaliaAPI.Integration.Test.Features.Web.Users;

public class UserTests : WebTestFixture
{
    public UserTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task UserMe_NotAuthenticated_Returns401()
    {
        HttpResponseMessage resp = await this.Client.GetAsync("/api/user/me");

        resp.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserMe_AuthenticatedWithoutDawnshardIdentity_Returns404()
    {
        string token = TokenHelper.GetToken(
            "non existent device account id",
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );

        this.MockBaasApi.Setup(x => x.GetUserId(token)).ReturnsAsync((string?)null);

        this.Client.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");

        HttpResponseMessage resp = await this.Client.GetAsync("/api/user/me");

        resp.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserMe_Authenticated_Returns200()
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );

        this.Client.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");

        HttpResponseMessage resp = await this.Client.GetAsync("/api/user/me");

        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        (await resp.Content.ReadFromJsonAsync<User>())
            .Should()
            .BeEquivalentTo(new User() { Name = "Euden", ViewerId = this.ViewerId });
    }

    [Fact]
    public async Task UserMeProfile_Unauthenticated_Returns401()
    {
        HttpResponseMessage resp = await this.Client.GetAsync("/api/user/me/profile");

        resp.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserMeProfile_Authenticated_Returns()
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );

        this.Client.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");

        HttpResponseMessage resp = await this.Client.GetAsync("/api/user/me/profile");

        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        (await resp.Content.ReadFromJsonAsync<UserProfile>())
            .Should()
            .BeEquivalentTo(
                new UserProfile()
                {
                    LastLoginTime = DateTimeOffset.UnixEpoch,
                    LastSaveImportTime = DateTimeOffset.MinValue,
                }
            );
    }
}
