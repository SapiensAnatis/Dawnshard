using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Other;

public class OutputCachingTest : TestFixture
{
    protected OutputCachingTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task RepeatedRequestPolicy_HandlesRepeatedUnsafeRequests()
    {
        DbFortBuild fortBuild =
            new()
            {
                PlantId = FortPlants.WindAltar,
                BuildStartDate = DateTimeOffset.Now.AddDays(-2),
                BuildEndDate = DateTimeOffset.Now.AddDays(-1),
                Level = 5,
            };

        await this.AddToDatabase(fortBuild);
    }
}
