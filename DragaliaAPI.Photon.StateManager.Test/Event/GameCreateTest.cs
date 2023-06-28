using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class GameCreateTest : TestFixture
{
    private const string Endpoint = "/Event/GameCreate";

    public GameCreateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GameCreate_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        HttpResponseMessage response = await this.Client.PostAsync(
            Endpoint,
            JsonContent.Create(new GameBase())
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GameCreate_AddsHostAndStoresInRedis()
    {
        GameBase game =
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
                Players = new List<Player>() { }
            };

        RedisGame expectedGame =
            new()
            {
                RoomId = 12345,
                Name = "639d263a-e05a-4432-952f-fa941e7f7f40",
                MatchingCompatibleId = 36,
                MatchingType = MatchingTypes.Anyone,
                QuestId = 301010103,
                StartEntryTime = game.StartEntryTime,
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
            };

        HttpResponseMessage response = await this.Client.PostAsJsonAsync<GameCreateRequest>(
            Endpoint,
            new()
            {
                Game = game,
                Player = new()
                {
                    ViewerId = 2,
                    PartyNoList = new List<int>() { 40 }
                }
            }
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await this.RedisConnectionProvider.GetGame(game.Name))
            .Should()
            .BeEquivalentTo(expectedGame);
    }
}
