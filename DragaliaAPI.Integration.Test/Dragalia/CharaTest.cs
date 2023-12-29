using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="CharaController"/>
/// </summary>
public class CharaTest : TestFixture
{
    public CharaTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override Task Setup()
    {
        this.AddCharacter(Charas.Naveed);
        this.AddCharacter(Charas.Ezelith);
        this.AddCharacter(Charas.Marth);
        this.AddCharacter(Charas.Hawk);
        this.AddCharacter(Charas.Mikoto);
        this.AddCharacter(Charas.Vanessa);
        this.AddCharacter(Charas.Celliera);
        this.AddCharacter(Charas.SummerCelliera);
        this.AddCharacter(Charas.Harle);
        this.AddCharacter(Charas.Ezelith);
        this.AddCharacter(Charas.Xander);
        this.AddCharacter(Charas.Vixel);
        this.AddCharacter(Charas.Nefaria);
        this.AddCharacter(Charas.Mitsuba);
        this.AddCharacter(Charas.Pipple);
        this.AddCharacter(Charas.GalaZethia);
        this.AddCharacter(Charas.Vida);
        this.AddCharacter(Charas.Delphi);
        this.AddCharacter(Charas.GalaAudric);
        this.AddCharacter(Charas.Gauld);

        return Task.CompletedTask;
    }

    [Fact]
    public async Task CharaAwake_IncreasedRarity()
    {
        CharaAwakeData response = (
            await this.Client.PostMsgpack<CharaAwakeData>(
                "chara/awake",
                new CharaAwakeRequest(Charas.Celliera, 5)
            )
        ).data;

        CharaList charaData = response
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        charaData.rarity.Should().Be(5);
    }

    [Fact]
    public async Task CharaBuildup_HasIncreasedXpAndLevel()
    {
        DbPlayerCharaData charaData;
        int matQuantity;

        using (
            IDisposable ctx = this.Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            charaData = await this.Services.GetRequiredService<IUnitRepository>()
                .Charas.Where(x => x.CharaId == Charas.Celliera)
                .FirstAsync();

            matQuantity = (
                await this.Services.GetRequiredService<IInventoryRepository>()
                    .GetMaterial(Materials.GoldCrystal)
            )!.Quantity;
        }

        CharaBuildupData response = (
            await this.Client.PostMsgpack<CharaBuildupData>(
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

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        responseCharaData
            .level.Should()
            .Be(Math.Min(CharaConstants.XpLimits.FindIndex(0, x => x > expectedXp), maxLevel));
        responseCharaData.exp.Should().Be(expectedXp);

        response
            .update_data_list.material_list.Where(
                x => (Materials)x.material_id == Materials.GoldCrystal
            )
            .First()
            .quantity.Should()
            .Be(matQuantity - 300);
    }

    [Fact]
    public async Task CharaBuildup_NotEnoughForLevel_DoesNotLevelUp()
    {
        await this.Client.PostMsgpack(
            "chara/buildup",
            new CharaBuildupRequest(
                Charas.Gauld,
                [new AtgenEnemyPiece() { id = Materials.GoldCrystal, quantity = 10 }]
            )
        );

        byte currentLevel = this.ApiContext.PlayerCharaData.AsNoTracking()
            .First(x => x.CharaId == Charas.Gauld)
            .Level;

        await this.Client.PostMsgpack(
            "chara/buildup",
            new CharaBuildupRequest(
                Charas.Gauld,
                [new AtgenEnemyPiece() { id = Materials.BronzeCrystal, quantity = 1 }]
            )
        );

        this.ApiContext.PlayerCharaData.AsNoTracking()
            .First(x => x.CharaId == Charas.Gauld)
            .Level.Should()
            .Be(currentLevel);
    }

    [Fact]
    public async Task CharaBuildupMana_HasManaNodes()
    {
        int manaPointNum;

        using (
            IDisposable ctx = this.Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            manaPointNum = (
                await this.Services.GetRequiredService<IUserDataRepository>().GetUserDataAsync()
            ).ManaPoint;
        }

        CharaBuildupManaData response = (
            await this.Client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(
                    Charas.Celliera,
                    new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                    false
                )
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData
            .mana_circle_piece_id_list.Should()
            .ContainInOrder(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

        response
            .update_data_list.user_data.mana_point.Should()
            .Be(manaPointNum - 350 - 450 - 650 - 350 - 450 - 350 - 500 - 450 - 350 - 650);
    }

    [Fact]
    public async Task CharaBuildupMana_AllStoriesUnlocked_DoesNotThrow()
    {
        foreach (int storyId in MasterAsset.CharaStories[(int)Charas.GalaAudric].storyIds)
        {
            await this.AddToDatabase(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryId = storyId,
                    State = StoryState.Read,
                    StoryType = StoryTypes.Chara
                }
            );
        }

        DragaliaResponse<CharaBuildupManaData> response = (
            await this.Client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(Charas.GalaAudric, new List<int>() { 5 }, false)
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task CharaLimitBreak_HasNextFloor()
    {
        int mat1Quantity,
            mat2Quantity;

        using (
            IDisposable ctx = this.Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            IInventoryRepository inventoryRepository =
                this.Services.GetRequiredService<IInventoryRepository>();
            mat1Quantity = (await inventoryRepository.GetMaterial(Materials.WaterOrb))!.Quantity;
            mat2Quantity = (await inventoryRepository.GetMaterial(Materials.StreamOrb))!.Quantity;
        }

        CharaLimitBreakData response = (
            await this.Client.PostMsgpack<CharaLimitBreakData>(
                "chara/limit_break",
                new CharaLimitBreakRequest(Charas.Celliera, 1, false)
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData.limit_break_count.Should().Be(1);

        response
            .update_data_list.material_list.Should()
            .ContainEquivalentOf(new MaterialList(Materials.WaterOrb, mat1Quantity - 8))
            .And.ContainEquivalentOf(new MaterialList(Materials.StreamOrb, mat2Quantity - 1));
    }

    [Fact]
    public async Task CharaLimitBreakAndBuildupMana_ReturnCorrectData()
    {
        int mat1Quantity,
            mat2Quantity,
            mat3Quantity,
            manaPointNum;

        using (
            IDisposable ctx = this.Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            IInventoryRepository inventoryRepository =
                this.Services.GetRequiredService<IInventoryRepository>();
            mat1Quantity = (await inventoryRepository.GetMaterial(Materials.WaterOrb))!.Quantity;
            mat2Quantity = (await inventoryRepository.GetMaterial(Materials.StreamOrb))!.Quantity;
            mat3Quantity = (await inventoryRepository.GetMaterial(Materials.DelugeOrb))!.Quantity;

            manaPointNum = (
                await this.Services.GetRequiredService<IUserDataRepository>().GetUserDataAsync()
            ).ManaPoint;
        }

        CharaLimitBreakAndBuildupManaData response = (
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaData>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    Charas.Celliera,
                    2,
                    new List<int>() { 21, 22, 24, 25, 28 },
                    false
                )
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();

        responseCharaData.limit_break_count.Should().Be(2);
        responseCharaData
            .mana_circle_piece_id_list.Should()
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

        response
            .update_data_list.user_data.mana_point.Should()
            .Be(manaPointNum - 2800 - 3400 - 5200 - 3400 - 2800);

        response
            .update_data_list.material_list.Should()
            .ContainEquivalentOf(new MaterialList(Materials.WaterOrb, mat1Quantity - 15 - 25))
            .And.ContainEquivalentOf(new MaterialList(Materials.StreamOrb, mat2Quantity - 4 - 5))
            .And.ContainEquivalentOf(new MaterialList(Materials.DelugeOrb, mat3Quantity - 1 - 1));
    }

    [Fact]
    public async Task CharaManaNodeBuildupHasCorrectInfoOnBothSidesOfSpiral()
    {
        CharaLimitBreakAndBuildupManaData preSpiralResponse = (
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaData>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    Charas.Delphi,
                    4,
                    Enumerable.Range(1, 50),
                    false
                )
            )
        ).data;

        CharaList preSpiralResponseCharaData = preSpiralResponse
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Delphi)
            .First();

        CharaData charaData = MasterAsset.CharaData.Get(Charas.Delphi);

        preSpiralResponseCharaData.level.Should().Be(1);
        preSpiralResponseCharaData.mana_circle_piece_id_list.Count().Should().Be(50);
        preSpiralResponseCharaData.additional_max_level.Should().Be(0);
        preSpiralResponseCharaData
            .hp.Should()
            .Be(
                charaData.PlusHp0
                    + charaData.PlusHp1
                    + charaData.PlusHp2
                    + charaData.PlusHp3
                    + charaData.PlusHp4
                    + charaData.McFullBonusHp5
                    + 67
            );
        preSpiralResponseCharaData
            .attack.Should()
            .Be(
                charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.McFullBonusAtk5
                    + 40
            );

        await this.Client.PostMsgpack<CharaLimitBreakData>(
            "chara/limit_break",
            new CharaLimitBreakRequest(Charas.Delphi, 5, false)
        );

        CharaBuildupManaData postSpiralResponse = (
            await this.Client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(Charas.Delphi, Enumerable.Range(51, 20), false)
            )
        ).data;

        CharaList postSpiralResponseCharaData = postSpiralResponse
            .update_data_list.chara_list.Where(x => (Charas)x.chara_id == Charas.Delphi)
            .First();

        postSpiralResponseCharaData.level.Should().Be(1);
        postSpiralResponseCharaData.mana_circle_piece_id_list.Count().Should().Be(70);
        postSpiralResponseCharaData.additional_max_level.Should().Be(20);
        postSpiralResponseCharaData
            .hp.Should()
            .Be(
                charaData.PlusHp0
                    + charaData.PlusHp1
                    + charaData.PlusHp2
                    + charaData.PlusHp3
                    + charaData.PlusHp4
                    + charaData.PlusHp5
                    + charaData.McFullBonusHp5
                    + 67
            );
        postSpiralResponseCharaData
            .attack.Should()
            .Be(
                charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.PlusAtk5
                    + charaData.McFullBonusAtk5
                    + 40
            );
    }

    [Fact]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithSpiralledCharacter()
    {
        CharaBuildupPlatinumData response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.SummerCelliera)
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => x.chara_id == Charas.SummerCelliera)
            .First();

        responseCharaData.level.Should().Be(100);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[99]);
        responseCharaData.additional_max_level.Should().Be(20);

        // Values from wiki
        responseCharaData.hp.Should().Be(956);
        responseCharaData.attack.Should().Be(574);

        responseCharaData.limit_break_count.Should().Be(5);
        responseCharaData
            .mana_circle_piece_id_list.Should()
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
            await this.Client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.Harle)
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => x.chara_id == Charas.Harle)
            .First();

        responseCharaData.level.Should().Be(80);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.additional_max_level.Should().Be(0);

        responseCharaData.hp.Should().Be(741);
        responseCharaData.attack.Should().Be(470);

        responseCharaData.limit_break_count.Should().Be(4);
        responseCharaData
            .mana_circle_piece_id_list.Should()
            .BeEquivalentTo(Enumerable.Range(1, 50));

        responseCharaData.skill_1_level.Should().Be(3);
        responseCharaData.skill_2_level.Should().Be(2);
        responseCharaData.ability_1_level.Should().Be(2);
        responseCharaData.ability_2_level.Should().Be(2);
        responseCharaData.ability_3_level.Should().Be(2);
    }

    [Theory]
    [InlineData(2, 0, Charas.Naveed)]
    [InlineData(10, 1, Charas.Ezelith)]
    [InlineData(20, 2, Charas.Marth)]
    [InlineData(30, 2, Charas.Hawk)] // limit break node not unlocked
    [InlineData(30, 3, Charas.Mikoto)]
    [InlineData(40, 4, Charas.Vanessa)]
    [InlineData(50, 5, Charas.Xander)]
    [InlineData(60, 5, Charas.Vixel)]
    [InlineData(70, 5, Charas.Nefaria)]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithSpiralledCharacterWhenAlreadyHalfBuilt(
        int manaNodes,
        int limitBreak,
        Charas id
    )
    {
        if (limitBreak == 0)
        {
            await this.Client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(id, Enumerable.Range(1, manaNodes), false)
            );
        }
        else
        {
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaData>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    id,
                    limitBreak,
                    Enumerable.Range(1, manaNodes),
                    false
                )
            );
        }

        CharaBuildupPlatinumData response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => x.chara_id == id)
            .First();

        responseCharaData.level.Should().Be(100);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[99]);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        // Values from wiki
        responseCharaData
            .hp.Should()
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
        responseCharaData
            .attack.Should()
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
        responseCharaData
            .mana_circle_piece_id_list.Should()
            .BeEquivalentTo(Enumerable.Range(1, 70));

        responseCharaData.skill_1_level.Should().Be(4);
        responseCharaData.skill_2_level.Should().Be(3);
        responseCharaData.ability_1_level.Should().Be(charaData.MaxAbility1Level);
        responseCharaData.ability_2_level.Should().Be(charaData.MaxAbility2Level);
        responseCharaData.ability_3_level.Should().Be(charaData.MaxAbility3Level);
    }

    [Theory]
    [InlineData(1, 0, Charas.Mitsuba)]
    [InlineData(25, 2, Charas.Pipple)]
    [InlineData(40, 3, Charas.GalaZethia)]
    [InlineData(50, 4, Charas.Vida)]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithNonSpiralledCharacterWhenAlreadyHalfBuilt(
        int manaNodes,
        int limitBreak,
        Charas id
    )
    {
        if (limitBreak == 0)
        {
            await this.Client.PostMsgpack<CharaBuildupManaData>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(id, Enumerable.Range(1, manaNodes), false)
            );
        }
        else
        {
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaData>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    id,
                    limitBreak,
                    Enumerable.Range(1, manaNodes),
                    false
                )
            );
        }

        CharaBuildupPlatinumData response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumData>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).data;

        CharaList responseCharaData = response
            .update_data_list.chara_list.Where(x => x.chara_id == id)
            .First();

        responseCharaData.level.Should().Be(80);
        responseCharaData.exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.additional_max_level.Should().Be(0);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        responseCharaData
            .hp.Should()
            .Be(
                charaData.MaxHp
                    + charaData.PlusHp0
                    + charaData.PlusHp1
                    + charaData.PlusHp2
                    + charaData.PlusHp3
                    + charaData.PlusHp4
                    + charaData.McFullBonusHp5
            );
        responseCharaData
            .attack.Should()
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
        responseCharaData
            .mana_circle_piece_id_list.Should()
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
            await this.Client.PostMsgpack<CharaGetCharaUnitSetData>(
                "chara/get_chara_unit_set",
                new CharaGetCharaUnitSetRequest(new List<Charas>() { Charas.Celliera })
            )
        ).data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task CharaSetCharaUnitSet_ReturnsCorrectSetData()
    {
        CharaSetCharaUnitSetData response = (
            await this.Client.PostMsgpack<CharaSetCharaUnitSetData>(
                "chara/set_chara_unit_set",
                new CharaSetCharaUnitSetRequest(
                    1,
                    "Exercise",
                    Charas.Celliera,
                    new AtgenRequestCharaUnitSetData() { dragon_key_id = 5 }
                )
            )
        ).data;

        CharaUnitSetList responseCharaData = response
            .update_data_list.chara_unit_set_list.Where(x => (Charas)x.chara_id == Charas.Celliera)
            .First();
        responseCharaData.chara_unit_set_detail_list.ToList()[0].dragon_key_id.Should().Be(5);
    }
}
