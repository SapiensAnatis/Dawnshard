using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using Xunit.Abstractions;

namespace DragaliaAPI.Test.Services;

public class AbilityCrestServiceTest
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly AbilityCrestService abilityCrestService;

    private static readonly Dictionary<Materials, int> EmptyMap = new();
    private static readonly Dictionary<Materials, int> SilverKey =
        new() { { Materials.SilverKey, 1 } };
    private static readonly Dictionary<Materials, int> GoldenKey =
        new() { { Materials.GoldenKey, 1 } };
    private static readonly Dictionary<Materials, int> GreatwyrmMidgardsormrUnbind1Map =
        new() { { Materials.JadeInsignia, 20 } };
    private static readonly Dictionary<Materials, int> GloriousTempestUnbind1Map =
        new() { { Materials.JadeInsignia, 100 }, { Materials.RoyalJadeInsignia, 40 } };
    private static readonly Dictionary<Materials, int> TutelaryUnbind1Map =
        new() { { Materials.TwilightShard, 50 }, { Materials.TutelarySuccessorsMemory, 20 } };
    private static readonly Dictionary<Materials, int> GreatwyrmMidgardsormrUnbind4Map =
        new() { { Materials.JadeInsignia, 40 }, { Materials.DyrenellAureus, 5 } };
    private static readonly Dictionary<Materials, int> GloriousTempestUnbind4Map =
        new() { { Materials.JadeInsignia, 100 }, { Materials.RoyalJadeInsignia, 40 } };
    private static readonly Dictionary<Materials, int> TutelaryUnbind4Map =
        new()
        {
            { Materials.TwilightShard, 150 },
            { Materials.TwilightPrism, 150 },
            { Materials.TutelarySuccessorsMemory, 40 }
        };

    public static IEnumerable<object[]> SuccessfulUnbindData =>
        new List<object[]>
        {
            new object[] { AbilityCrests.ManaFount, 1, false, EmptyMap, 10 },
            new object[] { AbilityCrests.TheOrdersMessengerOwl, 1, false, EmptyMap, 30 },
            new object[] { AbilityCrests.DragonsNest, 1, false, EmptyMap, 300 },
            new object[] { AbilityCrests.TheBewitchingMagician, 1, false, EmptyMap, 150 },
            new object[] { AbilityCrests.HisCleverBrother, 1, false, EmptyMap, 3_000 },
            new object[]
            {
                AbilityCrests.GreatwyrmMidgardsormr,
                1,
                false,
                GreatwyrmMidgardsormrUnbind1Map,
                0
            },
            new object[] { AbilityCrests.UnitedbyOneVision, 1, false, EmptyMap, 300 },
            new object[] { AbilityCrests.WorthyRivals, 1, false, EmptyMap, 6_000 },
            new object[] { AbilityCrests.GloriousTempest, 1, false, GloriousTempestUnbind1Map, 0 },
            new object[]
            {
                AbilityCrests.TutelarysDestinyWolfsBoon,
                1,
                false,
                TutelaryUnbind1Map,
                0
            },
            new object[] { AbilityCrests.TheBewitchingMagician, 1, true, SilverKey, 0 },
            new object[] { AbilityCrests.HisCleverBrother, 1, true, SilverKey, 0 },
            new object[] { AbilityCrests.GreatwyrmMidgardsormr, 1, true, SilverKey, 0 },
            new object[] { AbilityCrests.UnitedbyOneVision, 1, true, GoldenKey, 0 },
            new object[] { AbilityCrests.WorthyRivals, 1, true, GoldenKey, 0 },
            new object[] { AbilityCrests.GloriousTempest, 1, true, GoldenKey, 0 },
            new object[] { AbilityCrests.ManaFount, 4, false, EmptyMap, 10 },
            new object[] { AbilityCrests.TheOrdersMessengerOwl, 4, false, EmptyMap, 40 },
            new object[] { AbilityCrests.DragonsNest, 4, false, EmptyMap, 400 },
            new object[] { AbilityCrests.TheBewitchingMagician, 4, false, EmptyMap, 200 },
            new object[] { AbilityCrests.HisCleverBrother, 4, false, EmptyMap, 4_000 },
            new object[]
            {
                AbilityCrests.GreatwyrmMidgardsormr,
                4,
                false,
                GreatwyrmMidgardsormrUnbind4Map,
                0
            },
            new object[] { AbilityCrests.UnitedbyOneVision, 4, false, EmptyMap, 400 },
            new object[] { AbilityCrests.WorthyRivals, 4, false, EmptyMap, 9_000 },
            new object[] { AbilityCrests.GloriousTempest, 4, false, GloriousTempestUnbind4Map, 0 },
            new object[]
            {
                AbilityCrests.TutelarysDestinyWolfsBoon,
                4,
                false,
                TutelaryUnbind4Map,
                0
            },
            new object[] { AbilityCrests.TheBewitchingMagician, 4, true, SilverKey, 0 },
            new object[] { AbilityCrests.HisCleverBrother, 4, true, SilverKey, 0 },
            new object[] { AbilityCrests.GreatwyrmMidgardsormr, 4, true, SilverKey, 0 },
            new object[] { AbilityCrests.UnitedbyOneVision, 4, true, GoldenKey, 0 },
            new object[] { AbilityCrests.WorthyRivals, 4, true, GoldenKey, 0 },
            new object[] { AbilityCrests.GloriousTempest, 4, true, GoldenKey, 0 }
        };

    private static readonly Dictionary<Materials, int> GreatwyrmMidgardsormrCopyMap =
        new() { { Materials.JadeInsignia, 200 }, { Materials.DyrenellAureus, 25 } };
    private static readonly Dictionary<Materials, int> GloriousTempestCopyMap =
        new() { { Materials.JadeInsignia, 500 }, { Materials.RoyalJadeInsignia, 200 } };
    private static readonly Dictionary<Materials, int> TutelarysDestinyCopyMap =
        new()
        {
            { Materials.TwilightShard, 200 },
            { Materials.TwilightPrism, 150 },
            { Materials.TutelarySuccessorsMemory, 120 }
        };

    public static IEnumerable<object[]> SuccessfulCopiesData =>
        new List<object[]>
        {
            new object[] { AbilityCrests.ManaFount, EmptyMap, 10 },
            new object[] { AbilityCrests.TheOrdersMessengerOwl, EmptyMap, 170 },
            new object[] { AbilityCrests.DragonsNest, EmptyMap, 1_700 },
            new object[] { AbilityCrests.TheBewitchingMagician, EmptyMap, 850 },
            new object[] { AbilityCrests.HisCleverBrother, EmptyMap, 17_000 },
            new object[] { AbilityCrests.GreatwyrmMidgardsormr, GreatwyrmMidgardsormrCopyMap, 0 },
            new object[] { AbilityCrests.UnitedbyOneVision, EmptyMap, 1_700 },
            new object[] { AbilityCrests.WorthyRivals, EmptyMap, 37_000 },
            new object[] { AbilityCrests.GloriousTempest, GloriousTempestCopyMap, 0 },
            new object[] { AbilityCrests.TutelarysDestinyWolfsBoon, TutelarysDestinyCopyMap, 0 }
        };

    private static readonly Dictionary<Materials, int> Rarity2LevelMap =
        new() { { Materials.HolyWater, 7 } };
    private static readonly Dictionary<Materials, int> Rarity3LevelMap =
        new() { { Materials.HolyWater, 7 }, { Materials.ConsecratedWater, 2 } };
    private static readonly Dictionary<Materials, int> Rarity4LevelMap =
        new() { { Materials.HolyWater, 3 }, { Materials.ConsecratedWater, 3 } };
    private static readonly Dictionary<Materials, int> Rarity5LevelMap =
        new() { { Materials.HolyWater, 3 }, { Materials.ConsecratedWater, 5 } };
    private static readonly Dictionary<Materials, int> Rarity9LevelMap =
        new()
        {
            { Materials.HolyWater, 2 },
            { Materials.ConsecratedWater, 14 },
            { Materials.TutelarySuccessorsMemory, 2 }
        };

    public static IEnumerable<object[]> SuccessfulLevelData =>
        new List<object[]>
        {
            new object[] { AbilityCrests.ManaFount, Rarity2LevelMap, 0, 6 },
            new object[] { AbilityCrests.ManaFount, Rarity2LevelMap, 4, 10 },
            new object[] { AbilityCrests.DragonsNest, Rarity3LevelMap, 0, 12 },
            new object[] { AbilityCrests.DragonsNest, Rarity3LevelMap, 4, 20 },
            new object[] { AbilityCrests.HisCleverBrother, Rarity4LevelMap, 0, 20 },
            new object[] { AbilityCrests.HisCleverBrother, Rarity4LevelMap, 4, 40 },
            new object[] { AbilityCrests.WorthyRivals, Rarity5LevelMap, 0, 30 },
            new object[] { AbilityCrests.WorthyRivals, Rarity5LevelMap, 4, 50 },
            new object[] { AbilityCrests.TutelarysDestinyWolfsBoon, Rarity9LevelMap, 0, 10 },
            new object[] { AbilityCrests.TutelarysDestinyWolfsBoon, Rarity9LevelMap, 4, 30 }
        };

    public AbilityCrestServiceTest(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);

        this.abilityCrestService = new AbilityCrestService(
            this.mockAbilityCrestRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockUserDataRepository.Object,
            LoggerTestUtils.Create<AbilityCrestService>(),
            this.mockMissionProgressionService.Object
        );
    }

    [Fact]
    public async Task AddOrRefund_AbilityCrestNotFoundAddsAbilityCrest()
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.WorthyRivals))
            .ReturnsAsync(() => null);
        this.mockAbilityCrestRepository.Setup(x =>
            x.Add(AbilityCrests.WorthyRivals, null, null, null)
        )
            .Returns(Task.CompletedTask);

        await this.abilityCrestService.AddOrRefund(AbilityCrests.WorthyRivals);

        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task AddOrRefund_TwoStarAbilityCrestRefundsCoin()
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(
                new DbAbilityCrest() { ViewerId = 1, AbilityCrestId = AbilityCrests.ManaFount }
            );
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(50)).Returns(Task.CompletedTask);

        await this.abilityCrestService.AddOrRefund(AbilityCrests.ManaFount);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(AbilityCrests.TheOrdersMessengerOwl, 10)]
    [InlineData(AbilityCrests.DragonsNest, 150)]
    [InlineData(AbilityCrests.TheBewitchingMagician, 100)]
    [InlineData(AbilityCrests.HisCleverBrother, 1_000)]
    [InlineData(AbilityCrests.UnitedbyOneVision, 300)]
    [InlineData(AbilityCrests.BondsBetweenWorlds, 1_500)]
    [InlineData(AbilityCrests.WorthyRivals, 3_000)]
    public async Task AddOrRefund_AbilityCrestsRefundExpectedDewpoint(
        AbilityCrests abilityCrestId,
        int dewpoint
    )
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(new DbAbilityCrest() { ViewerId = 1, AbilityCrestId = abilityCrestId });
        this.mockUserDataRepository.Setup(x => x.UpdateDewpoint(dewpoint))
            .Returns(Task.CompletedTask);

        await this.abilityCrestService.AddOrRefund(abilityCrestId);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task AddOrRefund_NineStarAbilityCrestRefundsExpectedMaterials()
    {
        this.mockAbilityCrestRepository.Setup(x =>
            x.FindAsync(AbilityCrests.TutelarysDestinyWolfsBoon)
        )
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon
                }
            );
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                new Dictionary<Materials, int>() { { Materials.TutelarySuccessorsMemory, 6 } }
            )
        )
            .Returns(Task.CompletedTask);

        await this.abilityCrestService.AddOrRefund(AbilityCrests.TutelarysDestinyWolfsBoon);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_WithInvalidBuildupPieceIdReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Refine,
                IsUseDedicatedMaterial = false,
                Step = 1
            };

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.CommonInvalidArgument);
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Unbind)]
    [InlineData(BuildupPieceTypes.Copies)]
    public async Task TryBuildup_Generic_WithInvalidStepReturnsInvalidResultCode(
        BuildupPieceTypes buildupType
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = buildupType,
                IsUseDedicatedMaterial = false,
                Step = 0
            };

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Unbind)]
    [InlineData(BuildupPieceTypes.Copies)]
    public async Task TryBuildup_Generic_WithInvalidBuildupGroupIdReturnsInvalidResultCode(
        BuildupPieceTypes buildupType
    )
    {
        AbilityCrest abilityCrest =
            new(
                0,
                0,
                0,
                0,
                Materials.Empty,
                Materials.Empty,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false
            );
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = buildupType,
                IsUseDedicatedMaterial = false,
                Step = 1
            };

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task TryBuildup_Copies_WithDedicatedUnbindMaterialThrowsError()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.WorthyRivals);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Copies,
                IsUseDedicatedMaterial = true,
                Step = 2
            };

        try
        {
            await this.abilityCrestService.TryBuildup(abilityCrest, pieceList);
            Assert.Fail("Should have been unable to buildup wyrmprint with dedicated material");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        }
    }

    [Theory]
    [InlineData(AbilityCrests.ManaFount)]
    [InlineData(AbilityCrests.DragonsNest)]
    [InlineData(AbilityCrests.TutelarysDestinyWolfsBoon)]
    public async Task TryBuildup_Unbind_WithDedicatedUnbindMaterialOnInvalidRarityThrowsError(
        AbilityCrests abilityCrestId
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Unbind,
                IsUseDedicatedMaterial = true,
                Step = 1
            };

        try
        {
            await this.abilityCrestService.TryBuildup(abilityCrest, pieceList);
            Assert.Fail("Should have been unable to unbind wyrmprint with dedicated material");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        }
    }

    [Theory]
    [InlineData(AbilityCrests.FromWhenceHeComes, Materials.SilverKey)]
    [InlineData(AbilityCrests.WorthyRivals, Materials.GoldenKey)]
    public async Task TryBuildup_Unbind_WithDedicatedUnbindMaterialWithoutMaterialReturnsInvalidResultCode(
        AbilityCrests abilityCrestId,
        Materials materialId
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Unbind,
                IsUseDedicatedMaterial = true,
                Step = 1
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(new Dictionary<Materials, int>() { { materialId, 1 } })
        )
            .ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Unbind_WithoutDedicatedMaterialWithoutMaterialReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.GreatwyrmMidgardsormr
        );
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Unbind,
                IsUseDedicatedMaterial = false,
                Step = 2
            };

        Dictionary<Materials, int> expectedMap =
            new() { { Materials.JadeInsignia, 40 }, { Materials.DyrenellAureus, 5 } };
        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IEnumerable<KeyValuePair<Materials, int>>>(y =>
                    y.IsEquivalent(expectedMap, this.testOutputHelper)
                )
            )
        )
            .ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Copies_WithoutMaterialReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Copies,
                IsUseDedicatedMaterial = false,
                Step = 3
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                new Dictionary<Materials, int>()
                {
                    { Materials.TwilightShard, 200 },
                    { Materials.TwilightPrism, 150 },
                    { Materials.TutelarySuccessorsMemory, 120 }
                }
            )
        )
            .ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Unbind_WithoutDedicatedMaterialWithoutDewpointReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Unbind,
                IsUseDedicatedMaterial = false,
                Step = 1
            };

        this.mockInventoryRepository.Setup(x => x.CheckQuantity(new Dictionary<Materials, int>()))
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(10)).ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Copies_WithoutDedicatedMaterialWithoutDewpointReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.DragonsNest);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Copies,
                IsUseDedicatedMaterial = false,
                Step = 2
            };

        this.mockInventoryRepository.Setup(x => x.CheckQuantity(new Dictionary<Materials, int>()))
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(1_700)).ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Unbind)]
    [InlineData(BuildupPieceTypes.Copies)]
    public async Task TryBuildup_Generic_CantFindAbilityCrestInDbThrowsError(
        BuildupPieceTypes buildupType
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = buildupType,
                IsUseDedicatedMaterial = false,
                Step = 2
            };

        this.mockInventoryRepository.Setup(x => x.CheckQuantity(new Dictionary<Materials, int>()))
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(10)).ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(() => null);

        try
        {
            await this.abilityCrestService.TryBuildup(abilityCrest, pieceList);
            Assert.Fail("Should have been unable to find ability crest in db and thrown error");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
        }

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Unbind, 1)]
    [InlineData(BuildupPieceTypes.Unbind, 3)]
    [InlineData(BuildupPieceTypes.Unbind, 4)]
    [InlineData(BuildupPieceTypes.Copies, 3)]
    [InlineData(BuildupPieceTypes.Copies, 4)]
    public async Task TryBuildup_Generic_StepNotSequentialReturnsInvalidResultCode(
        BuildupPieceTypes buildupType,
        int currLevel
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = buildupType,
                IsUseDedicatedMaterial = false,
                Step = 3
            };

        this.mockInventoryRepository.Setup(x => x.CheckQuantity(new Dictionary<Materials, int>()))
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(10)).ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = AbilityCrests.ManaFount,
                    LimitBreakCount = currLevel
                }
            );

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(SuccessfulUnbindData))]
    public async Task TryBuildup_Unbind_SuccessfulReturnsSuccessfulResultCode(
        AbilityCrests abilityCrestId,
        int step,
        bool isDedicated,
        Dictionary<Materials, int> materialMap,
        int dewpoint
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Unbind,
                IsUseDedicatedMaterial = isDedicated,
                Step = step
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IEnumerable<KeyValuePair<Materials, int>>>(y =>
                    y.IsEquivalent(materialMap, this.testOutputHelper)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(dewpoint)).ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = abilityCrestId,
                    LimitBreakCount = step - 1
                }
            );
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IEnumerable<KeyValuePair<Materials, int>>>(y =>
                    y.IsEquivalent(materialMap.Invert(), this.testOutputHelper)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateDewpoint(-dewpoint))
            .Returns(Task.CompletedTask);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(SuccessfulCopiesData))]
    public async Task TryBuildup_Copies_SuccessfulReturnsSuccessfulResultCode(
        AbilityCrests abilityCrestId,
        Dictionary<Materials, int> materialMap,
        int dewpoint
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Copies,
                IsUseDedicatedMaterial = false,
                Step = 2
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IEnumerable<KeyValuePair<Materials, int>>>(y =>
                    y.IsEquivalent(materialMap, this.testOutputHelper)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckDewpoint(dewpoint)).ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = abilityCrestId,
                    EquipableCount = 1
                }
            );
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IEnumerable<KeyValuePair<Materials, int>>>(y =>
                    y.IsEquivalent(materialMap.Invert(), this.testOutputHelper)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateDewpoint(-dewpoint))
            .Returns(Task.CompletedTask);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Level_WithInvalidBuildupLevelIdReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest =
            new(
                0,
                0,
                0,
                0,
                Materials.Empty,
                Materials.Empty,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false
            );
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = 1
            };

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task TryBuildup_Level_WithDedicatedUnbindMaterialThrowsError()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.WorthyRivals);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = true,
                Step = 15
            };

        try
        {
            await this.abilityCrestService.TryBuildup(abilityCrest, pieceList);
            Assert.Fail("Should have been unable to buildup wyrmprint with dedicated material");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        }
    }

    [Fact]
    public async Task TryBuildup_Level_WithoutMaterialsReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TheOrdersMessengerOwl
        );
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = 15
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                new Dictionary<Materials, int>
                {
                    { Materials.HolyWater, 7 },
                    { Materials.ConsecratedWater, 2 }
                }
            )
        )
            .ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Level_CantFindAbilityCrestInDbThrowsError()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.DragonsNest);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = 12
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                new Dictionary<Materials, int>
                {
                    { Materials.HolyWater, 7 },
                    { Materials.ConsecratedWater, 2 }
                }
            )
        )
            .ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.DragonsNest))
            .ReturnsAsync(() => null);

        try
        {
            await this.abilityCrestService.TryBuildup(abilityCrest, pieceList);
            Assert.Fail("Should have been unable to find ability crest in db and thrown error");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
        }

        this.mockUserDataRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task TryBuildup_Level_StepNotSequentialReturnsInvalidResultCode(int currLevel)
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = 5
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(new Dictionary<Materials, int> { { Materials.HolyWater, 7 } })
        )
            .ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = AbilityCrests.ManaFount,
                    BuildupCount = currLevel
                }
            );

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(0, 7)]
    [InlineData(1, 8)]
    [InlineData(2, 9)]
    [InlineData(3, 10)]
    public async Task TryBuildup_Level_LimitBreakTooLowReturnsInvalidResultCode(
        int limitBreak,
        int step
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = step
            };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(new Dictionary<Materials, int> { { Materials.HolyWater, 7 } })
        )
            .ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = AbilityCrests.ManaFount,
                    LimitBreakCount = limitBreak,
                    BuildupCount = step - 1
                }
            );

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPieceShortLimitBreakCount);

        this.mockInventoryRepository.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(SuccessfulLevelData))]
    public async Task TryBuildup_Level_SuccessfulReturnsSuccessfulResultCode(
        AbilityCrests abilityCrestId,
        Dictionary<Materials, int> materialMap,
        int limitBreak,
        int step
    )
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenBuildupAbilityCrestPieceList pieceList =
            new()
            {
                BuildupPieceType = BuildupPieceTypes.Stats,
                IsUseDedicatedMaterial = false,
                Step = step
            };

        this.mockInventoryRepository.Setup(x => x.CheckQuantity(materialMap)).ReturnsAsync(true);
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    ViewerId = 1,
                    AbilityCrestId = abilityCrestId,
                    LimitBreakCount = limitBreak,
                    BuildupCount = step - 1
                }
            );
        this.mockInventoryRepository.Setup(x => x.UpdateQuantity(materialMap.Invert()))
            .Returns(Task.CompletedTask);

        this.mockMissionProgressionService.Setup(x =>
            x.OnAbilityCrestLevelUp(abilityCrestId, 1, step)
        );

        (await this.abilityCrestService.TryBuildup(abilityCrest, pieceList))
            .Should()
            .Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockMissionProgressionService.VerifyAll();
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildupAugments_InvalidAugmentNumberReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenPlusCountParamsList augmentParams =
            new() { PlusCount = 41, PlusCountType = PlusCountType.Hp };

        (await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPlusCountCountError);
    }

    [Fact]
    public async Task TryBuildupAugments_InvalidAugmentTypeThrowsError()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenPlusCountParamsList augmentParams = new() { PlusCount = 40, PlusCountType = 0 };

        try
        {
            await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams);
            Assert.Fail("Should have thrown error when augment type invalid");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.CommonInvalidArgument);
        }
    }

    [Fact]
    public async Task TryBuildupAugments_AbilityCrestNotFoundInDbThrowsError()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenPlusCountParamsList augmentParams =
            new() { PlusCount = 40, PlusCountType = PlusCountType.Atk };

        this.mockAbilityCrestRepository.Setup(x =>
            x.FindAsync(AbilityCrests.TutelarysDestinyWolfsBoon)
        )
            .ReturnsAsync(() => null);

        try
        {
            await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams);
            Assert.Fail("Should have been unable to find ability crest in db and thrown error");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
        }

        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildupAugments_DecreaseInAugmentsReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenPlusCountParamsList augmentParams =
            new() { PlusCount = 39, PlusCountType = PlusCountType.Hp };

        this.mockAbilityCrestRepository.Setup(x =>
            x.FindAsync(AbilityCrests.TutelarysDestinyWolfsBoon)
        )
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                    ViewerId = 1,
                    HpPlusCount = 40,
                    AttackPlusCount = 38
                }
            );

        (await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPlusCountCountError);

        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildupAugments_NotEnoughMaterialsReturnsInvalidResultCode()
    {
        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(
            AbilityCrests.TutelarysDestinyWolfsBoon
        );
        AtgenPlusCountParamsList augmentParams =
            new() { PlusCount = 38, PlusCountType = PlusCountType.Atk };

        this.mockAbilityCrestRepository.Setup(x =>
            x.FindAsync(AbilityCrests.TutelarysDestinyWolfsBoon)
        )
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                    ViewerId = 1,
                    HpPlusCount = 40,
                    AttackPlusCount = 31
                }
            );

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                new Dictionary<Materials, int>() { { Materials.AmplifyingGemstone, 7 } }
            )
        )
            .ReturnsAsync(false);

        (await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams))
            .Should()
            .Be(ResultCode.AbilityCrestBuildupPlusCountCountError);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Theory]
    [InlineData(AbilityCrests.WorthyRivals, PlusCountType.Hp, 50)]
    [InlineData(AbilityCrests.WorthyRivals, PlusCountType.Atk, 1)]
    [InlineData(AbilityCrests.ManaFount, PlusCountType.Hp, 1)]
    [InlineData(AbilityCrests.ManaFount, PlusCountType.Atk, 50)]
    [InlineData(AbilityCrests.TutelarysDestinyWolfsBoon, PlusCountType.Hp, 40)]
    [InlineData(AbilityCrests.TutelarysDestinyWolfsBoon, PlusCountType.Atk, 1)]
    [InlineData(AbilityCrests.TheGeniusTacticianBowsBoon, PlusCountType.Hp, 1)]
    [InlineData(AbilityCrests.TheGeniusTacticianBowsBoon, PlusCountType.Atk, 40)]
    public async Task TryBuildupAugments_SuccessfulReturnsSuccessfulResultCode(
        AbilityCrests abilityCrestId,
        PlusCountType augmentType,
        int amount
    )
    {
        this.mockMissionProgressionService.Setup(x =>
            x.OnAbilityCrestBuildupPlusCount(abilityCrestId, augmentType, amount, amount)
        );

        // 0, 0 since this only ever levels up one augment
        this.mockMissionProgressionService.Setup(x =>
            x.OnAbilityCrestTotalPlusCountUp(abilityCrestId, 0, 0)
        );

        AbilityCrest abilityCrest = MasterAsset.AbilityCrest.Get(abilityCrestId);
        AtgenPlusCountParamsList augmentParams =
            new() { PlusCount = amount, PlusCountType = augmentType };
        Materials material =
            augmentType == PlusCountType.Hp
                ? Materials.FortifyingGemstone
                : Materials.AmplifyingGemstone;

        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(new DbAbilityCrest() { AbilityCrestId = abilityCrestId, ViewerId = 1 });

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(new Dictionary<Materials, int>() { { material, amount } })
        )
            .ReturnsAsync(true);

        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(new Dictionary<Materials, int>() { { material, -amount } })
        )
            .Returns(Task.CompletedTask);

        (await this.abilityCrestService.TryBuildupAugments(abilityCrest, augmentParams))
            .Should()
            .Be(ResultCode.Success);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockMissionProgressionService.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryResetAugments_AbilityCrestNotFoundInDbThrowsError()
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.WorthyRivals))
            .ReturnsAsync(() => null);

        try
        {
            await this.abilityCrestService.TryResetAugments(
                AbilityCrests.WorthyRivals,
                PlusCountType.Hp
            );
            Assert.Fail("Should have been unable to find ability crest in db and thrown error");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
        }

        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryResetAugments_InvalidAugmentTypeReturnsInvalidResultCode()
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.WorthyRivals))
            .ReturnsAsync(
                new DbAbilityCrest() { AbilityCrestId = AbilityCrests.WorthyRivals, ViewerId = 1 }
            );

        try
        {
            await this.abilityCrestService.TryResetAugments(AbilityCrests.WorthyRivals, 0);
            Assert.Fail("Should have thrown error when augment type invalid");
        }
        catch (DragaliaException e)
        {
            e.Code.Should().Be(ResultCode.CommonInvalidArgument);
        }

        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task TryResetAugments_NotEnoughCoinReturnsInvalidResultCode()
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrests.WorthyRivals))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    AbilityCrestId = AbilityCrests.WorthyRivals,
                    ViewerId = 1,
                    HpPlusCount = 35,
                    AttackPlusCount = 50
                }
            );

        this.mockUserDataRepository.Setup(x => x.CheckCoin(700_000)).ReturnsAsync(false);

        (
            await this.abilityCrestService.TryResetAugments(
                AbilityCrests.WorthyRivals,
                PlusCountType.Hp
            )
        )
            .Should()
            .Be(ResultCode.CommonMaterialShort);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(AbilityCrests.WorthyRivals, 1)]
    [InlineData(AbilityCrests.HisCleverBrother, 50)]
    [InlineData(AbilityCrests.TutelarysDestinyWolfsBoon, 1)]
    [InlineData(AbilityCrests.TutelarysDestinyWolfsBoon, 40)]
    public async Task TryResetAugments_SuccessfulReturnsSuccessfulResultCodeForHpAugmentsAndRefundsMaterials(
        AbilityCrests abilityCrestId,
        int amount
    )
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    AbilityCrestId = abilityCrestId,
                    ViewerId = 1,
                    HpPlusCount = amount,
                    AttackPlusCount = 30
                }
            );

        this.mockUserDataRepository.Setup(x => x.CheckCoin(amount * 20_000)).ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-amount * 20_000))
            .Returns(Task.CompletedTask);
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                new Dictionary<Materials, int> { { Materials.FortifyingGemstone, amount } }
            )
        )
            .Returns(Task.CompletedTask);

        (await this.abilityCrestService.TryResetAugments(abilityCrestId, PlusCountType.Hp))
            .Should()
            .Be(ResultCode.Success);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(AbilityCrests.DragonsNest, 1)]
    [InlineData(AbilityCrests.ManaFount, 50)]
    [InlineData(AbilityCrests.TheGeniusTacticianBowsBoon, 1)]
    [InlineData(AbilityCrests.TheGeniusTacticianBowsBoon, 40)]
    public async Task TryResetAugments_SuccessfulReturnsSuccessfulResultCodeForAttackAugmentsAndRefundsMaterials(
        AbilityCrests abilityCrestId,
        int amount
    )
    {
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(abilityCrestId))
            .ReturnsAsync(
                new DbAbilityCrest()
                {
                    AbilityCrestId = abilityCrestId,
                    ViewerId = 1,
                    HpPlusCount = 25,
                    AttackPlusCount = amount
                }
            );

        this.mockUserDataRepository.Setup(x => x.CheckCoin(amount * 20_000)).ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-amount * 20_000))
            .Returns(Task.CompletedTask);
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                new Dictionary<Materials, int> { { Materials.AmplifyingGemstone, amount } }
            )
        )
            .Returns(Task.CompletedTask);

        (await this.abilityCrestService.TryResetAugments(abilityCrestId, PlusCountType.Atk))
            .Should()
            .Be(ResultCode.Success);

        this.mockAbilityCrestRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }
}
