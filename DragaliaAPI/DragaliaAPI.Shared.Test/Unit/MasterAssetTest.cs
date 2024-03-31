using System.Collections.Frozen;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;

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
                    ManaCircleName: "MC_0511",
                    CharaLimitBreak: 1050201,
                    PieceElementGroupId: 502,
                    PieceMaterialElementId: 5091,
                    AwakeNeedEntityType4: EntityTypes.Dew,
                    AwakeNeedEntityId4: 0,
                    AwakeNeedEntityQuantity4: 2500,
                    AwakeNeedEntityType5: EntityTypes.Dew,
                    AwakeNeedEntityId5: 0,
                    AwakeNeedEntityQuantity5: 25000,
                    GrowMaterialOnlyStartDate: DateTimeOffset.UnixEpoch,
                    GrowMaterialOnlyEndDate: DateTimeOffset.UnixEpoch,
                    UniqueGrowMaterialId1: Materials.GalaLaxisConviction,
                    UniqueGrowMaterialId2: Materials.GalaLaxisDevotion,
                    GrowMaterialId: Materials.Empty,
                    DefaultAbility1Level: 1,
                    DefaultAbility2Level: 0,
                    DefaultAbility3Level: 0,
                    DefaultBurstAttackLevel: 0,
                    Abilities11: 1063,
                    Abilities12: 1064,
                    Abilities13: 2030,
                    Abilities14: 0,
                    Abilities21: 1077,
                    Abilities22: 1078,
                    Abilities23: 2037,
                    Abilities24: 0,
                    Abilities31: 1071,
                    Abilities32: 1074,
                    Abilities33: 2041,
                    Abilities34: 0,
                    MinDef: 10,
                    ExAbilityData1: 106070004,
                    ExAbilityData2: 106070005,
                    ExAbilityData3: 106070006,
                    ExAbilityData4: 106070007,
                    ExAbilityData5: 106070008,
                    ExAbility2Data1: 400000735,
                    ExAbility2Data2: 400000736,
                    ExAbility2Data3: 400000737,
                    ExAbility2Data4: 400000738,
                    ExAbility2Data5: 400000740,
                    EditReleaseEntityType1: EntityTypes.Material,
                    EditReleaseEntityId1: 201019011,
                    EditReleaseEntityQuantity1: 5,
                    BaseId: 100032,
                    VariationId: 4
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
                    GrowMaterialCount: 1,
                    MC_0: 4032,
                    Step: 1,
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
    [InlineData(Charas.ThePrince, 3, 3, 3)]
    [InlineData(Charas.Harle, 2, 2, 2)]
    [InlineData(Charas.Celliera, 3, 3, 2)]
    public void CharaData_MaxAbilityLevels_HaveExpectedCounts(
        Charas id,
        int expectedAbility1Level,
        int expectedAbility2Level,
        int expectedAbility3Level
    )
    {
        CharaData chara = MasterAsset.MasterAsset.CharaData.Get(id);

        chara.MaxAbility1Level.Should().Be(expectedAbility1Level);
        chara.MaxAbility2Level.Should().Be(expectedAbility2Level);
        chara.MaxAbility3Level.Should().Be(expectedAbility3Level);
    }

    [Theory]
    [InlineData(Charas.Elisanne, UnitAvailability.Story)]
    [InlineData(Charas.Annelie, UnitAvailability.Permanent)]
    [InlineData(Charas.Chelle, UnitAvailability.Story)]
    public void CharaData_Availability_ReturnsExpectedResult(Charas id, UnitAvailability expected)
    {
        CharaData chara = MasterAsset.MasterAsset.CharaData.Get(id);

        chara.GetAvailability().Should().Be(expected);
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
                    LimitBreakId: DragonLimitBreakTypes.Normal,
                    LimitBreakMaterialId: 0,
                    DefaultReliabilityLevel: 0,
                    DmodePassiveAbilityId: 0,
                    BaseId: 210173,
                    VariationId: 1
                )
            );
    }

    [Fact]
    public void ManaNode_Get_ReturnsExpectedProperties()
    {
        ManaNode node = MasterAsset.MasterAsset.MC.Get(5045);

        node.Should()
            .BeEquivalentTo(
                new ManaNode(
                    ManaPieceType: ManaNodeTypes.Hp,
                    IsReleaseStory: true,
                    NecessaryManaPoint: 450,
                    UniqueGrowMaterialCount1: 0,
                    UniqueGrowMaterialCount2: 0,
                    GrowMaterialCount: 1,
                    MC_0: 5045,
                    Step: 3,
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
                    Gid: 10001,
                    GroupType: QuestGroupType.MainStory,
                    QuestPlayModeType: QuestPlayModeTypes.Normal,
                    LimitedElementalType: 0,
                    LimitedElementalType2: 0,
                    LimitedWeaponTypePatternId: 0,
                    IsPayForceStaminaSingle: false,
                    PayStaminaSingle: 2,
                    CampaignStaminaSingle: 1,
                    PayStaminaMulti: 1,
                    CampaignStaminaMulti: 1,
                    DungeonType: DungeonTypes.Normal,
                    VariationType: VariationTypes.Normal,
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
                    AreaName06: "",
                    ContinueLimit: 3,
                    RebornLimit: 3,
                    Difficulty: 500,
                    PayEntityTargetType: PayTargetType.None,
                    PayEntityType: 0,
                    PayEntityId: 0,
                    PayEntityQuantity: 0,
                    HoldEntityType: EntityTypes.None,
                    HoldEntityId: 0,
                    HoldEntityQuantity: 0,
                    IsSumUpTotalDamage: false
                )
            );

        quest
            .AreaInfo.Should()
            .BeEquivalentTo(
                new List<AreaInfo>()
                {
                    new("Main/01/MAIN_01_0101_01", "MAIN_01_0101_01"),
                    new("Boss/BG001_5001_00/BG001_5001_00_00", "MAIN_01_0101_02")
                }
            );
    }

    [Fact]
    public void FortPlant_Get_ReturnsExpectedProperties()
    {
        FortPlantDetail fortPlant = MasterAssetUtils.GetFortPlant(FortPlants.FlameTree, 6);

        fortPlant
            .Should()
            .BeEquivalentTo(
                new FortPlantDetail(
                    Id: 10230106,
                    AssetGroup: FortPlants.FlameTree,
                    Level: 6,
                    NextAssetGroup: 10230107,
                    LevelType: 1,
                    NeedLevel: 1,
                    Time: 43200,
                    Cost: 50000,
                    MaterialsId1: Materials.DestituteOnesMaskFragment,
                    MaterialsNum1: 30,
                    MaterialsId2: 0,
                    MaterialsNum2: 0,
                    MaterialsId3: 0,
                    MaterialsNum3: 0,
                    MaterialsId4: 0,
                    MaterialsNum4: 0,
                    MaterialsId5: 0,
                    MaterialsNum5: 0,
                    EffectId: FortEffectTypes.Element,
                    EffType1: 1,
                    EffType2: 0,
                    EffArgs1: 7.0f,
                    EffArgs2: 6.5f,
                    CostMaxTime: 0,
                    CostMax: 0,
                    MaterialMaxTime: 0,
                    MaterialMax: 0,
                    StaminaMaxTime: 0,
                    StaminaMax: 0,
                    EventEffectType: 0,
                    EventEffectArgs: 0.0f,
                    Odds: ""
                )
            );
    }

    [Fact]
    public void WeaponBody_Get_ReturnsExpectedProperties()
    {
        WeaponBody body = MasterAsset.MasterAsset.WeaponBody.Get(WeaponBodies.EndlessAzureCore);

        body.Should()
            .BeEquivalentTo(
                new WeaponBody(
                    Id: WeaponBodies.EndlessAzureCore,
                    WeaponSeriesId: WeaponSeries.Void,
                    WeaponType: WeaponTypes.Cane,
                    Rarity: 5,
                    ElementalType: UnitElement.Water,
                    MaxLimitOverCount: 1,
                    WeaponPassiveAbilityGroupId: 10802,
                    WeaponBodyBuildupGroupId: 572,
                    MaxWeaponPassiveCharaCount: 1,
                    WeaponPassiveEffHp: 0.5f,
                    WeaponPassiveEffAtk: 0.5f,
                    Abilities11: 137,
                    Abilities12: 137,
                    Abilities13: 137,
                    Abilities21: 0,
                    Abilities22: 0,
                    Abilities23: 0,
                    ChangeSkillId1: 0,
                    ChangeSkillId2: 0,
                    ChangeSkillId3: 0,
                    RewardWeaponSkinId1: 0,
                    RewardWeaponSkinId2: 0,
                    RewardWeaponSkinId3: 0,
                    RewardWeaponSkinId4: 0,
                    RewardWeaponSkinId5: 0,
                    NeedFortCraftLevel: 7,
                    NeedCreateWeaponBodyId1: 0,
                    NeedCreateWeaponBodyId2: 0,
                    CreateCoin: 80000,
                    CreateEntityId1: Materials.BatsWing,
                    CreateEntityQuantity1: 430,
                    CreateEntityId2: Materials.SolidFungus,
                    CreateEntityQuantity2: 35,
                    CreateEntityId3: Materials.ShinySpore,
                    CreateEntityQuantity3: 1,
                    CreateEntityId4: Materials.StreamOrb,
                    CreateEntityQuantity4: 8,
                    CreateEntityId5: 0,
                    CreateEntityQuantity5: 0,
                    LimitOverCountPartyPower1: 100,
                    LimitOverCountPartyPower2: 150,
                    BaseHp: 45,
                    MaxHp1: 151,
                    MaxHp2: 216,
                    MaxHp3: 0,
                    BaseAtk: 97,
                    MaxAtk1: 324,
                    MaxAtk2: 590,
                    MaxAtk3: 0
                )
            );
    }

    [Fact]
    public void WeaponBodyBuildupGroup_Get_ReturnsExpectedProperties()
    {
        int key = MasterAsset
            .MasterAsset.WeaponBody[WeaponBodies.Marmyadose]
            .GetBuildupGroupId(BuildupPieceTypes.Refine, 2);

        WeaponBodyBuildupGroup group = MasterAsset.MasterAsset.WeaponBodyBuildupGroup[key];

        group
            .Should()
            .BeEquivalentTo(
                new WeaponBodyBuildupGroup(
                    Id: 8030202,
                    WeaponBodyBuildupGroupId: 803,
                    BuildupPieceType: BuildupPieceTypes.Refine,
                    Step: 2,
                    UnlockConditionLimitBreakCount: 0,
                    RewardWeaponSkinNo: 1,
                    BuildupCoin: 2_500_000,
                    BuildupMaterialId1: Materials.EliminatingOnesMaskFragment,
                    BuildupMaterialQuantity1: 40,
                    BuildupMaterialId2: Materials.DespairingOnesMaskFragment,
                    BuildupMaterialQuantity2: 30,
                    BuildupMaterialId3: Materials.RebelliousOnesDesperation,
                    BuildupMaterialQuantity3: 10,
                    BuildupMaterialId4: Materials.RebelliousBirdsTide,
                    BuildupMaterialQuantity4: 10,
                    BuildupMaterialId5: Materials.Orichalcum,
                    BuildupMaterialQuantity5: 10,
                    BuildupMaterialId6: Materials.Empty,
                    BuildupMaterialQuantity6: 0,
                    BuildupMaterialId7: Materials.Empty,
                    BuildupMaterialQuantity7: 0
                )
            );
    }

    [Fact]
    public void WeaponBodyBuildupLevel_Get_ReturnsExpectedProperties()
    {
        int key = MasterAsset.MasterAsset.WeaponBody[WeaponBodies.Camelot].GetBuildupLevelId(40);

        MasterAsset
            .MasterAsset.WeaponBodyBuildupLevel[key]
            .Should()
            .BeEquivalentTo(
                new WeaponBodyBuildupLevel(
                    Id: 601040,
                    RarityGroup: 6,
                    Level: 40,
                    BuildupMaterialId1: Materials.BronzeWhetstone,
                    BuildupMaterialQuantity1: 5,
                    BuildupMaterialId2: Materials.GoldWhetstone,
                    BuildupMaterialQuantity2: 5,
                    BuildupMaterialId3: Materials.Empty,
                    BuildupMaterialQuantity3: 0
                )
            );
    }

    [Fact]
    public void WeaponPassiveAbility_Get_ReturnsExpectedProperties()
    {
        int key = MasterAsset
            .MasterAsset.WeaponBody[WeaponBodies.InfernoApogee]
            .GetPassiveAbilityId(1);

        MasterAsset
            .MasterAsset.WeaponPassiveAbility[key]
            .Should()
            .BeEquivalentTo(
                new WeaponPassiveAbility(
                    Id: 1010101,
                    WeaponPassiveAbilityGroupId: 10101,
                    WeaponPassiveAbilityNo: 1,
                    WeaponType: WeaponTypes.Sword,
                    ElementalType: UnitElement.Fire,
                    UnlockConditionLimitBreakCount: 1,
                    RewardWeaponSkinId1: 30140105,
                    RewardWeaponSkinId2: 0,
                    UnlockCoin: 80_000,
                    UnlockMaterialId1: Materials.Granite,
                    UnlockMaterialQuantity1: 80,
                    UnlockMaterialId2: Materials.OldCloth,
                    UnlockMaterialQuantity2: 30,
                    UnlockMaterialId3: Materials.FloatingYellowCloth,
                    UnlockMaterialQuantity3: 7,
                    UnlockMaterialId4: Materials.UnearthlyLantern,
                    UnlockMaterialQuantity4: 1,
                    UnlockMaterialId5: Materials.BlazeOrb,
                    UnlockMaterialQuantity5: 8,
                    AbilityId: 389
                )
            );
    }

    [Theory]
    [InlineData(Charas.Celliera, 110255011, 110255012, 110255013, 110255014, 110255015)]
    [InlineData(Charas.SummerCelliera, 110255021, 110255022, 110255023, 110255024, 110255025)]
    public void CharaStories_ReturnsExpectedStoryIds(Charas chara, params int[] expectedStoryIds)
    {
        int key = MasterAsset.MasterAsset.CharaStories[(int)chara].Id;

        MasterAsset
            .MasterAsset.CharaStories[key]
            .StoryIds.Should()
            .ContainInConsecutiveOrder(expectedStoryIds);
    }

    [Theory]
    [InlineData(Dragons.Garuda, 210036011, 210036012)]
    [InlineData(Dragons.Liger, 210043011, 210043012)]
    public void DragonStories_ReturnsExpectedStoryIds(Dragons dragon, params int[] expectedStoryIds)
    {
        int key = MasterAsset.MasterAsset.DragonStories[(int)dragon].Id;

        MasterAsset
            .MasterAsset.DragonStories[key]
            .StoryIds.Should()
            .ContainInConsecutiveOrder(expectedStoryIds);
    }

    [Fact]
    public void StoryData_HasExpectedProperties()
    {
        MasterAsset
            .MasterAsset.UnitStory[200010011]
            .Should()
            .BeEquivalentTo(
                new UnitStory(
                    Id: 200010011,
                    ReleaseTriggerId: (int)Dragons.Chthonius,
                    UnlockQuestStoryId: 0,
                    UnlockTriggerStoryId: 0
                )
            );
    }

    [Theory]
    [InlineData(100001141, StoryTypes.Chara)]
    [InlineData(210001011, StoryTypes.Dragon)]
    public void StoryData_Type_IsCorrect(int storyId, StoryTypes expectedType)
    {
        MasterAsset.MasterAsset.UnitStory[storyId].Type.Should().Be(expectedType);
    }

    [Fact]
    public void AbilityCrestTrade_Get_ReturnsExpectedProperties()
    {
        AbilityCrestTrade abilityCrestTrade = MasterAsset.MasterAsset.AbilityCrestTrade.Get(5101);

        abilityCrestTrade
            .Should()
            .BeEquivalentTo(
                new
                {
                    Id = 5101,
                    AbilityCrestId = AbilityCrests.SweetSurprise,
                    NeedDewPoint = 4000,
                    Priority = 5199
                }
            );
    }

    [Fact]
    public void AbilityCrestBuildupGroup_Get_ReturnsExpectedProperties()
    {
        AbilityCrestBuildupGroup buildupGroup =
            MasterAsset.MasterAsset.AbilityCrestBuildupGroup.Get(6020603);

        buildupGroup
            .Should()
            .BeEquivalentTo(
                new AbilityCrestBuildupGroup(
                    Id: 6020603,
                    AbilityCrestBuildupGroupId: 602,
                    BuildupPieceType: BuildupPieceTypes.Copies,
                    Step: 3,
                    BuildupDewPoint: 0,
                    BuildupMaterialId1: Materials.AzureInsignia,
                    BuildupMaterialQuantity1: 200,
                    BuildupMaterialId2: Materials.DyrenellAureus,
                    BuildupMaterialQuantity2: 25,
                    BuildupMaterialId3: 0,
                    BuildupMaterialQuantity3: 0,
                    UniqueBuildupMaterialCount: 0
                )
            );
    }

    [Fact]
    public void AbilityCrestBuildupGroup_MaterialMap_ReturnsExpectedDictionary()
    {
        FrozenDictionary<Materials, int> map = MasterAsset
            .MasterAsset.AbilityCrestBuildupGroup.Get(6020603)
            .MaterialMap;

        map.Should()
            .BeEquivalentTo(
                new Dictionary<Materials, int>()
                {
                    { Materials.AzureInsignia, 200 },
                    { Materials.DyrenellAureus, 25 }
                }
            );
    }

    [Theory]
    [InlineData(6040104, false)]
    [InlineData(11030602, true)]
    public void AbilityCrestBuildupGroup_IsUseUniqueMaterial_ReturnsExpectedBool(
        int id,
        bool expected
    )
    {
        bool actual = MasterAsset.MasterAsset.AbilityCrestBuildupGroup.Get(id).IsUseUniqueMaterial;

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(6020603, false)]
    [InlineData(5010101, true)]
    public void AbilityCrestBuildupGroup_IsUseDewpoint_ReturnsExpectedBool(int id, bool expected)
    {
        bool actual = MasterAsset.MasterAsset.AbilityCrestBuildupGroup.Get(id).IsUseDewpoint;

        actual.Should().Be(expected);
    }

    [Fact]
    public void AbilityCrestBuildupLevel_Get_ReturnsExpectedProperties()
    {
        AbilityCrestBuildupLevel buildupLevel =
            MasterAsset.MasterAsset.AbilityCrestBuildupLevel.Get(901010);

        buildupLevel
            .Should()
            .BeEquivalentTo(
                new AbilityCrestBuildupLevel(
                    Id: 901010,
                    RarityGroup: 901,
                    Level: 10,
                    BuildupMaterialId1: Materials.HolyWater,
                    BuildupMaterialQuantity1: 2,
                    BuildupMaterialId2: Materials.ConsecratedWater,
                    BuildupMaterialQuantity2: 14,
                    BuildupMaterialId3: 0,
                    BuildupMaterialQuantity3: 0,
                    UniqueBuildupMaterialCount: 2
                )
            );
    }

    [Fact]
    public void AbilityCrestBuildupLevel_MaterialMap_ReturnsExpectedDictionary()
    {
        FrozenDictionary<Materials, int> map = MasterAsset
            .MasterAsset.AbilityCrestBuildupLevel.Get(901010)
            .MaterialMap;

        map.Should()
            .BeEquivalentTo(
                new Dictionary<Materials, int>()
                {
                    { Materials.HolyWater, 2 },
                    { Materials.ConsecratedWater, 14 }
                }
            );
    }

    [Theory]
    [InlineData(901010, true)]
    [InlineData(301013, false)]
    public void AbilityCrestBuildupLevel_IsUseUniqueMaterial_ReturnsExpectedBool(
        int id,
        bool expected
    )
    {
        bool actual = MasterAsset.MasterAsset.AbilityCrestBuildupLevel.Get(id).IsUseUniqueMaterial;

        actual.Should().Be(expected);
    }

    [Fact]
    public void AbilityCrestRarity_Get_ReturnsExpectedProperties()
    {
        AbilityCrestRarity rarity = MasterAsset.MasterAsset.AbilityCrestRarity.Get(9);

        rarity
            .Should()
            .BeEquivalentTo(
                new AbilityCrestRarity(
                    Id: 9,
                    MaxLimitLevelByLimitBreak0: 10,
                    MaxLimitLevelByLimitBreak1: 15,
                    MaxLimitLevelByLimitBreak2: 20,
                    MaxLimitLevelByLimitBreak3: 25,
                    MaxLimitLevelByLimitBreak4: 30,
                    MaxHpPlusCount: 40,
                    MaxAtkPlusCount: 40
                )
            );
    }

    [Fact]
    public void AbilityCrest_Get_ReturnsExpectedProperties()
    {
        AbilityCrest abilityCrest = MasterAsset.MasterAsset.AbilityCrest.Get(
            AbilityCrests.TheGeniusTacticianBowsBoon
        );

        abilityCrest
            .Should()
            .BeEquivalentTo(
                new AbilityCrest(
                    Id: AbilityCrests.TheGeniusTacticianBowsBoon,
                    AbilityCrestBuildupGroupId: 1101,
                    AbilityCrestLevelRarityGroupId: 901,
                    Rarity: 9,
                    UniqueBuildupMaterialId: Materials.GeniusoftheCenturysMemory,
                    DuplicateEntityId: Materials.GeniusoftheCenturysMemory,
                    DuplicateEntityQuantity: 6,
                    DuplicateEntityType: EntityTypes.Material,
                    Abilities11: 2338,
                    Abilities12: 2339,
                    Abilities13: 2340,
                    Abilities21: 0,
                    Abilities22: 0,
                    Abilities23: 0,
                    BaseHp: 14,
                    MaxHp: 44,
                    BaseAtk: 5,
                    MaxAtk: 25,
                    UnionAbilityGroupId: 4,
                    BaseId: 400192,
                    IsHideChangeImage: true
                )
            );
    }

    [Fact]
    public void AbilityCrest_GetBuildupGroupId_ReturnsExpectedId()
    {
        AbilityCrest abilityCrest = MasterAsset.MasterAsset.AbilityCrest.Get(
            AbilityCrests.TheGeniusTacticianBowsBoon
        );

        int buildupGroupId = abilityCrest.GetBuildupGroupId(BuildupPieceTypes.Copies, 4);
        buildupGroupId.Should().Be(11010604);
    }

    [Fact]
    public void AbilityCrest_GetBuildupLevelId_ReturnsExpectedId()
    {
        AbilityCrest abilityCrest = MasterAsset.MasterAsset.AbilityCrest.Get(
            AbilityCrests.TheGeniusTacticianBowsBoon
        );

        int buildupLevelId = abilityCrest.GetBuildupLevelId(9);
        buildupLevelId.Should().Be(901009);
    }

    [Fact]
    public void AbilityCrest_DuplicateMaterialMap_ReturnsExpectedDictionary()
    {
        FrozenDictionary<Materials, int> map = MasterAsset
            .MasterAsset.AbilityCrest.Get(AbilityCrests.TheGeniusTacticianBowsBoon)
            .DuplicateMaterialMap;

        map.Should()
            .BeEquivalentTo(
                new Dictionary<Materials, int>() { { Materials.GeniusoftheCenturysMemory, 6 } }
            );
    }

    [Fact]
    public void AbilityCrest_GetAbilities_ReturnsExpected()
    {
        AbilityCrest crest = MasterAsset.MasterAsset.AbilityCrest[AbilityCrests.TotheExtreme];

        crest
            .GetAbilities(1)
            .Should()
            .BeEquivalentTo(new List<int>() { crest.Abilities11, crest.Abilities21 });

        crest
            .GetAbilities(2)
            .Should()
            .BeEquivalentTo(new List<int>() { crest.Abilities12, crest.Abilities22 });

        crest
            .GetAbilities(3)
            .Should()
            .BeEquivalentTo(new List<int>() { crest.Abilities13, crest.Abilities23 });
    }
}
