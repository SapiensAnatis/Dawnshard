using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Wall;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V21UpdateTest : SavefileUpdateTestFixture
{
    public V21UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V21Update_WallNotInitialized_DoesNotAddRewardDate()
    {
        await this.LoadIndex();

        this.ApiContext.WallRewardDates.Should().BeEmpty();
    }

    [Fact]
    public async Task V21Update_WallInitialized_DoesNotAddRewardDate()
    {
        await this.AddToDatabase(
            new DbPlayerQuestWall() { WallId = WallService.FlameWallId, WallLevel = 2 }
        );

        await this.LoadIndex();

        this.ApiContext.WallRewardDates.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new DbWallRewardDate()
                {
                    ViewerId = this.ViewerId,
                    LastClaimDate = DateTimeOffset.UnixEpoch,
                }
            );
    }

    [Fact]
    public async Task V21Update_WallInitialized_AlreadyHasRewardDate_DoesNotAddRewardDate()
    {
        await this.AddRangeToDatabase(
            [
                new DbPlayerQuestWall() { WallId = WallService.FlameWallId, WallLevel = 2 },
                new DbWallRewardDate() { LastClaimDate = DateTimeOffset.UnixEpoch.AddDays(1) }
            ]
        );

        await this.LoadIndex();

        this.ApiContext.WallRewardDates.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new DbWallRewardDate()
                {
                    ViewerId = this.ViewerId,
                    LastClaimDate = DateTimeOffset.UnixEpoch.AddDays(1),
                }
            );
    }
}
