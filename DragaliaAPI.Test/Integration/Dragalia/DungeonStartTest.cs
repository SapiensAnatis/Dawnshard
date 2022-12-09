using System.Text.Json;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.DungeonStartController"/>.
/// </summary>
[Collection("DragaliaIntegration")]
public class DungeonStartTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;
    private readonly ITestOutputHelper output;

    public DungeonStartTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task EnterNewQuest_UsesExpectedParty()
    {
        DungeonStartStartData response = (
            await this.client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 1 },
                    quest_id = 100010103
                }
            )
        ).data;

        // TODO: add some more weapons to the default savefile to make a more interesting party to assert against#
        // Maybe once we do savefile import we can set the test fixture to have an endgame savefile

        IEnumerable<object> storedPartyData = (
            await this.fixture.ApiContext.PlayerParties.SingleAsync(
                x => x.DeviceAccountId == fixture.DeviceAccountId && x.PartyNo == 1
            )
        ).Units
            .Where(x => x.CharaId != 0)
            .Select(
                x =>
                    new
                    {
                        CharaId = x.CharaId,
                        WeaponBodyId = x.EquipWeaponBodyId,
                        DragonKeyId = x.EquipDragonKeyId
                    }
            );

        response.ingame_data.party_info.party_unit_list
            .Select(
                x =>
                    new
                    {
                        CharaId = x.chara_data?.chara_id ?? Charas.Empty,
                        WeaponBodyId = x.weapon_body_data?.weapon_body_id ?? WeaponBodies.Empty,
                        DragonKeyId = x.dragon_data?.dragon_key_id ?? 0L
                    }
            )
            .Should()
            .BeEquivalentTo(storedPartyData);
    }
}
