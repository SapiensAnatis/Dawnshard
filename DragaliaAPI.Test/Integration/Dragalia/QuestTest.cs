using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class QuestTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public QuestTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        _factory.SeedCache();
    }

    [Fact]
    public async Task ReadStory_ReturnCorrectResponse()
    {
        QuestReadStoryRequest request = new(400);
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/quest/read_story", content);

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

        HttpResponseMessage response = await _client.PostAsync("/quest/read_story", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        using IServiceScope scope = _factory.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        List<DbPlayerStoryState> storyStates = await apiContext.PlayerStoryState
            .Where(x => x.DeviceAccountId == _factory.DeviceAccountId)
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 700 && x.State == 1);
    }
}
