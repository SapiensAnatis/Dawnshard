using DragaliaAPI.Database.Entities;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Integration.Test.Other;

public class OutputCachingTest : TestFixture
{
    private readonly HttpClient httpClient;

    public OutputCachingTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.httpClient = this.CreateClient();
    }

    [Fact]
    public async Task RepeatedRequestPolicy_HandlesRepeatedUnsafeRequests()
    {
        this.MockTimeProvider.AdjustTime(DateTimeOffset.UtcNow);

        DbFortBuild build =
            new()
            {
                PlantId = FortPlants.WindAltar,
                BuildStartDate = DateTimeOffset.UtcNow.AddDays(-2),
                BuildEndDate = DateTimeOffset.UtcNow.AddDays(-1),
                Level = 5,
            };

        await this.AddToDatabase(build);

        this.httpClient.DefaultRequestHeaders.Add(Headers.RequestToken, "1234");

        DragaliaResponse<FortBuildEndResponse> response =
            await this.httpClient.PostMsgpack<FortBuildEndResponse>(
                "/fort/levelup_end",
                new FortBuildEndRequest(build.BuildId),
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);

        DragaliaResponse<FortBuildEndResponse> repeatedResponse =
            await this.httpClient.PostMsgpack<FortBuildEndResponse>(
                "/fort/levelup_end",
                new FortBuildEndRequest(build.BuildId),
                ensureSuccessHeader: false
            );

        // Ordinarily this request would fail because the building is no longer being upgraded.
        repeatedResponse.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }
}
