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

        await this.Client.PostMsgpack<PartySetPartySettingResponse>(
            "/party/set_party_setting",
            new PartySetPartySettingRequest(
                1,
                new List<PartySettingList>()
                {
                    new()
                    {
                        UnitNo = 1,
                        CharaId = Charas.Ilia,
                        EquipWeaponBodyId = WeaponBodies.DivineTrigger,
                        EquipCrestSlotType1CrestId1 = AbilityCrestId.ADragonyuleforIlia,
                    },
                },
                "My New Party",
                false,
                0
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();
        DbParty dbparty = await apiContext
            .PlayerParties.Include(x => x.Units)
            .Where(x => x.ViewerId == ViewerId && x.PartyNo == 1)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

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
                        EquipCrestSlotType1CrestId1 = AbilityCrestId.ADragonyuleforIlia,
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
            await this.Client.PostMsgpack<PartySetPartySettingResponse>(
                "/party/set_party_setting",
                new PartySetPartySettingRequest(
                    1,
                    new List<PartySettingList>()
                    {
                        new() { UnitNo = 1, CharaId = storyZethia },
                    },
                    "My New Party",
                    false,
                    0
                ),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedCharacter_ReturnsResultCode()
    {
        PartySetPartySettingRequest request = new(
            1,
            new List<PartySettingList>()
            {
                new(1, Charas.GalaGatov, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            },
            "My New Party",
            false,
            0
        );

        (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "/party/set_party_setting",
                request,
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new ResultCodeResponse(ResultCode.PartySwitchSettingCharaShort),
                    new DataHeaders(ResultCode.PartySwitchSettingCharaShort)
                )
            );
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedDragonKeyId_ReturnsResultCode()
    {
        PartySetPartySettingRequest request = new(
            1,
            new List<PartySettingList>()
            {
                new(1, Charas.ThePrince, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            },
            "My New Party",
            false,
            0
        );

        (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "/party/set_party_setting",
                request,
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new ResultCodeResponse(ResultCode.PartySwitchSettingCharaShort),
                    new DataHeaders(ResultCode.PartySwitchSettingCharaShort)
                )
            );
    }

    [Fact]
    public async Task SetMainPartyNo_UpdatesDatabase()
    {
        await this.Client.PostMsgpack<PartySetMainPartyNoResponse>(
            "/party/set_main_party_no",
            new PartySetMainPartyNoRequest(2),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        userData.MainPartyNo.Should().Be(2);
    }

    [Fact]
    public async Task UpdatePartyName_UpdatesDatabase()
    {
        DbParty party =
            await this.ApiContext.PlayerParties.FindAsync(
                [ViewerId, 1],
                TestContext.Current.CancellationToken
            ) ?? throw new NullReferenceException();

        await this.Client.PostMsgpack<PartyUpdatePartyNameResponse>(
            "/party/update_party_name",
            new PartyUpdatePartyNameRequest() { PartyNo = 1, PartyName = "LIblis Full Auto" },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.ApiContext.Entry(party).ReloadAsync(TestContext.Current.CancellationToken);

        party.PartyName.Should().Be("LIblis Full Auto");
    }

    [Fact]
    public async Task UpdatePartyName_ReturnsCorrectResponse()
    {
        PartyUpdatePartyNameResponse response = (
            await this.Client.PostMsgpack<PartyUpdatePartyNameResponse>(
                "/party/update_party_name",
                new PartyUpdatePartyNameRequest() { PartyNo = 2, PartyName = "LIblis Full Auto" },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.Should().NotBeNull();

        PartyList updateParty = response.UpdateDataList.PartyList!.ElementAt(0);
        updateParty.PartyName.Should().Be("LIblis Full Auto");
        updateParty.PartyNo.Should().Be(2);
        updateParty.PartySettingList.Should().NotBeEmpty();
        updateParty.PartySettingList.Should().BeInAscendingOrder(x => x.UnitNo);
    }
}
