using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.Test.Unit;

public class MasterAssetTest
{
    [Fact]
    public void CharaData_Get_ReturnsExpectedProperties()
    {
        CharaData laxi = MasterAsset.MasterAsset.CharaData.Get(Charas.GalaLaxi);

        laxi.Should()
            .BeEquivalentTo(
                new CharaData(
                    Id: Charas.GalaLaxi,
                    WeaponType: WeaponTypes.Dagger,
                    IsPlayable: true,
                    Rarity: 5,
                    MaxLimitBreakCount: 5,
                    ElementalType: UnitElement.Fire,
                    MinHp3: 44,
                    MinHp4: 55,
                    MinHp5: 64,
                    MaxHp: 454,
                    AddMaxHp1: 544,
                    PlusHp0: 53,
                    PlusHp1: 61,
                    PlusHp2: 72,
                    PlusHp3: 61,
                    PlusHp4: 30,
                    PlusHp5: 60,
                    McFullBonusHp5: 27,
                    MinAtk3: 29,
                    MinAtk4: 36,
                    MinAtk5: 42,
                    MaxAtk: 299,
                    AddMaxAtk1: 358,
                    PlusAtk0: 35,
                    PlusAtk1: 40,
                    PlusAtk2: 48,
                    PlusAtk3: 40,
                    PlusAtk4: 20,
                    PlusAtk5: 40,
                    McFullBonusAtk5: 18,
                    Skill1: 103501021,
                    Skill2: 103501022,
                    HoldEditSkillCost: 10,
                    EditSkillId: 103501021,
                    EditSkillLevelNum: 1,
                    EditSkillCost: 5,
                    ManaCircleName: "MC_0511"
                )
            );
    }

    [Fact]
    public void CharaData_GetManaNode_ReturnsNode()
    {
        CharaData euden = MasterAsset.MasterAsset.CharaData.Get(Charas.ThePrince);

        euden
            .GetManaNode(2)
            .Should()
            .BeEquivalentTo(
                new ManaNode(
                    ManaPieceType: ManaNodeTypes.Atk,
                    IsReleaseStory: false,
                    NecessaryManaPoint: 400,
                    UniqueGrowMaterialCount1: 0,
                    UniqueGrowMaterialCount2: 0,
                    MC_0: 4032,
                    ManaCircleName: "MC_0403"
                )
            );
    }

    [Theory]
    [InlineData(Charas.GalaElisanne, 70)]
    [InlineData(Charas.SummerLeonidas, 50)]
    public void CharaData_GetManaNodes_HasExpectedCount(Charas id, int expectedCount)
    {
        CharaData chara = MasterAsset.MasterAsset.CharaData.Get(id);

        chara.GetManaNodes().Should().HaveCount(expectedCount);
    }

    [Theory]
    [InlineData(Charas.Elisanne, CharaAvailabilities.Story)]
    [InlineData(Charas.Annelie, CharaAvailabilities.Default)]
    [InlineData(Charas.Chelle, CharaAvailabilities.Story)]
    public void CharaData_Availability_ReturnsExpectedResult(
        Charas id,
        CharaAvailabilities expected
    )
    {
        CharaData chara = MasterAsset.MasterAsset.CharaData.Get(id);

        chara.Availability.Should().Be(expected);
    }

    [Fact]
    public void DragonData_Get_ReturnsExpectedProperties()
    {
        DragonData elysium = MasterAsset.MasterAsset.DragonData.Get(Dragons.GalaElysium);

        elysium
            .Should()
            .BeEquivalentTo(
                new DragonData(
                    Id: Dragons.GalaElysium,
                    Rarity: 5,
                    ElementalType: UnitElement.Light,
                    MaxLimitBreakCount: 4,
                    IsPlayable: true,
                    MinHp: 37,
                    MaxHp: 371,
                    AddMaxHp1: 0,
                    MinAtk: 12,
                    MaxAtk: 124,
                    AddMaxAtk1: 0,
                    Skill1: 200504191,
                    Skill2: 0,
                    Abilities11: 3000,
                    Abilities12: 3001,
                    Abilities13: 3002,
                    Abilities14: 3003,
                    Abilities15: 3004,
                    Abilities16: 0,
                    Abilities21: 3100,
                    Abilities22: 3100,
                    Abilities23: 3100,
                    Abilities24: 3100,
                    Abilities25: 3101,
                    Abilities26: 0,
                    FavoriteType: 5,
                    SellCoin: 5000,
                    SellDewPoint: 8500,
                    LimitBreakMaterialId: 0,
                    DefaultReliabilityLevel: 0,
                    DmodePassiveAbilityId: 0
                )
            );
    }

    [Fact]
    public void ManaNode_Get_ReturnsExpectedProperties()
    {
        ManaNode node = MasterAsset.MasterAsset.ManaNode.Get(5045);

        node.Should()
            .BeEquivalentTo(
                new ManaNode(
                    ManaPieceType: ManaNodeTypes.Hp,
                    IsReleaseStory: true,
                    NecessaryManaPoint: 450,
                    UniqueGrowMaterialCount1: 0,
                    UniqueGrowMaterialCount2: 0,
                    MC_0: 5045,
                    ManaCircleName: "MC_0504"
                )
            );
    }

    [Fact]
    public void QuestData_Get_ReturnsExpectedProperties()
    {
        QuestData quest = MasterAsset.MasterAsset.QuestData.Get(100010101);

        quest
            .Should()
            .BeEquivalentTo(
                new QuestData(
                    Id: 100010101,
                    QuestPlayModeType: QuestPlayModeTypes.Normal,
                    LimitedElementalType: 0,
                    LimitedElementalType2: 0,
                    LimitedWeaponTypePatternId: 0,
                    PayStaminaSingle: 2,
                    PayStaminaMulti: 1,
                    DungeonType: DungeonTypes.Normal,
                    Scene01: "Main/01/MAIN_01_0101_01",
                    AreaName01: "MAIN_01_0101_01",
                    Scene02: "Boss/BG001_5001_00/BG001_5001_00_00",
                    AreaName02: "MAIN_01_0101_02",
                    Scene03: "",
                    AreaName03: "",
                    Scene04: "",
                    AreaName04: "",
                    Scene05: "",
                    AreaName05: "",
                    Scene06: "",
                    AreaName06: ""
                )
            );

        quest.AreaInfo
            .Should()
            .BeEquivalentTo(
                new List<AreaInfo>()
                {
                    new("Main/01/MAIN_01_0101_01", "MAIN_01_0101_01"),
                    new("Boss/BG001_5001_00/BG001_5001_00_00", "MAIN_01_0101_02")
                }
            );
    }
}
