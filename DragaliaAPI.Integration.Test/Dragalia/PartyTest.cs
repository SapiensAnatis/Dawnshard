using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.PartyController"/>
/// </summary>
public class PartyTest : TestFixture
{
    public PartyTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task SetPartySetting_ValidRequest_UpdatesDatabase()
    {
        this.AddCharacter(Charas.Ilia);

        await AddToDatabase(
            new DbWeaponBody { ViewerId = ViewerId, WeaponBodyId = WeaponBodies.DivineTrigger }
        );

        await this.Client.PostMsgpack<PartySetPartySettingData>(
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
                false,
                0
            )
        );

        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();
        DbParty dbparty = await apiContext
            .PlayerParties.Include(x => x.Units)
            .Where(x => x.ViewerId == ViewerId && x.PartyNo == 1)
            .SingleAsync();

        dbparty
            .Should()
            .BeEquivalentTo(
                new DbParty()
                {
                    ViewerId = ViewerId,
                    PartyNo = 1,
                    PartyName = "My New Party",
                },
                opts => opts.Excluding(x => x.Units)
            );

        dbparty
            .Units.Should()
            .BeEquivalentTo(
                new List<DbPartyUnit>()
                {
                    new()
                    {
                        UnitNo = 1,
                        PartyNo = 1,
                        ViewerId = ViewerId,
                        CharaId = Charas.Ilia,
                        EquipCrestSlotType1CrestId1 = AbilityCrests.ADragonyuleforIlia,
                        EquipWeaponBodyId = WeaponBodies.DivineTrigger,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 2,
                        PartyNo = 1,
                        ViewerId = ViewerId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 3,
                        PartyNo = 1,
                        ViewerId = ViewerId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                    new()
                    {
                        UnitNo = 4,
                        PartyNo = 1,
                        ViewerId = ViewerId,
                        CharaId = Charas.Empty,
                        Party = dbparty,
                    },
                },
                opts => opts.Excluding(x => x.Id)
            );
    }

    [Fact]
    public async Task SetPartySetting_IllegalCharacters_Handles()
    {
        Charas storyZethia = (Charas)19900001;
        this.AddCharacter(storyZethia);

        (
            await this.Client.PostMsgpack<PartySetPartySettingData>(
                "/party/set_party_setting",
                new PartySetPartySettingRequest(
                    1,
                    new List<PartySettingList>()
                    {
                        new() { unit_no = 1, chara_id = storyZethia, }
                    },
                    "My New Party",
                    false,
                    0
                )
            )
        ).data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedCharacter_ReturnsResultCode()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                new List<PartySettingList>()
                {
                    new(1, Charas.GalaGatov, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                },
                "My New Party",
                false,
                0
            );

        (
            await this.Client.PostMsgpack<ResultCodeData>(
                "/party/set_party_setting",
                request,
                ensureSuccessHeader: false
            )
        )
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.PartySwitchSettingCharaShort),
                    new ResultCodeData(ResultCode.PartySwitchSettingCharaShort)
                )
            );
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedDragonKeyId_ReturnsResultCode()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                new List<PartySettingList>()
                {
                    new(1, Charas.ThePrince, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                },
                "My New Party",
                false,
                0
            );

        (
            await this.Client.PostMsgpack<ResultCodeData>(
                "/party/set_party_setting",
                request,
                ensureSuccessHeader: false
            )
        )
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.PartySwitchSettingCharaShort),
                    new ResultCodeData(ResultCode.PartySwitchSettingCharaShort)
                )
            );
    }

    [Fact]
    public async Task SetMainPartyNo_UpdatesDatabase()
    {
        await this.Client.PostMsgpack<PartySetMainPartyNoData>(
            "/party/set_main_party_no",
            new PartySetMainPartyNoRequest(2)
        );

        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();
        DbPlayerUserData userData = await apiContext
            .PlayerUserData.Where(x => x.ViewerId == ViewerId)
            .SingleAsync();

        userData.MainPartyNo.Should().Be(2);
    }

    [Fact]
    public async Task UpdatePartyName_UpdatesDatabase()
    {
        DbParty party =
            await this.ApiContext.PlayerParties.FindAsync(ViewerId, 1)
            ?? throw new NullReferenceException();

        await this.Client.PostMsgpack<PartyUpdatePartyNameData>(
            "/party/update_party_name",
            new PartyUpdatePartyNameRequest() { party_no = 1, party_name = "LIblis Full Auto" }
        );

        await this.ApiContext.Entry(party).ReloadAsync();

        party.PartyName.Should().Be("LIblis Full Auto");
    }

    [Fact]
    public async Task UpdatePartyName_ReturnsCorrectResponse()
    {
        PartyUpdatePartyNameData response = (
            await this.Client.PostMsgpack<PartyUpdatePartyNameData>(
                "/party/update_party_name",
                new PartyUpdatePartyNameRequest() { party_no = 2, party_name = "LIblis Full Auto" }
            )
        ).data;

        response.update_data_list.Should().NotBeNull();

        PartyList updateParty = response.update_data_list.party_list.ElementAt(0);
        updateParty.party_name.Should().Be("LIblis Full Auto");
        updateParty.party_no.Should().Be(2);
        updateParty.party_setting_list.Should().NotBeEmpty();
        updateParty.party_setting_list.Should().BeInAscendingOrder(x => x.unit_no);
    }
}
