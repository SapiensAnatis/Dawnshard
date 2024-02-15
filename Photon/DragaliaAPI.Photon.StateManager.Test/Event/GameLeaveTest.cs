using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class GameLeaveTest : TestFixture
{
    private const string Endpoint = "/Event/GameLeave";

    public GameLeaveTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GameLeave_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        HttpResponseMessage response = await this.Client.PostAsync(Endpoint, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GameLeave_RemovesPlayerFromGame()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "affa751f-b3ce-4dd7-9b07-bbeaa1783acc",
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
                    },
                    new()
                    {
                        ViewerId = 5,
                        PartyNoList = new List<int>() { 20 }
                    }
                }
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(game);

        await this.Client.PostAsJsonAsync<GameModifyRequest>(
            Endpoint,
            new()
            {
                GameName = game.Name,
                Player = new() { ViewerId = 5 }
            }
        );

        RedisGame? storedGame = await this.RedisConnectionProvider.GetGame(game.Name);
        storedGame.Should().NotBeNull();
        storedGame!
            .Players.Should()
            .BeEquivalentTo(
                new List<Player>()
                {
                    new()
                    {
                        ViewerId = 2,
                        PartyNoList = new List<int>() { 40 }
                    }
                }
            );
        storedGame!.MatchingType.Should().Be(MatchingTypes.Anyone);
    }

    [Fact]
    public async Task GameLeave_LastPlayer_RemovesPlayerFromGame_SetsVisibleFalse()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "5ff0c20c-b1b6-4377-81d4-a201038faf01",
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
                        ViewerId = 5,
                        PartyNoList = new List<int>() { 20 }
                    }
                }
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(game);

        await this.Client.PostAsJsonAsync<GameModifyRequest>(
            Endpoint,
            new()
            {
                GameName = game.Name,
                Player = new() { ViewerId = 5 }
            }
        );

        RedisGame? storedGame = await this.RedisConnectionProvider.GetGame(game.Name);
        storedGame.Should().NotBeNull();
        storedGame!.Players.Should().BeEmpty();
        storedGame!.Visible.Should().BeFalse();
    }
}
