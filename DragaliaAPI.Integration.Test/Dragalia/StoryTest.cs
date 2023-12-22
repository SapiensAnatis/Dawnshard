using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

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
        StoryReadData data = (
            await this.Client.PostMsgpack<StoryReadData>(
                "/story/read",
                new StoryReadRequest() { unit_story_id = 100001141 }
            )
        ).data;

        data.unit_story_reward_list.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        entity_type = EntityTypes.Wyrmite,
                        entity_quantity = 25,
                        entity_id = 0
                    }
                }
            );

        data.update_data_list.user_data.Should().NotBeNull();
        data.update_data_list.unit_story_list.Should()
            .BeEquivalentTo(
                new List<UnitStoryList>()
                {
                    new() { unit_story_id = 100001141, is_read = 1, }
                }
            );
    }

    [Fact]
    public async Task ReadStory_StoryRead_ResponseHasNoRewards()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                State = StoryState.Read,
                StoryId = 100001121,
                StoryType = StoryTypes.Chara
            },
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                State = StoryState.Read,
                StoryId = 100001122,
                StoryType = StoryTypes.Chara
            }
        );
        await this.ApiContext.SaveChangesAsync();

        StoryReadData data = (
            await this.Client.PostMsgpack<StoryReadData>(
                "/story/read",
                new StoryReadRequest() { unit_story_id = 100001122 }
            )
        ).data;

        data.unit_story_reward_list.Should().BeEmpty();

        data.update_data_list.user_data.Should().BeNull();
        data.update_data_list.unit_story_list.Should().BeNull();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_UpdatesDatabase()
    {
        int oldCrystal = await this.ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync();

        StoryReadData data = (
            await this.Client.PostMsgpack<StoryReadData>(
                "/story/read",
                new StoryReadRequest() { unit_story_id = 100002011 }
            )
        ).data;

        int newCrystal = await this.ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Crystal)
            .SingleAsync();

        newCrystal.Should().Be(oldCrystal + 25);

        IEnumerable<DbPlayerStoryState> stories = this.ApiContext.PlayerStoryState.Where(
            x => x.ViewerId == ViewerId
        );

        stories
            .Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    State = StoryState.Read,
                    StoryId = 100002011,
                    StoryType = StoryTypes.Chara
                }
            );
    }
}
