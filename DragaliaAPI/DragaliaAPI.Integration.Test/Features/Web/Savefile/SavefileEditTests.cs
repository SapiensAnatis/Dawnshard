using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Features.Web.Savefile;

namespace DragaliaAPI.Integration.Test.Features.Web.Savefile;

public class SavefileEditTests : WebTestFixture
{
    public SavefileEditTests(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.SetupMockBaas();
    }

    [Fact]
    public async Task PresentWidgetData_Unauthenticated_Returns401() =>
        (await this.Client.GetAsync("/api/savefile/edit/widgets/present"))
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task PresentWidgetData_ReturnsData()
    {
        this.AddTokenCookie();

        HttpResponseMessage resp = await this.Client.GetAsync("/api/savefile/edit/widgets/present");
        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        PresentWidgetData? data = await resp.Content.ReadFromJsonAsync<PresentWidgetData>();

        data.Should().NotBeNull();
        data!
            .Types.Should()
            .ContainEquivalentOf(
                new EntityTypeInformation() { Type = EntityTypes.Chara, HasQuantity = false }
            );
        data.AvailableItems.Should()
            .ContainKey(EntityTypes.DmodePoint)
            .WhoseValue.Should()
            .BeEquivalentTo(
                [
                    new EntityTypeItem() { Id = (int)DmodePoint.Point1 },
                    new EntityTypeItem() { Id = (int)DmodePoint.Point2 },
                ]
            );
    }
}
