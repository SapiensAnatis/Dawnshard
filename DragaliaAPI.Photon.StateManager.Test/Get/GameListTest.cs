using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.StateManager.Models;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Get;

public class GameListTest : TestFixture
{
    private const string Endpoint = "/Get/GameList";

    public GameListTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GameList_GetsVisibleAndOpenGames()
    {
        List<RedisGame> games =
            new()
            {
                // Should be returned
                new()
                {
                    RoomId = 12345,
                    Name = "639d263a-e05a-4432-952f-fa941e7f7f40",
                    MatchingCompatibleId = 36,
                    MatchingType = MatchingTypes.Anyone,
                    QuestId = 301010103,
                    StartEntryTime = DateTimeOffset.UtcNow,
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                        RequiredPartyPower = 11700,
                        ObjectiveTextId = 1,
                    },
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ViewerId = 2,
                            PartyNoList = new List<int>() { 40 }
                        }
                    }
                },
                // Non-public matching
                new()
                {
                    RoomId = 12345,
                    Name = "0023679c-c449-45bd-881d-3e29a9d982ac",
                    MatchingCompatibleId = 36,
                    MatchingType = MatchingTypes.ById,
                    QuestId = 301010103,
                    StartEntryTime = DateTimeOffset.UtcNow,
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                        RequiredPartyPower = 11700,
                        ObjectiveTextId = 1,
                    },
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ViewerId = 3,
                            PartyNoList = new List<int>() { 40 }
                        }
                    }
                },
                // Visible = false

                new()
                {
                    RoomId = 12345,
                    Name = "04e2c5dd-055c-4c3b-85d0-29b4ea90b03e",
                    MatchingCompatibleId = 36,
                    MatchingType = MatchingTypes.Anyone,
                    QuestId = 301010103,
                    StartEntryTime = DateTimeOffset.UtcNow,
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                        RequiredPartyPower = 11700,
                        ObjectiveTextId = 1,
                    },
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ViewerId = 3,
                            PartyNoList = new List<int>() { 40 }
                        }
                    },
                    Visible = false,
                },
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(games);

        HttpResponseMessage response = await this.Client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<IEnumerable<ApiGame>>())
            .Should()
            .HaveCount(1)
            .And.BeEquivalentTo(new List<ApiGame>() { new(games[0]) });
    }

    [Fact]
    public async Task GameList_QuestIdSpecified_FiltersByQuest()
    {
        List<RedisGame> games =
            new()
            {
                new()
                {
                    RoomId = 12345,
                    Name = "d4592641-6f07-46d9-8b45-21059b27d1e2",
                    MatchingCompatibleId = 36,
                    MatchingType = MatchingTypes.Anyone,
                    QuestId = 301010103,
                    StartEntryTime = DateTimeOffset.UtcNow,
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                        RequiredPartyPower = 11700,
                        ObjectiveTextId = 1,
                    },
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ViewerId = 2,
                            PartyNoList = new List<int>() { 40 }
                        }
                    }
                },
                new()
                {
                    RoomId = 12345,
                    Name = "c7c494cc-0ffc-46d6-a19b-cbfcad903225",
                    MatchingCompatibleId = 36,
                    MatchingType = MatchingTypes.Anyone,
                    QuestId = 219010102,
                    StartEntryTime = DateTimeOffset.UtcNow,
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                        RequiredPartyPower = 11700,
                        ObjectiveTextId = 1,
                    },
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ViewerId = 3,
                            PartyNoList = new List<int>() { 40 }
                        }
                    }
                },
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(games);

        HttpResponseMessage response = await this.Client.GetAsync(
            $"{Endpoint}?questId={219010102}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<IEnumerable<ApiGame>>())
            .Should()
            .HaveCount(1)
            .And.BeEquivalentTo(new List<ApiGame>() { new(games[1]) });
    }
}
