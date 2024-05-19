using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Story;

public class StoryTest : TestFixture
{
    public StoryTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_ResponseHasRewards()
    {
        StoryReadResponse data = (
            await this.Client.PostMsgpack<StoryReadResponse>(
                "/story/read",
                new StoryReadRequest() { UnitStoryId = 100001141 }
            )
        ).Data;

        data.UnitStoryRewardList.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        EntityType = EntityTypes.Wyrmite,
                        EntityQuantity = 25,
                        EntityId = 0
                    }
                }
            );

        data.UpdateDataList.UserData.Should().NotBeNull();
        data.UpdateDataList.UnitStoryList.Should()
            .BeEquivalentTo(
                new List<UnitStoryList>()
                {
                    new() { UnitStoryId = 100001141, IsRead = true, }
                }
            );
    }

    [Fact]
    public async Task ReadStory_StoryRead_ResponseHasNoRewards()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = this.ViewerId,
                State = StoryState.Read,
                StoryId = 100001121,
                StoryType = StoryTypes.Chara
            },
            new DbPlayerStoryState()
            {
                ViewerId = this.ViewerId,
                State = StoryState.Read,
                StoryId = 100001122,
                StoryType = StoryTypes.Chara
            }
        );
        await this.ApiContext.SaveChangesAsync();

        StoryReadResponse data = (
            await this.Client.PostMsgpack<StoryReadResponse>(
                "/story/read",
                new StoryReadRequest() { UnitStoryId = 100001122 }
            )
        ).Data;

        data.UnitStoryRewardList.Should().BeEmpty();

        data.UpdateDataList.UserData.Should().BeNull();
        data.UpdateDataList.UnitStoryList.Should().BeNull();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_UpdatesDatabase()
    {
        int oldCrystal = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync();

        StoryReadResponse data = (
            await this.Client.PostMsgpack<StoryReadResponse>(
                "/story/read",
                new StoryReadRequest() { UnitStoryId = 100002011 }
            )
        ).Data;

        int newCrystal = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync();

        newCrystal.Should().Be(oldCrystal + 25);

        IEnumerable<DbPlayerStoryState> stories = this.ApiContext.PlayerStoryState.Where(x =>
            x.ViewerId == this.ViewerId
        );

        stories
            .Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    State = StoryState.Read,
                    StoryId = 100002011,
                    StoryType = StoryTypes.Chara
                }
            );
    }
}
