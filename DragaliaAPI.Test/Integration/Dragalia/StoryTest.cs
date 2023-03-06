using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class StoryTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public StoryTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task ReadStory_StoryNotRead_ResponseHasRewards()
    {
        StoryReadData data = (
            await this.client.PostMsgpack<StoryReadData>(
                "/story/read",
                new StoryReadRequest() { unit_story_id = 100001141 }
            )
        ).data;

        data.unit_story_reward_list
            .Should()
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
        data.update_data_list.unit_story_list
            .Should()
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
        await this.fixture.AddToDatabase(
            new DbPlayerStoryState()
            {
                DeviceAccountId = this.fixture.DeviceAccountId,
                State = StoryState.Read,
                StoryId = 100001121,
                StoryType = StoryTypes.Chara
            },
            new DbPlayerStoryState()
            {
                DeviceAccountId = this.fixture.DeviceAccountId,
                State = StoryState.Read,
                StoryId = 100001122,
                StoryType = StoryTypes.Chara
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        StoryReadData data = (
            await this.client.PostMsgpack<StoryReadData>(
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
        int oldCrystal = await this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == this.fixture.DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        StoryReadData data = (
            await this.client.PostMsgpack<StoryReadData>(
                "/story/read",
                new StoryReadRequest() { unit_story_id = 100002011 }
            )
        ).data;

        int newCrystal = await this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == this.fixture.DeviceAccountId)
            .Select(x => x.Crystal)
            .SingleAsync();

        newCrystal.Should().Be(oldCrystal + 25);

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
                    StoryId = 100002011,
                    StoryType = StoryTypes.Chara
                }
            );
    }
}
