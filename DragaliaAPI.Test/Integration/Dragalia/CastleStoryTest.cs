using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class CastleStoryTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public CastleStoryTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        TestUtils.IgnoreNavigationAssertions();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_ResponseHasRewards()
    {
        CastleStoryReadData data = (
            await this.client.PostMsgpack<CastleStoryReadData>(
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
        this.fixture.ApiContext.Add(
            new DbPlayerStoryState()
            {
                DeviceAccountId = this.fixture.DeviceAccountId,
                State = StoryState.Read,
                StoryId = 2,
                StoryType = StoryTypes.Castle
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        CastleStoryReadData data = (
            await this.client.PostMsgpack<CastleStoryReadData>(
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
        int oldCrystal = await this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == this.fixture.DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        CastleStoryReadData data = (
            await this.client.PostMsgpack<CastleStoryReadData>(
                "/castle_story/read",
                new CastleStoryReadRequest() { castle_story_id = 3 }
            )
        ).data;

        int newCrystal = await this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == this.fixture.DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        newCrystal.Should().Be(oldCrystal + 50);

        IEnumerable<DbPlayerStoryState> stories = this.fixture.ApiContext.PlayerStoryState.Where(
            x => x.DeviceAccountId == this.fixture.DeviceAccountId
        );

        stories
            .Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = this.fixture.DeviceAccountId,
                    State = StoryState.Read,
                    StoryId = 3,
                    StoryType = StoryTypes.Castle
                }
            );
    }
}
