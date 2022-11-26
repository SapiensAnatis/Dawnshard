using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.QuestController"/>
/// </summary>
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
                new QuestReadStoryRequest() { quest_story_id = 400 }
            )
        ).data;

        response.update_data_list.quest_story_list
            .Should()
            .ContainEquivalentOf(new QuestStoryList() { quest_story_id = 400, state = 1 });
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryData response = (
            await client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 700 }
            )
        ).data;

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        List<DbPlayerStoryState> storyStates = await apiContext.PlayerStoryState
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 700 && x.State == 1);
    }
}
