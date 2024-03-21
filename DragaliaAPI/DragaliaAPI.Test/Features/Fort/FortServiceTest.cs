using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using MockQueryable.Moq;
using Range = Moq.Range;

namespace DragaliaAPI.Test.Features.Fort;

public class FortServiceTest
{
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<ILogger<FortService>> mockLogger;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly IMapper mapper;
    private readonly Mock<IFortMissionProgressionService> mockFortMissionProgressionService;
    private readonly Mock<IPaymentService> mockPaymentService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IOptionsMonitor<DragonfruitConfig>> mockConfig;
    private readonly Mock<IUserService> mockUserService;
    private readonly FakeTimeProvider mockDateTimeProvider;

    private readonly DateTimeOffset FixedTime = DateTimeOffset.UtcNow;

    private readonly FortService fortService;

    public FortServiceTest()
    {
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mapper = UnitTestUtils.CreateMapper();
        this.mockFortMissionProgressionService = new(MockBehavior.Strict);
        this.mockPaymentService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockConfig = new(MockBehavior.Strict);
        this.mockUserService = new(MockBehavior.Strict);
        this.mockDateTimeProvider = new();

        this.mockConfig.SetupGet(x => x.CurrentValue)
            .Returns(
                new DragonfruitConfig
                {
                    FruitOdds = new Dictionary<string, DragonfruitOdds>
                    {
                        {
                            "NormalOdds",
                            new DragonfruitOdds()
                            {
                                Normal = 100,
                                Ripe = 0,
                                Succulent = 0
                            }
                        },
                        {
                            "RipeOdds",
                            new DragonfruitOdds()
                            {
                                Normal = 0,
                                Ripe = 100,
                                Succulent = 0
                            }
                        },
                        {
                            "SucculentOdds",
                            new DragonfruitOdds()
                            {
                                Normal = 0,
                                Ripe = 0,
                                Succulent = 100
                            }
                        }
                    }
                }
            );

        fortService = new FortService(
            mockFortRepository.Object,
            mockUserDataRepository.Object,
            mockInventoryRepository.Object,
            mockLogger.Object,
            mockPlayerIdentityService.Object,
            mapper,
            mockFortMissionProgressionService.Object,
            mockPaymentService.Object,
            mockRewardService.Object,
            mockConfig.Object,
            mockUserService.Object,
            mockDateTimeProvider
        );

        mockDateTimeProvider.SetUtcNow(FixedTime);

        UnitTestUtils.ApplyDateTimeAssertionOptions();
    }

    [Fact]
    public async Task GetBuildList_ReturnsBuildList()
    {
        mockFortRepository
            .Setup(x => x.Builds)
            .Returns(
                new List<DbFortBuild>()
                {
                    new()
                    {
                        ViewerId = 1,
                        BuildId = 4,
                        BuildEndDate = new(2023, 04, 18, 18, 32, 35, TimeSpan.Zero),
                        BuildStartDate = new(2023, 04, 18, 18, 32, 34, TimeSpan.Zero),
                        Level = 5,
                        PlantId = FortPlants.Dragontree,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        (await fortService.GetBuildList())
            .Should()
            .BeEquivalentTo(
                new List<BuildList>()
                {
                    new()
                    {
                        BuildId = 4,
                        BuildEndDate = new(2023, 04, 18, 18, 32, 35, TimeSpan.Zero),
                        BuildStartDate = new(2023, 04, 18, 18, 32, 34, TimeSpan.Zero),
                        Level = 5,
                        PlantId = FortPlants.Dragontree,
                        FortPlantDetailId = 10030106,
                        BuildStatus = FortBuildStatus.LevelUp,
                    },
                },
                opts => opts.Excluding(x => x.RemainTime).Excluding(x => x.LastIncomeTime)
            );

        mockFortRepository.VerifyAll();
    }

    [Theory]
    [InlineData(2, 250)]
    [InlineData(3, 400)]
    [InlineData(4, 750)]
    public async Task AddCarpenter_Success_AddsCarpenterWithExpectedCost(
        int existingCarpenters,
        int expectedCost
    )
    {
        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = existingCarpenters, ViewerId = 1 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);
        mockFortRepository
            .Setup(x => x.UpdateFortMaximumCarpenter(existingCarpenters + 1))
            .Returns(Task.CompletedTask);

        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Wyrmite, null, expectedCost))
            .Returns(Task.CompletedTask);

        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Wyrmite, null, expectedCost))
            .Returns(Task.CompletedTask);

        await fortService.AddCarpenter(PaymentTypes.Wyrmite);

        mockUserDataRepository.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_OverMaxCarpenters_Throws()
    {
        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = 5, ViewerId = 1 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);

        await fortService
            .Invoking(x => x.AddCarpenter(PaymentTypes.Diamantium))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortExtendCarpenterLimit);

        mockUserDataRepository.VerifyAll();
        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_InvalidPaymentType_Throws()
    {
        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = 4, ViewerId = 1 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);

        await fortService
            .Invoking(x => x.AddCarpenter(PaymentTypes.Ticket))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.ShopPaymentTypeInvalid);

        mockUserDataRepository.VerifyAll();
        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_UpgradesBuilding()
    {
        DbPlayerUserData userData = new() { ViewerId = 1, BuildTimePoint = 1 };
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                Level = 2,
                BuildStartDate = FixedTime,
                BuildEndDate = FixedTime + TimeSpan.FromSeconds(5)
            };

        mockFortMissionProgressionService
            .Setup(x => x.OnFortPlantLevelUp(0, 3))
            .Returns(Task.CompletedTask);

        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.HalidomHustleHammer, null, 1))
            .Returns(Task.CompletedTask);

        await fortService.LevelupAtOnce(PaymentTypes.HalidomHustleHammer, 1);

        mockPaymentService.VerifyAll();
        mockFortMissionProgressionService.VerifyAll();
        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelLevelup_CancelsUpgrade()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                ViewerId = 1,
                BuildStartDate = FixedTime,
                BuildEndDate = FixedTime + TimeSpan.FromDays(1),
                Level = 2,
            };
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await fortService.CancelLevelup(1);

        build.Level.Should().Be(2);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelBuild_CancelsUpgradeAndDeletes()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                ViewerId = 1,
                BuildStartDate = FixedTime,
                BuildEndDate = FixedTime + TimeSpan.FromDays(1),
                Level = 0,
            };
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);
        mockFortRepository.Setup(x => x.DeleteBuild(build));

        await fortService.CancelBuild(1);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelLevelup_NotBuilding_ThrowsInvalidOperationException()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                ViewerId = 1,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UnixEpoch,
                Level = 3,
            };
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await fortService
            .Invoking(x => x.CancelLevelup(1))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        build.Level.Should().Be(3);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task EndLevelup_ResetsBuildDates()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                ViewerId = 1,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = FixedTime - TimeSpan.FromMinutes(1),
                Level = 2,
            };
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        mockFortMissionProgressionService
            .Setup(x => x.OnFortPlantLevelUp(0, 3))
            .Returns(Task.CompletedTask);

        await fortService.EndLevelup(1);

        build.Level.Should().Be(3);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task EndLevelup_NotConstructionComplete_ThrowsInvalidOperationException()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                ViewerId = 1,
                BuildStartDate = DateTimeOffset.MinValue,
                BuildEndDate = DateTimeOffset.MaxValue,
                Level = 2,
            };
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await fortService
            .Invoking(x => x.EndLevelup(1))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        build.Level.Should().Be(2);
        build.BuildStartDate.Should().Be(DateTimeOffset.MinValue);
        build.BuildEndDate.Should().Be(DateTimeOffset.MaxValue);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_StartsBuilding()
    {
        mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { ViewerId = 1, CarpenterNum = 4 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        mockFortRepository
            .Setup(x => x.AddBuild(It.IsAny<DbFortBuild>()))
            .Returns(Task.CompletedTask)
            .Callback(
                (DbFortBuild build) =>
                    build
                        .Should()
                        .BeEquivalentTo(
                            new DbFortBuild()
                            {
                                ViewerId = 1,
                                PlantId = FortPlants.BlueFlowers,
                                Level = 0,
                                PositionX = 2,
                                PositionZ = 3,
                                BuildStartDate = DateTimeOffset.UnixEpoch,
                                BuildEndDate = DateTimeOffset.UnixEpoch,
                                IsNew = true,
                                LastIncomeDate = DateTimeOffset.UnixEpoch
                            }
                        )
            );

        mockInventoryRepository
            .Setup(x => x.UpdateQuantity(new Dictionary<Materials, int>()))
            .Returns(Task.CompletedTask);

        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Coin, null, 300))
            .Returns(Task.CompletedTask);

        await fortService.BuildStart(FortPlants.BlueFlowers, 2, 3);

        mockFortRepository.VerifyAll();
        mockInventoryRepository.VerifyAll();
        mockPlayerIdentityService.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_InsufficientCarpenters_Throws()
    {
        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { ViewerId = 1, CarpenterNum = 1 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);

        await fortService
            .Invoking(x => x.BuildStart(FortPlants.BlueFlowers, 2, 3))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortBuildCarpenterBusy);

        mockFortRepository.VerifyAll();
        mockPlayerIdentityService.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_StartsBuilding()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                Level = 20,
                PlantId = FortPlants.Dragonata
            };

        mockUserDataRepository
            .Setup(x => x.GetFortOpenTimeAsync())
            .ReturnsAsync(DateTimeOffset.MinValue);

        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { ViewerId = 1, CarpenterNum = 4 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Coin, null, 3200))
            .Returns(Task.CompletedTask);

        mockInventoryRepository
            .Setup(x => x.UpdateQuantity(It.IsAny<Dictionary<Materials, int>>()))
            .Returns(Task.CompletedTask)
            .Callback(
                (IEnumerable<KeyValuePair<Materials, int>> materials) =>
                    materials
                        .Should()
                        .BeEquivalentTo(
                            new Dictionary<Materials, int>() { { Materials.Papiermache, -350 } }
                        )
            );

        await fortService.LevelupStart(1);

        build.Level.Should().Be(20);
        build.BuildStartDate.Should().BeCloseTo(FixedTime, TimeSpan.FromSeconds(1));
        build
            .BuildEndDate.Should()
            .BeCloseTo(FixedTime + TimeSpan.FromSeconds(21600), TimeSpan.FromSeconds(1));

        mockFortRepository.VerifyAll();
        mockInventoryRepository.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_InsufficientCarpenters_Throws()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                Level = 20,
                PlantId = FortPlants.Dragonata
            };

        mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { ViewerId = 1, CarpenterNum = 1 });
        mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await fortService
            .Invoking(x => x.LevelupStart(1))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortBuildCarpenterBusy);

        build.Level.Should().Be(20);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        mockFortRepository.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task Move_MovesBuilding()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                Level = 20,
                PlantId = FortPlants.Dragonata,
                PositionX = 2,
                PositionZ = 3,
            };

        mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await fortService.Move(1, 4, 5);

        build.PositionX.Should().Be(4);
        build.PositionZ.Should().Be(5);

        mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_Wyrmite_ConsumesPayment()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                BuildId = 444,
                Level = 5,
                PlantId = FortPlants.Smithy,
                BuildStartDate = FixedTime,
                BuildEndDate = FixedTime + TimeSpan.FromDays(7)
            };

        mockFortRepository.Setup(x => x.GetBuilding(444)).ReturnsAsync(build);
        mockPaymentService
            .Setup(x =>
                x.ProcessPayment(
                    PaymentTypes.Wyrmite,
                    null,
                    It.IsInRange(840, 842, Range.Inclusive)
                )
            )
            .Returns(Task.CompletedTask);

        mockFortMissionProgressionService
            .Setup(x => x.OnFortPlantLevelUp(FortPlants.Smithy, 6))
            .Returns(Task.CompletedTask);

        await fortService.LevelupAtOnce(PaymentTypes.Wyrmite, 444);

        mockFortMissionProgressionService.VerifyAll();
        mockPaymentService.VerifyAll();
        mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_PartialWyrmite_ConsumesPayment()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                BuildId = 445,
                Level = 5,
                PlantId = FortPlants.Smithy,
                BuildStartDate = FixedTime - TimeSpan.FromDays(1),
                BuildEndDate = FixedTime + TimeSpan.FromDays(6)
            };

        const int wyrmiteDifference = 24 * 60 / 12;

        mockFortRepository.Setup(x => x.GetBuilding(445)).ReturnsAsync(build);
        mockPaymentService
            .Setup(x =>
                x.ProcessPayment(
                    PaymentTypes.Wyrmite,
                    null,
                    It.IsInRange(840 - wyrmiteDifference, 842 - wyrmiteDifference, Range.Inclusive)
                )
            )
            .Returns(Task.CompletedTask);

        mockFortMissionProgressionService
            .Setup(x => x.OnFortPlantLevelUp(FortPlants.Smithy, 6))
            .Returns(Task.CompletedTask);

        await fortService.LevelupAtOnce(PaymentTypes.Wyrmite, 445);

        mockFortMissionProgressionService.VerifyAll();
        mockPaymentService.VerifyAll();
        mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_HustleHammers_ConsumesPayment()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                BuildId = 446,
                Level = 5,
                PlantId = FortPlants.Smithy,
                BuildStartDate = FixedTime,
                BuildEndDate = FixedTime + TimeSpan.FromDays(7)
            };

        mockFortRepository.Setup(x => x.GetBuilding(446)).ReturnsAsync(build);
        mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.HalidomHustleHammer, null, 1))
            .Returns(Task.CompletedTask);

        mockFortMissionProgressionService
            .Setup(x => x.OnFortPlantLevelUp(FortPlants.Smithy, 6))
            .Returns(Task.CompletedTask);

        await fortService.LevelupAtOnce(PaymentTypes.HalidomHustleHammer, 446);

        mockFortMissionProgressionService.VerifyAll();
        mockPaymentService.VerifyAll();
    }
}
