using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class CastleStoryTest : TestFixture
{
    public CastleStoryTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ReadStory_StoryNotRead_ResponseHasRewards()
    {
        CastleStoryReadResponse data = (
            await this.Client.PostMsgpack<CastleStoryReadResponse>(
                "/castle_story/read",
                new CastleStoryReadRequest() { CastleStoryId = 1 },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.CastleStoryRewardList.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        EntityType = EntityTypes.Wyrmite,
                        EntityQuantity = 50,
                        EntityId = 0,
                    },
                }
            );

        data.UpdateDataList.UserData.Should().NotBeNull();
        data.UpdateDataList.CastleStoryList.Should()
            .BeEquivalentTo(
                new List<CastleStoryList>()
                {
                    new() { CastleStoryId = 1, IsRead = true },
                }
            );
    }

    [Fact]
    public async Task ReadStory_StoryRead_ResponseHasNoRewards()
    {
        this.ApiContext.Add(
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                State = StoryState.Read,
                StoryId = 2,
                StoryType = StoryTypes.Castle,
            }
        );
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        CastleStoryReadResponse data = (
            await this.Client.PostMsgpack<CastleStoryReadResponse>(
                "/castle_story/read",
                new CastleStoryReadRequest() { CastleStoryId = 2 },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.CastleStoryRewardList.Should().BeEmpty();

        data.UpdateDataList.UserData.Should().BeNull();
        data.UpdateDataList.UnitStoryList.Should().BeNull();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_UpdatesDatabase()
    {
        int oldCrystal = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        CastleStoryReadResponse data = (
            await this.Client.PostMsgpack<CastleStoryReadResponse>(
                "/castle_story/read",
                new CastleStoryReadRequest() { CastleStoryId = 3 },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        int newCrystal = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        newCrystal.Should().Be(oldCrystal + 50);

        IEnumerable<DbPlayerStoryState> stories = this.ApiContext.PlayerStoryState.Where(x =>
            x.ViewerId == ViewerId
        );

        stories
            .Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    State = StoryState.Read,
                    StoryId = 3,
                    StoryType = StoryTypes.Castle,
                }
            );
    }
}
