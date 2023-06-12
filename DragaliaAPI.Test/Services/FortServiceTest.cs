using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
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
    private readonly Mock<IPlayerDetailsService> mockPlayerDetailsService;
    private readonly IMapper mapper;

    private readonly IFortService fortService;

    public FortServiceTest()
    {
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockPlayerDetailsService = new(MockBehavior.Strict);
        this.mapper = UnitTestUtils.CreateMapper();

        this.fortService = new FortService(
            this.mockFortRepository.Object,
            this.mockUserDataRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockLogger.Object,
            this.mockPlayerDetailsService.Object,
            this.mapper
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
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData() { DeviceAccountId = "id", Crystal = 10000 });

        this.mockFortRepository
            .Setup(x => x.GetFortDetail())
            .ReturnsAsync(
                new DbFortDetail() { CarpenterNum = existingCarpenters, DeviceAccountId = "id" }
            );
        this.mockFortRepository.Setup(x => x.GetActiveCarpenters()).ReturnsAsync(0);
        this.mockFortRepository.Setup(
            x =>
                x.ConsumePaymentCost(
                    It.IsAny<DbPlayerUserData>(),
                    PaymentTypes.Wyrmite,
                    expectedCost
                )
        );
        this.mockFortRepository
            .Setup(x => x.UpdateFortMaximumCarpenter(existingCarpenters + 1))
            .Returns(Task.CompletedTask);

        await this.fortService.AddCarpenter(PaymentTypes.Wyrmite);

        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_OverMaxCarpenters_Throws()
    {
        this.mockUserDataRepository
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData() { DeviceAccountId = "id", Crystal = 10000 });

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
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData() { DeviceAccountId = "id", Crystal = 10000 });

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
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData() { DeviceAccountId = "id", Crystal = 1 });

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

        this.mockUserDataRepository.Setup(x => x.LookupUserData()).ReturnsAsync(userData);
        this.mockFortRepository
            .Setup(x => x.UpgradeAtOnce(userData, 1, PaymentTypes.HalidomHustleHammer))
            .ReturnsAsync(new DbFortBuild() { DeviceAccountId = "id" });

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
        this.mockPlayerDetailsService.SetupGet(x => x.AccountId).Returns("id");

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
                                Level = 1,
                                PositionX = 2,
                                PositionZ = 3,
                                BuildStartDate = DateTimeOffset.UtcNow,
                                BuildEndDate = DateTimeOffset.UtcNow,
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
        this.mockPlayerDetailsService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_InsufficientCarpenters_Throws()
    {
        this.mockPlayerDetailsService.SetupGet(x => x.AccountId).Returns("id");

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
        this.mockPlayerDetailsService.VerifyAll();
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
}
