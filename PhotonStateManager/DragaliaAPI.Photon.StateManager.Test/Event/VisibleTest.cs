using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using DragaliaAPI.Photon.StateManager.Models;
using DragaliaAPI.Photon.StateManager.Test.Helpers;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Event;

public class VisibleTest : TestFixture
{
    private const string Endpoint = "/Event/Visible";

    public VisibleTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public async Task Visible_SetsGameVisible(bool visibility)
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

        HttpResponseMessage response = await this.Client.PostAsJsonAsync<GameModifyVisibleRequest>(
            Endpoint,
            new() { GameName = game.Name, NewVisibility = visibility }
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        RedisGame? storedGame = await this.RedisConnectionProvider.GetGame(game.Name);
        storedGame.Should().NotBeNull();
        storedGame!.Visible.Should().Be(visibility);
    }
}
