using MessagePack.Resolvers;
using MessagePack;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.CharaController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class CharaTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;
    private readonly ITestOutputHelper outputHelper;

    public CharaTest(IntegrationTestFixture fixture, ITestOutputHelper outputHelper)
    {
        this.fixture = fixture;
        this.outputHelper = outputHelper;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        fixture.AddCharacter(Charas.Celliera).Wait();
        fixture.AddCharacter(Charas.SummerCelliera).Wait();
        fixture.AddCharacter(Charas.Harle).Wait();
        fixture.AddCharacter(Charas.Ezelith).Wait();
        fixture.AddCharacter(Charas.Xander).Wait();
        fixture.AddCharacter(Charas.Vixel).Wait();
        fixture.AddCharacter(Charas.Nefaria).Wait();
        fixture.AddCharacter(Charas.Mitsuba).Wait();
        fixture.AddCharacter(Charas.Pipple).Wait();
        fixture.AddCharacter(Charas.GalaZethia).Wait();
        fixture.AddCharacter(Charas.Vida).Wait();
    }

    [Fact]
    public async Task CharaAwake_IncreasedRarity()
    {
        CharaAwakeData response = (
            await client.PostMsgpack<CharaAwakeData>(
                "chara/awake",
                new CharaAwakeRequest(Charas.Celliera, 5)
            )
        ).data;

        CharaList charaData = response.update_data_list.chara_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        charaData.rarity.Should().Be(5);
    }

    [Fact]
    public async Task CharaBuildup_HasIncreasedXpAndLevel()
    {
        DbPlayerCharaData charaData = await fixture.Services
            .GetRequiredService<IUnitRepository>()
            .GetAllCharaData(fixture.DeviceAccountId)
            .Where(x => x.CharaId == Charas.Celliera)
            .FirstAsync();

        int matQuantity = (
            await fixture.Services
                .GetRequiredService<IInventoryRepository>()
                .GetMaterial(fixture.DeviceAccountId, Materials.GoldCrystal)
        )!.Quantity;

        CharaBuildupData response = (
            await client.PostMsgpack<CharaBuildupData>(
                "chara/buildup",
                new CharaBuildupRequest(
                    Charas.Celliera,
                    new List<AtgenEnemyPiece>()
                    {
                        new AtgenEnemyPiece() { id = Materials.GoldCrystal, quantity = 300 }
                    }
                )
            )
        ).data;

        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(charaData.Rarity) + charaData.AdditionalMaxLevel
        );

        int expectedXp = Math.Min(
            UpgradeMaterials.buildupXpValues[Materials.GoldCrystal] * 300,
            CharaConstants.XpLimits[maxLevel - 1]
        );

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        responseCharaData.level
            .Should()
            .Be(Math.Min(CharaConstants.XpLimits.FindIndex(0, x => x > expectedXp), maxLevel));
        responseCharaData.exp.Should().Be(expectedXp);

        response.update_data_list.material_list
            .Where(x => (Materials)x.material_id == Materials.GoldCrystal)
            .First()
            .quantity.Should()
            .Be(matQuantity - 300);
    }

    [Fact]
    public async Task CharaBuildupMana_HasManaNodes()
    {
        CharaBuildupManaData response = (
            await client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(
                    Charas.Celliera,
                    new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                    (int)CharaUpgradeMaterialTypes.Standard
                )
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData.mana_circle_piece_id_list
            .Should()
            .ContainInOrder(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [Fact]
    public async Task CharaLimitBreak_HasNextFloor()
    {
        CharaLimitBreakData response = (
            await client.PostMsgpack<CharaLimitBreakData>(
                "chara/limit_break",
                new CharaLimitBreakRequest(
                    Charas.Celliera,
                    1,
                    (int)CharaUpgradeMaterialTypes.Standard
                )
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData.limit_break_count.Should().Be(1);
    }

    [Fact]
    public async Task CharaLimitBreakAndBuildupMana_ReturnCorrectData()
    {
        CharaLimitBreakAndBuildupManaData response = (
            await client.PostMsgpack<CharaLimitBreakAndBuildupManaData>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    Charas.Celliera,
                    2,
                    new List<int>() { 21, 22, 24, 25, 28 },
                    (int)CharaUpgradeMaterialTypes.Standard
                )
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData.limit_break_count.Should().Be(2);
        responseCharaData.mana_circle_piece_id_list
            .Should()
            .ContainInOrder(
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12,
                13,
                14,
                15,
                16,
                17,
                18,
                19,
                20,
                21,
                22,
                24,
                25,
                28
            );
    }

    [Fact]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithSpiralledCharacter()
    {
        CharaBuildupPlatinumData response = (
            await client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.SummerCelliera)
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => x.chara_id == Charas.SummerCelliera)
            .First();

        responseCharaData.level.Should().Be(100);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[99]);
        responseCharaData.additional_max_level.Should().Be(20);

        // Values from wiki
        responseCharaData.hp.Should().Be(956);
        responseCharaData.attack.Should().Be(574);

        responseCharaData.limit_break_count.Should().Be(5);
        responseCharaData.mana_circle_piece_id_list
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 70));

        responseCharaData.skill_1_level.Should().Be(4);
        responseCharaData.skill_2_level.Should().Be(3);
        responseCharaData.ability_1_level.Should().Be(3);
        responseCharaData.ability_2_level.Should().Be(3);
        responseCharaData.ability_3_level.Should().Be(3);
    }

    [Fact]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithNonSpiralledCharacter()
    {
        CharaBuildupPlatinumData response = (
            await client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.Harle)
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => x.chara_id == Charas.Harle)
            .First();

        responseCharaData.level.Should().Be(80);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.additional_max_level.Should().Be(0);

        responseCharaData.hp.Should().Be(741);
        responseCharaData.attack.Should().Be(470);

        responseCharaData.limit_break_count.Should().Be(4);
        responseCharaData.mana_circle_piece_id_list
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 50));

        responseCharaData.skill_1_level.Should().Be(3);
        responseCharaData.skill_2_level.Should().Be(2);
        responseCharaData.ability_1_level.Should().Be(2);
        responseCharaData.ability_2_level.Should().Be(2);
        responseCharaData.ability_3_level.Should().Be(2);
    }

    [Theory]
    [InlineData(40, Charas.Ezelith)]
    [InlineData(50, Charas.Xander)]
    [InlineData(60, Charas.Vixel)]
    [InlineData(70, Charas.Nefaria)]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithSpiralledCharacterWhenAlreadyHalfBuilt(
        int manaNodes,
        Charas id
    )
    {
        await client.PostMsgpack<CharaBuildupManaData>(
            "chara/buildup_mana",
            new CharaBuildupManaRequest(
                id,
                Enumerable.Range(1, manaNodes),
                (int)CharaUpgradeMaterialTypes.Standard
            )
        );

        CharaBuildupPlatinumData response = (
            await client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => x.chara_id == id)
            .First();

        responseCharaData.level.Should().Be(100);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[99]);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        // Values from wiki
        responseCharaData.hp
            .Should()
            .Be(
                charaData.AddMaxHp1
                    + charaData.PlusHp0
                    + charaData.PlusHp1
                    + charaData.PlusHp2
                    + charaData.PlusHp3
                    + charaData.PlusHp4
                    + charaData.PlusHp5
                    + charaData.McFullBonusHp5
            );
        responseCharaData.attack
            .Should()
            .Be(
                charaData.AddMaxAtk1
                    + charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.PlusAtk5
                    + charaData.McFullBonusAtk5
            );

        responseCharaData.limit_break_count.Should().Be(5);
        responseCharaData.mana_circle_piece_id_list
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 70));

        responseCharaData.skill_1_level.Should().Be(4);
        responseCharaData.skill_2_level.Should().Be(3);
        responseCharaData.ability_1_level.Should().Be(charaData.MaxAbility1Level);
        responseCharaData.ability_2_level.Should().Be(charaData.MaxAbility2Level);
        responseCharaData.ability_3_level.Should().Be(charaData.MaxAbility3Level);
    }

    [Theory]
    [InlineData(1, Charas.Mitsuba)]
    [InlineData(25, Charas.Pipple)]
    [InlineData(40, Charas.GalaZethia)]
    [InlineData(50, Charas.Vida)]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithNonSpiralledCharacterWhenAlreadyHalfBuilt(
        int manaNodes,
        Charas id
    )
    {
        await client.PostMsgpack<CharaBuildupManaData>(
            "chara/buildup_mana",
            new CharaBuildupManaRequest(
                id,
                Enumerable.Range(1, manaNodes),
                (int)CharaUpgradeMaterialTypes.Standard
            )
        );
        CharaBuildupPlatinumData response = (
            await client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).data;

        CharaList responseCharaData = response.update_data_list.chara_list
            .Where(x => x.chara_id == id)
            .First();

        responseCharaData.level.Should().Be(80);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.additional_max_level.Should().Be(0);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        responseCharaData.hp
            .Should()
            .Be(
                charaData.MaxHp
                    + charaData.PlusHp0
                    + charaData.PlusHp1
                    + charaData.PlusHp2
                    + charaData.PlusHp3
                    + charaData.PlusHp4
                    + charaData.McFullBonusHp5
            );
        responseCharaData.attack
            .Should()
            .Be(
                charaData.MaxAtk
                    + charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.McFullBonusAtk5
            );

        responseCharaData.limit_break_count.Should().Be(4);
        responseCharaData.mana_circle_piece_id_list
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 50));

        responseCharaData.skill_1_level.Should().Be(3);
        responseCharaData.skill_2_level.Should().Be(id == Charas.GalaZethia ? 0 : 2);
        responseCharaData.ability_1_level.Should().Be(charaData.MaxAbility1Level);
        responseCharaData.ability_2_level.Should().Be(charaData.MaxAbility2Level);
        responseCharaData.ability_3_level.Should().Be(charaData.MaxAbility3Level);
    }

    [Fact]
    public async Task CharaGetCharaUnitSet_ReturnsData()
    {
        CharaGetCharaUnitSetData response = (
            await client.PostMsgpack<CharaGetCharaUnitSetData>(
                "chara/get_chara_unit_set",
                new CharaGetCharaUnitSetRequest(new List<int>() { (int)Charas.Celliera })
            )
        ).data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task CharaSetCharaUnitSet_ReturnsCorrectSetData()
    {
        CharaSetCharaUnitSetData response = (
            await client.PostMsgpack<CharaSetCharaUnitSetData>(
                "chara/set_chara_unit_set",
                new CharaSetCharaUnitSetRequest(
                    1,
                    "Exercise",
                    Charas.Celliera,
                    new AtgenRequestCharaUnitSetData() { dragon_key_id = 5 }
                )
            )
        ).data;

        CharaUnitSetList responseCharaData = response.update_data_list.chara_unit_set_list
            .Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        responseCharaData.chara_unit_set_detail_list.ToList()[0].dragon_key_id.Should().Be(5);
    }
}
