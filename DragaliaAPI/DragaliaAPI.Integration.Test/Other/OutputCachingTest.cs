using DragaliaAPI.Database.Entities;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Integration.Test.Other;

public class OutputCachingTest : TestFixture
{
    public OutputCachingTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task RepeatedRequestPolicy_HandlesRepeatedUnsafeRequests()
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        DbFortBuild build = new()
        {
            PlantId = FortPlants.WindAltar,
            BuildStartDate = DateTimeOffset.UtcNow.AddDays(-2),
            BuildEndDate = DateTimeOffset.UtcNow.AddDays(-1),
            Level = 5,
        };

        await this.AddToDatabase(build);

        this.Client.DefaultRequestHeaders.Add(Headers.RequestToken, "1234");

        DragaliaResponse<FortBuildEndResponse> response =
            await this.Client.PostMsgpack<FortBuildEndResponse>(
                "/fort/levelup_end",
                new FortBuildEndRequest(build.BuildId),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);

        DragaliaResponse<FortBuildEndResponse> repeatedResponse =
            await this.Client.PostMsgpack<FortBuildEndResponse>(
                "/fort/levelup_end",
                new FortBuildEndRequest(build.BuildId),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            );

        // Ordinarily this request would fail because the building is no longer being upgraded.
        repeatedResponse.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }
}
