using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Features.Web.Savefile;
using DragaliaAPI.Features.Web.Users;

namespace DragaliaAPI.Integration.Test.Features.Web.Settings;

public class SettingsTests : WebTestFixture
{
    public SettingsTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper)
    {
        this.SetupMockBaas();
    }

    [Fact]
    public async Task SetSettings_Unauthenticated_Returns401() =>
        (await this.Client.PutAsync("/api/settings", null, TestContext.Current.CancellationToken))
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task SetSettings_PersistsSettings()
    {
        this.AddTokenCookie();

        HttpResponseMessage resp = await this.Client.PutAsJsonAsync(
            "/api/settings",
            new PlayerSettings() { DailyGifts = false },
            TestContext.Current.CancellationToken
        );

        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        HttpResponseMessage getResp = await this.Client.GetAsync(
            "/api/user/me/profile",
            TestContext.Current.CancellationToken
        );

        getResp.Should().HaveStatusCode(HttpStatusCode.OK);

        UserProfile? profile = await getResp.Content.ReadFromJsonAsync<UserProfile>(
            cancellationToken: TestContext.Current.CancellationToken
        );

        profile.Should().NotBeNull();

        profile!.Settings.Should().BeEquivalentTo(new PlayerSettings() { DailyGifts = false });
    }
}
