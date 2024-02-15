using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class GameCloseTest : TestFixture
{
    private const string Endpoint = "/Event/GameClose";

    public GameCloseTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GameClose_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        HttpResponseMessage response = await this.Client.PostAsync(Endpoint, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GameClose_RemovesGame()
    {
        RedisGame game =
            new()
            {
                RoomId = 12345,
                Name = "7e2ae7ce-50c6-47e3-90b4-817a048627ef",
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
            new() { GameName = game.Name, }
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await this.RedisConnectionProvider.GetGame(game.Name)).Should().BeNull();
    }
}
