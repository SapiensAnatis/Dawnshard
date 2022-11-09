using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
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
        QuestReadStoryRequest request = new(400);
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/quest/read_story", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        QuestReadStoryResponse deserialized =
            MessagePackSerializer.Deserialize<QuestReadStoryResponse>(responseBytes);

        deserialized.data.update_data_list.quest_story_list
            .Should()
            .ContainEquivalentOf(new QuestStory(400, 1));
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryRequest request = new(700);
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/quest/read_story", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        List<DbPlayerStoryState> storyStates = await apiContext.PlayerStoryState
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 700 && x.State == 1);
    }
}
