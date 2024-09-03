using System.Net;

namespace DragaliaAPI.Integration.Test.Features.Web.Savefile;

public class SavefileExportTests : WebTestFixture
{
    public SavefileExportTests(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.SetupMockBaas();
    }

    [Fact]
    public async Task Export_Unauthenticated_Returns401() =>
        (await this.Client.GetAsync("/api/savefile/export"))
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task Export_ExportsSave()
    {
        this.AddTokenCookie();

        HttpResponseMessage resp = await this.Client.GetAsync("/api/savefile/export");
        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        string content = await resp.Content.ReadAsStringAsync();
        content.Should().StartWith("{");
    }
}
