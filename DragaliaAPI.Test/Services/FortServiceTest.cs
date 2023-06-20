using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Services;

public class FortServiceTest
{
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<ILogger<FortService>> mockLogger;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly IMapper mapper;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IPaymentService> mockPaymentService;

    private readonly IFortService fortService;

    public FortServiceTest()
    {
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mapper = UnitTestUtils.CreateMapper();
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockPaymentService = new(MockBehavior.Strict);

        this.fortService = new FortService(
            this.mockFortRepository.Object,
            this.mockUserDataRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockLogger.Object,
            this.mockPlayerIdentityService.Object,
            this.mapper,
            this.mockMissionProgressionService.Object,
            this.mockPaymentService.Object
        );

        UnitTestUtils.ApplyDateTimeAssertionOptions();
    }

    [Fact]
    public async Task GetBuildList_ReturnsBuildList()
    {
        this.mockFortRepository
            .Setup(x => x.Builds)
            .Returns(
                new List<DbFortBuild>()
                {
                    new()
                    {
                        DeviceAccountId = "id",
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

        (await this.fortService.GetBuildList())
            .Should()
            .BeEquivalentTo(
                new List<BuildList>()
                {
                    new()
                    {
                        build_id = 4,
                        build_end_date = new(2023, 04, 18, 18, 32, 35, TimeSpan.Zero),
                        build_start_date = new(2023, 04, 18, 18, 32, 34, TimeSpan.Zero),
                        level = 5,
                        plant_id = FortPlants.Dragontree,
                        fort_plant_detail_id = 10030105,
                        build_status = FortBuildStatus.ConstructionComplete,
                    },
                },
                opts => opts.Excluding(x => x.remain_time).Excluding(x => x.last_income_time)
            );

        this.mockFortRepository.VerifyAll();
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
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { DeviceAccountId = "id", Crystal = 10000 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(
                new DbFortDetail() { CarpenterNum = existingCarpenters, DeviceAccountId = "id" }
            );
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);
        this.mockFortRepository
            .Setup(x => x.UpdateFortMaximumCarpenter(existingCarpenters + 1))
            .Returns(Task.CompletedTask);
        
        this.mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Wyrmite, null, expectedCost))
            .Returns(Task.CompletedTask);

        await this.fortService.AddCarpenter(PaymentTypes.Wyrmite);

        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_OverMaxCarpenters_Throws()
    {
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { DeviceAccountId = "id", Crystal = 10000 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = 5, DeviceAccountId = "id" });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);

        await this.fortService
            .Invoking(x => x.AddCarpenter(PaymentTypes.Diamantium))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortExtendCarpenterLimit);

        this.mockUserDataRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_InvalidPaymentType_Throws()
    {
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { DeviceAccountId = "id", Crystal = 10000 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = 4, DeviceAccountId = "id" });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);

        await this.fortService
            .Invoking(x => x.AddCarpenter(PaymentTypes.Ticket))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortExtendCarpenterLimit);

        this.mockUserDataRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_InsufficientCurrency_Throws()
    {
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { DeviceAccountId = "id", Crystal = 1 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { CarpenterNum = 4, DeviceAccountId = "id" });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);

        await this.fortService
            .Invoking(x => x.AddCarpenter(PaymentTypes.Wyrmite))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortExtendCarpenterLimit);

        this.mockUserDataRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CompleteAtOnce_UpgradesBuilding()
    {
        DbPlayerUserData userData = new() { DeviceAccountId = "id", BuildTimePoint = 1 };
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                Level = 2,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5)
            };

        this.mockMissionProgressionService.Setup(x => x.OnFortPlantUpgraded(0));

        this.mockMissionProgressionService.Setup(x => x.OnFortLevelup());

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData> { userData }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        this.mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.HalidomHustleHammer, null, 1))
            .Returns(Task.CompletedTask);

        await this.fortService.CompleteAtOnce(PaymentTypes.HalidomHustleHammer, 1);

        this.mockUserDataRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelUpgrade_CancelsUpgrade()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                DeviceAccountId = "id",
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(1),
                Level = 3,
            };
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await this.fortService.CancelUpgrade(1);

        build.Level.Should().Be(2);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelUpgrade_LevelOne_CancelsUpgradeAndDeletes()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                DeviceAccountId = "id",
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(1),
                Level = 1,
            };
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);
        this.mockFortRepository.Setup(x => x.DeleteBuild(build));

        await this.fortService.CancelUpgrade(1);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CancelUpgrade_NotBuilding_ThrowsInvalidOperationException()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                DeviceAccountId = "id",
                BuildStartDate = DateTimeOffset.MinValue,
                BuildEndDate = DateTimeOffset.MinValue,
                Level = 3,
            };
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await this.fortService
            .Invoking(x => x.CancelUpgrade(1))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        build.Level.Should().Be(3);
        build.BuildStartDate.Should().Be(DateTimeOffset.MinValue);
        build.BuildEndDate.Should().Be(DateTimeOffset.MinValue);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task EndUpgrade_ResetsBuildDates()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                DeviceAccountId = "id",
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1),
                Level = 3,
            };
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        this.mockMissionProgressionService.Setup(x => x.OnFortPlantUpgraded(0));

        this.mockMissionProgressionService.Setup(x => x.OnFortLevelup());

        await this.fortService.EndUpgrade(1);

        build.Level.Should().Be(3);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task EndUpgrade_NotConstructionComplete_ThrowsInvalidOperationException()
    {
        DbFortBuild build =
            new()
            {
                BuildId = 1,
                DeviceAccountId = "id",
                BuildStartDate = DateTimeOffset.MinValue,
                BuildEndDate = DateTimeOffset.MaxValue,
                Level = 3,
            };
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await this.fortService
            .Invoking(x => x.EndUpgrade(1))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        build.Level.Should().Be(3);
        build.BuildStartDate.Should().Be(DateTimeOffset.MinValue);
        build.BuildEndDate.Should().Be(DateTimeOffset.MaxValue);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_StartsBuilding()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-300)).Returns(Task.CompletedTask);

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { DeviceAccountId = "id", CarpenterNum = 4 });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        this.mockFortRepository
            .Setup(x => x.AddBuild(It.IsAny<DbFortBuild>()))
            .Returns(Task.CompletedTask)
            .Callback(
                (DbFortBuild build) =>
                    build
                        .Should()
                        .BeEquivalentTo(
                            new DbFortBuild()
                            {
                                DeviceAccountId = "id",
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

        this.mockInventoryRepository
            .Setup(x => x.UpdateQuantity(new Dictionary<Materials, int>()))
            .Returns(Task.CompletedTask);

        await this.fortService.BuildStart(FortPlants.BlueFlowers, 2, 3);

        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockPlayerIdentityService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_InsufficientCarpenters_Throws()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { DeviceAccountId = "id", CarpenterNum = 1 });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);

        await this.fortService
            .Invoking(x => x.BuildStart(FortPlants.BlueFlowers, 2, 3))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortBuildCarpenterBusy);

        this.mockFortRepository.VerifyAll();
        this.mockPlayerIdentityService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_StartsBuilding()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                Level = 20,
                PlantId = FortPlants.Dragonata
            };

        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-3200)).Returns(Task.CompletedTask);

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { DeviceAccountId = "id", CarpenterNum = 4 });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        this.mockInventoryRepository
            .Setup(x => x.UpdateQuantity(It.IsAny<Dictionary<Materials, int>>()))
            .Returns(Task.CompletedTask)
            .Callback(
                (IEnumerable<KeyValuePair<Materials, int>> materials) =>
                    materials
                        .Should()
                        .BeEquivalentTo(
                            new Dictionary<Materials, int>() { { Materials.Papiermache, 350 } }
                        )
            );

        await this.fortService.LevelupStart(1);

        build.Level.Should().Be(21);
        build.BuildStartDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        build.BuildEndDate
            .Should()
            .BeCloseTo(
                DateTimeOffset.UtcNow + TimeSpan.FromSeconds(21600),
                TimeSpan.FromSeconds(1)
            );

        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_InsufficientCarpenters_Throws()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                Level = 20,
                PlantId = FortPlants.Dragonata
            };

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(new DbFortDetail() { DeviceAccountId = "id", CarpenterNum = 1 });
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(1);
        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await this.fortService
            .Invoking(x => x.LevelupStart(1))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(e => e.Code == ResultCode.FortBuildCarpenterBusy);

        build.Level.Should().Be(20);
        build.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        build.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);

        this.mockFortRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task Move_MovesBuilding()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                Level = 20,
                PlantId = FortPlants.Dragonata,
                PositionX = 2,
                PositionZ = 3,
            };

        this.mockFortRepository.Setup(x => x.GetBuilding(1)).ReturnsAsync(build);

        await this.fortService.Move(1, 4, 5);

        build.PositionX.Should().Be(4);
        build.PositionZ.Should().Be(5);

        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task CompleteAtOnce_Wyrmite_ConsumesPayment()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "wyrmite",
                BuildId = 444,
                Level = 5,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            };

        this.mockFortRepository.Setup(x => x.GetBuilding(444)).ReturnsAsync(build);
        this.mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.Wyrmite, null, 840))
            .Returns(Task.CompletedTask);

        this.mockMissionProgressionService
            .Setup(x => x.OnFortPlantUpgraded(FortPlants.Smithy));

        this.mockMissionProgressionService
            .Setup(x => x.OnFortLevelup());

        await this.fortService.CompleteAtOnce(PaymentTypes.Wyrmite, 444);

        this.mockMissionProgressionService.VerifyAll();
        this.mockPaymentService.VerifyAll();
        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task CompleteAtOnce_HustleHammers_ConsumesPayment()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "hustler",
                BuildId = 445,
                Level = 5,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            };

        this.mockFortRepository.Setup(x => x.GetBuilding(444)).ReturnsAsync(build);
        this.mockPaymentService
            .Setup(x => x.ProcessPayment(PaymentTypes.HalidomHustleHammer, null, 1))
            .Returns(Task.CompletedTask);

        this.mockMissionProgressionService
            .Setup(x => x.OnFortPlantUpgraded(FortPlants.Smithy));

        this.mockMissionProgressionService
            .Setup(x => x.OnFortLevelup());

        await this.fortService.CompleteAtOnce(
            PaymentTypes.HalidomHustleHammer,
            445
        );

        this.mockMissionProgressionService.VerifyAll();
        this.mockPaymentService.VerifyAll();
    }

    [Theory]
    [InlineData(PaymentTypes.Wyrmite)]
    [InlineData(PaymentTypes.HalidomHustleHammer)]
    public async Task UpgradeAtOnce_InsufficientHeld_Throws(PaymentTypes type)
    {
        DbPlayerUserData userData =
            new()
            {
                DeviceAccountId = "broke",
                Crystal = 0,
                BuildTimePoint = 0
            };

        int buildId = Random.Shared.Next(500, 600);
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "broke",
                BuildId = buildId,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            };

        this.mockFortRepository
            .Setup(x => x.GetBuilding(buildId))
            .ReturnsAsync(build);

        await this.fortService
            .Invoking(x => x.CompleteAtOnce(type, buildId))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        this.mockPlayerIdentityService.VerifyAll();
    }
}
