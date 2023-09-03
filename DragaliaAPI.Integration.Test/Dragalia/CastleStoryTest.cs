using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class CastleStoryTest : TestFixture
{
    public CastleStoryTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_ResponseHasRewards()
    {
        CastleStoryReadData data = (
            await this.Client.PostMsgpack<CastleStoryReadData>(
                "/castle_story/read",
                new CastleStoryReadRequest() { castle_story_id = 1 }
            )
        ).data;

        data.castle_story_reward_list
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        entity_type = EntityTypes.Wyrmite,
                        entity_quantity = 50,
                        entity_id = 0
                    }
                }
            );

        data.update_data_list.user_data.Should().NotBeNull();
        data.update_data_list.castle_story_list
            .Should()
            .BeEquivalentTo(
                new List<CastleStoryList>()
                {
                    new() { castle_story_id = 1, is_read = 1, }
                }
            );
    }

    [Fact]
    public async Task ReadStory_StoryRead_ResponseHasNoRewards()
    {
        this.ApiContext.Add(
            new DbPlayerStoryState()
            {
                DeviceAccountId = DeviceAccountId,
                State = StoryState.Read,
                StoryId = 2,
                StoryType = StoryTypes.Castle
            }
        );
        await this.ApiContext.SaveChangesAsync();

        CastleStoryReadData data = (
            await this.Client.PostMsgpack<CastleStoryReadData>(
                "/castle_story/read",
                new CastleStoryReadRequest() { castle_story_id = 2 }
            )
        ).data;

        data.castle_story_reward_list.Should().BeEmpty();

        data.update_data_list.user_data.Should().BeNull();
        data.update_data_list.unit_story_list.Should().BeNull();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_UpdatesDatabase()
    {
        int oldCrystal = await this.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        CastleStoryReadData data = (
            await this.Client.PostMsgpack<CastleStoryReadData>(
                "/castle_story/read",
                new CastleStoryReadRequest() { castle_story_id = 3 }
            )
        ).data;

        int newCrystal = await this.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        newCrystal.Should().Be(oldCrystal + 50);

        IEnumerable<DbPlayerStoryState> stories = this.ApiContext.PlayerStoryState.Where(
            x => x.DeviceAccountId == DeviceAccountId
        );

        stories
            .Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = DeviceAccountId,
                    State = StoryState.Read,
                    StoryId = 3,
                    StoryType = StoryTypes.Castle
                }
            );
    }
}
