using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class EntryConditionsTest : TestFixture
{
    private const string Endpoint = "/Event/EntryConditions";

    public EntryConditionsTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task EntryConditions_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        HttpResponseMessage response = await this.Client.PostAsync(
            Endpoint,
            JsonContent.Create(new GameBase())
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task EntryConditions_UpdatesEntryConditions()
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
        EntryConditions newConditions =
            new()
            {
                UnacceptedElementTypeList = new List<int>() { 1 },
                UnacceptedWeaponTypeList = new List<int>() { 9 },
                RequiredPartyPower = 12000,
                ObjectiveTextId = 2,
            };

        await this.RedisConnectionProvider.RedisCollection<RedisGame>().InsertAsync(game);

        HttpResponseMessage response =
            await this.Client.PostAsJsonAsync<GameModifyConditionsRequest>(
                Endpoint,
                new() { GameName = game.Name, NewEntryConditions = newConditions }
            );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        RedisGame? storedGame = await this.RedisConnectionProvider.GetGame(game.Name);
        storedGame.Should().NotBeNull();
        storedGame!.EntryConditions.Should().BeEquivalentTo(newConditions);
    }
}
