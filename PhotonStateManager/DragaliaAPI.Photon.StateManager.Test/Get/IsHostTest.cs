using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.StateManager.Models;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Get;

public class IsHostTest : TestFixture
{
    private const string Endpoint = "/Get/IsHost";

    public IsHostTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task IsHost_HostFound_ReturnsTrue()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "151de85a-200a-4952-8757-e0f868bbf28b",
                EntryConditions = new(),
                Players =
                [
                    new()
                    {
                        ViewerId = 2,
                        ActorNr = 1,
                        PartyNoList = [40]
                    },
                    new()
                    {
                        ViewerId = 3,
                        ActorNr = 2,
                        PartyNoList = [40]
                    }
                ]
            };

        this.RedisConnectionProvider.RedisCollection<RedisGame>().Insert(game);

        HttpResponseMessage response = await this.Client.GetAsync($"{Endpoint}/2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<bool>()).Should().Be(true);
    }

    [Fact]
    public async Task IsHost_IngameButNotHost_ReturnsFalse()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "151de85a-200a-4952-8757-e0f868bbf28b",
                EntryConditions = new(),
                Players =
                [
                    new()
                    {
                        ViewerId = 2,
                        ActorNr = 1,
                        PartyNoList = [40]
                    },
                    new()
                    {
                        ViewerId = 3,
                        ActorNr = 2,
                        PartyNoList = [40]
                    }
                ]
            };

        this.RedisConnectionProvider.RedisCollection<RedisGame>().Insert(game);

        HttpResponseMessage response = await this.Client.GetAsync($"{Endpoint}/3");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<bool>()).Should().Be(false);
    }

    [Fact]
    public async Task IsHost_NotInGame_ReturnsFalse()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "151de85a-200a-4952-8757-e0f868bbf28b",
                MatchingCompatibleId = 36,
                MatchingType = MatchingTypes.Anyone,
                QuestId = 301010103,
                StartEntryTime = DateTimeOffset.UtcNow,
                EntryConditions = new() { },
                Players =
                [
                    new()
                    {
                        ViewerId = 2,
                        ActorNr = 1,
                        PartyNoList = [40]
                    }
                ]
            };

        this.RedisConnectionProvider.RedisCollection<RedisGame>().Insert(game);

        HttpResponseMessage response = await this.Client.GetAsync($"{Endpoint}/8888");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<bool>()).Should().Be(false);
    }
}
