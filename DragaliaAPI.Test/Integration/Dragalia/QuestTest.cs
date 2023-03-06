using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.QuestController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class QuestTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public QuestTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task ReadStory_ReturnCorrectResponse()
    {
        QuestReadStoryData response = (
            await client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1000106 }
            )
        ).data;

        response.update_data_list.user_data.Should().NotBeNull();
        response.update_data_list.chara_list
            .Should()
            .ContainSingle()
            .And.Subject.Any(x => x.chara_id == Charas.Ranzal)
            .Should()
            .BeTrue();

        response.update_data_list.quest_story_list
            .Should()
            .ContainEquivalentOf(new QuestStoryList() { quest_story_id = 1000106, state = 1 });
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryData response = (
            await client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1001410 }
            )
        ).data;

        fixture.ApiContext.PlayerStoryState
            .First(x => x.DeviceAccountId == fixture.DeviceAccountId && x.StoryId == 1001410)
            .State.Should()
            .Be(StoryState.Read);

        fixture.ApiContext.PlayerCharaData.Any(x => x.CharaId == Charas.Zena).Should().BeTrue();
    }
}
