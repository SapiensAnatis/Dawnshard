using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Shared.Models;
using Xunit.Abstractions;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class GameJoinTest : TestFixture
{
    private const string Endpoint = "/Event/GameJoin";

    public GameJoinTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GameJoin_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        HttpResponseMessage response = await this.Client.PostAsync(Endpoint, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GameJoin_AddsPlayerToList()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "f162d896-f59e-416c-8df2-46a7649e1074",
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
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(game);

        HttpResponseMessage response = await this.Client.PostAsJsonAsync<GameModifyRequest>(
            Endpoint,
            new()
            {
                GameName = game.Name,
                Player = new()
                {
                    ViewerId = 22,
                    PartyNoList = new List<int>() { 1, 2 }
                }
            }
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        RedisGame? storedGame = await this.RedisConnectionProvider.GetGame(game.Name);
        storedGame.Should().NotBeNull();
        storedGame!.Players
            .Should()
            .BeEquivalentTo(
                new List<Player>()
                {
                    new()
                    {
                        ViewerId = 2,
                        PartyNoList = new List<int>() { 40 }
                    },
                    new()
                    {
                        ViewerId = 22,
                        PartyNoList = new List<int>() { 1, 2 }
                    }
                }
            );
    }
}
