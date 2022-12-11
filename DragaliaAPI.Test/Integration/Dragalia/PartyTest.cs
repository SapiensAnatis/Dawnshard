using System.Net;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.PartyController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class PartyTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public PartyTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task SetPartySetting_ValidRequest_UpdatesDatabase()
    {
        await fixture.AddCharacter(Charas.Ilia);

        await client.PostMsgpack<PartySetPartySettingData>(
            "/party/set_party_setting",
            new PartySetPartySettingRequest(
                1,
                new List<PartySettingList>()
                {
                    new()
                    {
                        unit_no = 1,
                        chara_id = Charas.Ilia,
                        equip_weapon_body_id = WeaponBodies.DivineTrigger,
                        equip_crest_slot_type_1_crest_id_1 = AbilityCrests.ADragonyuleforIlia
                    }
                },
                "My New Party",
                0,
                0
            )
        );

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbParty dbparty = await apiContext.PlayerParties
            .Include(x => x.Units)
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId && x.PartyNo == 1)
            .SingleAsync();

        dbparty
            .Should()
            .BeEquivalentTo(
                new DbParty()
                {
                    DeviceAccountId = fixture.DeviceAccountId,
                    PartyNo = 1,
                    PartyName = "My New Party",
                },
                opts => opts.Excluding(x => x.Units)
            );

        dbparty.Units
            .Should()
            .BeEquivalentTo(
                new List<DbPartyUnit>()
                {
                    new()
                    {
                        UnitNo = 1,
                        PartyNo = 1,
                        DeviceAccountId = fixture.DeviceAccountId,
                        CharaId = Charas.Ilia,
                        EquipCrestSlotType1CrestId1 = AbilityCrests.ADragonyuleforIlia,
                        EquipWeaponBodyId = WeaponBodies.DivineTrigger,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 2,
                        PartyNo = 1,
                        DeviceAccountId = fixture.DeviceAccountId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 3,
                        PartyNo = 1,
                        DeviceAccountId = fixture.DeviceAccountId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 4,
                        PartyNo = 1,
                        DeviceAccountId = fixture.DeviceAccountId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                },
                opts => opts.Excluding(x => x.Id)
            );
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedCharacter_ReturnsBadRequest()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                new List<PartySettingList>()
                {
                    new(1, Charas.GalaGatov, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                },
                "My New Party",
                0,
                0
            );

        HttpContent content = TestUtils.CreateMsgpackContent(request);

        HttpResponseMessage response = await client.PostAsync("/party/set_party_setting", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedDragonKeyId_ReturnsBadRequest()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                new List<PartySettingList>()
                {
                    new(1, Charas.ThePrince, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                },
                "My New Party",
                0,
                0
            );

        HttpContent content = TestUtils.CreateMsgpackContent(request);

        HttpResponseMessage response = await client.PostAsync("/party/set_party_setting", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetMainPartyNo_UpdatesDatabase()
    {
        await client.PostMsgpack<PartySetMainPartyNoRequest>(
            "/party/set_main_party_no",
            new PartySetMainPartyNoRequest(2)
        );

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbPlayerUserData userData = await apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .SingleAsync();

        userData.MainPartyNo.Should().Be(2);
    }
}
