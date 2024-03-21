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
        : base(factory, outputHelper)
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
    }

    [Fact]
    public async Task CharaAwake_IncreasedRarity()
    {
        CharaAwakeResponse response = (
            await this.Client.PostMsgpack<CharaAwakeResponse>(
                "chara/awake",
                new CharaAwakeRequest(Charas.Celliera, 5)
            )
        ).Data;

        CharaList charaData = response.UpdateDataList.CharaList!.First(x =>
            x.CharaId == Charas.Celliera
        );
        charaData.Rarity.Should().Be(5);
    }

    [Fact]
    public async Task CharaBuildup_HasIncreasedXpAndLevel()
    {
        DbPlayerCharaData charaData;
        int matQuantity;

        using (
            IDisposable ctx = this
                .Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            charaData = await this
                .Services.GetRequiredService<IUnitRepository>()
                .Charas.Where(x => x.CharaId == Charas.Celliera)
                .FirstAsync();

            matQuantity = (
                await this
                    .Services.GetRequiredService<IInventoryRepository>()
                    .GetMaterial(Materials.GoldCrystal)
            )!.Quantity;
        }

        CharaBuildupResponse response = (
            await this.Client.PostMsgpack<CharaBuildupResponse>(
                "chara/buildup",
                new CharaBuildupRequest(
                    Charas.Celliera,
                    new List<AtgenEnemyPiece>()
                    {
                        new AtgenEnemyPiece() { Id = Materials.GoldCrystal, Quantity = 300 }
                    }
                )
            )
        ).Data;

        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(charaData.Rarity) + charaData.AdditionalMaxLevel
        );

        int expectedXp = Math.Min(
            UpgradeMaterials.buildupXpValues[Materials.GoldCrystal] * 300,
            CharaConstants.XpLimits[maxLevel - 1]
        );

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Celliera)
            .First();
        responseCharaData
            .Level.Should()
            .Be(Math.Min(CharaConstants.XpLimits.FindIndex(0, x => x > expectedXp), maxLevel));
        responseCharaData.Exp.Should().Be(expectedXp);

        response
            .UpdateDataList.MaterialList!.Where(x =>
                (Materials)x.MaterialId == Materials.GoldCrystal
            )
            .First()
            .Quantity.Should()
            .Be(matQuantity - 300);
    }

    [Fact]
    public async Task CharaBuildup_NotEnoughForLevel_DoesNotLevelUp()
    {
        await this.Client.PostMsgpack(
            "chara/buildup",
            new CharaBuildupRequest(
                Charas.Gauld,
                [new AtgenEnemyPiece() { Id = Materials.GoldCrystal, Quantity = 10 }]
            )
        );

        byte currentLevel = this
            .ApiContext.PlayerCharaData.AsNoTracking()
            .First(x => x.CharaId == Charas.Gauld)
            .Level;

        await this.Client.PostMsgpack(
            "chara/buildup",
            new CharaBuildupRequest(
                Charas.Gauld,
                [new AtgenEnemyPiece() { Id = Materials.BronzeCrystal, Quantity = 1 }]
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
            IDisposable ctx = this
                .Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            manaPointNum = (
                await this.Services.GetRequiredService<IUserDataRepository>().GetUserDataAsync()
            ).ManaPoint;
        }

        CharaBuildupManaResponse response = (
            await this.Client.PostMsgpack<CharaBuildupManaResponse>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(
                    Charas.Celliera,
                    new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                    false
                )
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Celliera)
            .First();

        responseCharaData
            .ManaCirclePieceIdList.Should()
            .ContainInOrder(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

        response
            .UpdateDataList.UserData.ManaPoint.Should()
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

        DragaliaResponse<CharaBuildupManaResponse> response = (
            await this.Client.PostMsgpack<CharaBuildupManaResponse>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(Charas.GalaAudric, new List<int>() { 5 }, false)
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task CharaLimitBreak_HasNextFloor()
    {
        int mat1Quantity,
            mat2Quantity;

        using (
            IDisposable ctx = this
                .Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            IInventoryRepository inventoryRepository =
                this.Services.GetRequiredService<IInventoryRepository>();
            mat1Quantity = (await inventoryRepository.GetMaterial(Materials.WaterOrb))!.Quantity;
            mat2Quantity = (await inventoryRepository.GetMaterial(Materials.StreamOrb))!.Quantity;
        }

        CharaLimitBreakResponse response = (
            await this.Client.PostMsgpack<CharaLimitBreakResponse>(
                "chara/limit_break",
                new CharaLimitBreakRequest(Charas.Celliera, 1, false)
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Celliera)
            .First();

        responseCharaData.LimitBreakCount.Should().Be(1);

        response
            .UpdateDataList.MaterialList.Should()
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
            IDisposable ctx = this
                .Services.GetRequiredService<IPlayerIdentityService>()
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

        CharaLimitBreakAndBuildupManaResponse response = (
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaResponse>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    Charas.Celliera,
                    2,
                    new List<int>() { 21, 22, 24, 25, 28 },
                    false
                )
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Celliera)
            .First();

        responseCharaData.LimitBreakCount.Should().Be(2);
        responseCharaData
            .ManaCirclePieceIdList.Should()
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
            .UpdateDataList.UserData.ManaPoint.Should()
            .Be(manaPointNum - 2800 - 3400 - 5200 - 3400 - 2800);

        response
            .UpdateDataList.MaterialList.Should()
            .ContainEquivalentOf(new MaterialList(Materials.WaterOrb, mat1Quantity - 15 - 25))
            .And.ContainEquivalentOf(new MaterialList(Materials.StreamOrb, mat2Quantity - 4 - 5))
            .And.ContainEquivalentOf(new MaterialList(Materials.DelugeOrb, mat3Quantity - 1 - 1));
    }

    [Fact]
    public async Task CharaManaNodeBuildupHasCorrectInfoOnBothSidesOfSpiral()
    {
        CharaLimitBreakAndBuildupManaResponse preSpiralResponse = (
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaResponse>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    Charas.Delphi,
                    4,
                    Enumerable.Range(1, 50).ToList(),
                    false
                )
            )
        ).Data;

        CharaList preSpiralResponseCharaData = preSpiralResponse
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Delphi)
            .First();

        CharaData charaData = MasterAsset.CharaData.Get(Charas.Delphi);

        preSpiralResponseCharaData.Level.Should().Be(1);
        preSpiralResponseCharaData.ManaCirclePieceIdList.Count().Should().Be(50);
        preSpiralResponseCharaData.AdditionalMaxLevel.Should().Be(0);
        preSpiralResponseCharaData
            .Hp.Should()
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
            .Attack.Should()
            .Be(
                charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.McFullBonusAtk5
                    + 40
            );

        await this.Client.PostMsgpack<CharaLimitBreakResponse>(
            "chara/limit_break",
            new CharaLimitBreakRequest(Charas.Delphi, 5, false)
        );

        CharaBuildupManaResponse postSpiralResponse = (
            await this.Client.PostMsgpack<CharaBuildupManaResponse>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(Charas.Delphi, Enumerable.Range(51, 20).ToList(), false)
            )
        ).Data;

        CharaList postSpiralResponseCharaData = postSpiralResponse
            .UpdateDataList.CharaList!.Where(x => (Charas)x.CharaId == Charas.Delphi)
            .First();

        postSpiralResponseCharaData.Level.Should().Be(1);
        postSpiralResponseCharaData.ManaCirclePieceIdList.Count().Should().Be(70);
        postSpiralResponseCharaData.AdditionalMaxLevel.Should().Be(20);
        postSpiralResponseCharaData
            .Hp.Should()
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
            .Attack.Should()
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
        CharaBuildupPlatinumResponse response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumResponse>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.SummerCelliera)
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => x.CharaId == Charas.SummerCelliera)
            .First();

        responseCharaData.Level.Should().Be(100);
        responseCharaData.Exp.Should().Be(CharaConstants.XpLimits[99]);
        responseCharaData.AdditionalMaxLevel.Should().Be(20);

        // Values from wiki
        responseCharaData.Hp.Should().Be(956);
        responseCharaData.Attack.Should().Be(574);

        responseCharaData.LimitBreakCount.Should().Be(5);
        responseCharaData.ManaCirclePieceIdList.Should().BeEquivalentTo(Enumerable.Range(1, 70));

        responseCharaData.Skill1Level.Should().Be(4);
        responseCharaData.Skill2Level.Should().Be(3);
        responseCharaData.Ability1Level.Should().Be(3);
        responseCharaData.Ability2Level.Should().Be(3);
        responseCharaData.Ability3Level.Should().Be(3);
    }

    [Fact]
    public async Task CharaBuildupPlatinum_ReturnsFullyBuiltWithNonSpiralledCharacter()
    {
        CharaBuildupPlatinumResponse response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumResponse>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(Charas.Harle)
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => x.CharaId == Charas.Harle)
            .First();

        responseCharaData.Level.Should().Be(80);
        responseCharaData.Exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.AdditionalMaxLevel.Should().Be(0);

        responseCharaData.Hp.Should().Be(741);
        responseCharaData.Attack.Should().Be(470);

        responseCharaData.LimitBreakCount.Should().Be(4);
        responseCharaData.ManaCirclePieceIdList.Should().BeEquivalentTo(Enumerable.Range(1, 50));

        responseCharaData.Skill1Level.Should().Be(3);
        responseCharaData.Skill2Level.Should().Be(2);
        responseCharaData.Ability1Level.Should().Be(2);
        responseCharaData.Ability2Level.Should().Be(2);
        responseCharaData.Ability3Level.Should().Be(2);
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
            await this.Client.PostMsgpack<CharaBuildupManaResponse>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(id, Enumerable.Range(1, manaNodes).ToList(), false)
            );
        }
        else
        {
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaResponse>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    id,
                    limitBreak,
                    Enumerable.Range(1, manaNodes).ToList(),
                    false
                )
            );
        }

        CharaBuildupPlatinumResponse response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumResponse>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => x.CharaId == id)
            .First();

        responseCharaData.Level.Should().Be(100);
        responseCharaData.Exp.Should().Be(CharaConstants.XpLimits[99]);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        // Values from wiki
        responseCharaData
            .Hp.Should()
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
            .Attack.Should()
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

        responseCharaData.LimitBreakCount.Should().Be(5);
        responseCharaData.ManaCirclePieceIdList.Should().BeEquivalentTo(Enumerable.Range(1, 70));

        responseCharaData.Skill1Level.Should().Be(4);
        responseCharaData.Skill2Level.Should().Be(3);
        responseCharaData.Ability1Level.Should().Be(charaData.MaxAbility1Level);
        responseCharaData.Ability2Level.Should().Be(charaData.MaxAbility2Level);
        responseCharaData.Ability3Level.Should().Be(charaData.MaxAbility3Level);
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
            await this.Client.PostMsgpack<CharaBuildupManaResponse>(
                "chara/buildup_mana",
                new CharaBuildupManaRequest(id, Enumerable.Range(1, manaNodes).ToList(), false)
            );
        }
        else
        {
            await this.Client.PostMsgpack<CharaLimitBreakAndBuildupManaResponse>(
                "chara/limit_break_and_buildup_mana",
                new CharaLimitBreakAndBuildupManaRequest(
                    id,
                    limitBreak,
                    Enumerable.Range(1, manaNodes).ToList(),
                    false
                )
            );
        }

        CharaBuildupPlatinumResponse response = (
            await this.Client.PostMsgpack<CharaBuildupPlatinumResponse>(
                "chara/buildup_platinum",
                new CharaBuildupPlatinumRequest(id)
            )
        ).Data;

        CharaList responseCharaData = response
            .UpdateDataList.CharaList!.Where(x => x.CharaId == id)
            .First();

        responseCharaData.Level.Should().Be(80);
        responseCharaData.Exp.Should().Be(CharaConstants.XpLimits[79]);
        responseCharaData.AdditionalMaxLevel.Should().Be(0);

        CharaData charaData = MasterAsset.CharaData.Get(id);

        responseCharaData
            .Hp.Should()
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
            .Attack.Should()
            .Be(
                charaData.MaxAtk
                    + charaData.PlusAtk0
                    + charaData.PlusAtk1
                    + charaData.PlusAtk2
                    + charaData.PlusAtk3
                    + charaData.PlusAtk4
                    + charaData.McFullBonusAtk5
            );

        responseCharaData.LimitBreakCount.Should().Be(4);
        responseCharaData.ManaCirclePieceIdList.Should().BeEquivalentTo(Enumerable.Range(1, 50));

        responseCharaData.Skill1Level.Should().Be(3);
        responseCharaData.Skill2Level.Should().Be(id == Charas.GalaZethia ? 0 : 2);
        responseCharaData.Ability1Level.Should().Be(charaData.MaxAbility1Level);
        responseCharaData.Ability2Level.Should().Be(charaData.MaxAbility2Level);
        responseCharaData.Ability3Level.Should().Be(charaData.MaxAbility3Level);
    }

    [Fact]
    public async Task CharaGetCharaUnitSet_ReturnsData()
    {
        CharaGetCharaUnitSetResponse response = (
            await this.Client.PostMsgpack<CharaGetCharaUnitSetResponse>(
                "chara/get_chara_unit_set",
                new CharaGetCharaUnitSetRequest(new List<Charas>() { Charas.Celliera })
            )
        ).Data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task CharaSetCharaUnitSet_ReturnsCorrectSetData()
    {
        CharaSetCharaUnitSetResponse response = (
            await this.Client.PostMsgpack<CharaSetCharaUnitSetResponse>(
                "chara/set_chara_unit_set",
                new CharaSetCharaUnitSetRequest(
                    1,
                    "Exercise",
                    Charas.Celliera,
                    new AtgenRequestCharaUnitSetData() { DragonKeyId = 5 }
                )
            )
        ).Data;

        CharaUnitSetList responseCharaData = response
            .UpdateDataList.CharaUnitSetList!.Where(x => (Charas)x.CharaId == Charas.Celliera)
            .First();
        responseCharaData.CharaUnitSetDetailList.ToList()[0].DragonKeyId.Should().Be(5);
    }
}
