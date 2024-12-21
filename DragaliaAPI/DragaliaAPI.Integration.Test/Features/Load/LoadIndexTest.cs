using DragaliaAPI.Infrastructure.Results;

namespace DragaliaAPI.Integration.Test.Features.Load;

public class LoadIndexTest : TestFixture
{
    public LoadIndexTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task LoadIndex_ReturnsPartyUnitsInSortedOrder()
    {
        DragaliaResponse<LoadIndexResponse> resp = await this.Client.PostMsgpack<LoadIndexResponse>(
            "/load/index",
            cancellationToken: TestContext.Current.CancellationToken
        );

        resp.Data.PartyList.Should()
            .AllSatisfy(x => x.PartySettingList.Should().BeInAscendingOrder(y => y.UnitNo));
    }
}
