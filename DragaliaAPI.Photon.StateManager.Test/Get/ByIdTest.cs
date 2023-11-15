using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.StateManager.Models;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test.Get;

public class ByIdTest : TestFixture
{
    private const string Endpoint = "/Get/ById";

    public ByIdTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ById_ReturnsGame()
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

        this.RedisConnectionProvider.RedisCollection<RedisGame>().Insert(game);

        HttpResponseMessage response = await this.Client.GetAsync($"{Endpoint}/12345");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<ApiGame>())
            .Should()
            .BeEquivalentTo(new ApiGame(game));
    }

    [Fact]
    public async Task ById_NotFound_Returns404()
    {
        HttpResponseMessage response = await this.Client.GetAsync($"{Endpoint}/56789");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
