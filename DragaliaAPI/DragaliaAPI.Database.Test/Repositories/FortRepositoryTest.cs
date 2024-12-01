using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.Time.Testing;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class FortRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly FortRepository fortRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly FakeTimeProvider fakeTimeProvider;

    public FortRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(DbTestFixture.ViewerId);

        this.fakeTimeProvider = new();

        this.fixture.ApiContext.Database.EnsureDeleted();
        this.fixture.ApiContext.Database.EnsureCreated();
        this.fixture.ApiContext.ChangeTracker.Clear();

        this.fortRepository = new FortRepository(
            this.fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            this.fakeTimeProvider,
            LoggerTestUtils.Create<FortRepository>()
        );
    }

    [Fact]
    public async Task CheckPlantLevel_Success_ReturnsTrue()
    {
        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                ViewerId = DbTestFixture.ViewerId,
                PlantId = FortPlants.Dragonata,
                Level = 10,
            }
        );

        (await this.fortRepository.CheckPlantLevel(FortPlants.Dragonata, 10)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckPlantLevel_Fail_ReturnsFalse()
    {
        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                ViewerId = DbTestFixture.ViewerId,
                PlantId = FortPlants.BroadleafTree,
                Level = 3,
            }
        );

        (await this.fortRepository.CheckPlantLevel(FortPlants.BroadleafTree, 10))
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task GetFortDetail_ReturnsFortDetail()
    {
        DbFortDetail detail = new DbFortDetail()
        {
            ViewerId = DbTestFixture.ViewerId,
            CarpenterNum = 2,
        };
        await this.fixture.AddToDatabase(detail);

        (await this.fortRepository.GetFortDetail())
            .Should()
            .BeEquivalentTo(detail, opts => opts.WithDateTimeTolerance());

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetFortDetail_NotFound_CreatesNew()
    {
        (await this.fortRepository.GetFortDetail())
            .Should()
            .BeEquivalentTo(
                new DbFortDetail() { ViewerId = DbTestFixture.ViewerId, CarpenterNum = 2 }
            );

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task UpdateFortMaximumCarpenter_UpdatesCarpenterNum()
    {
        await this.fixture.AddToDatabase(
            new DbFortDetail() { ViewerId = DbTestFixture.ViewerId, CarpenterNum = 2 }
        );

        await this.fortRepository.UpdateFortMaximumCarpenter(4);

        (
            await this.fixture.ApiContext.PlayerFortDetails.FindAsync(
                DbTestFixture.ViewerId,
                TestContext.Current.CancellationToken
            )
        )!
            .CarpenterNum.Should()
            .Be(4);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetBuilding_GetsBuilding()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = DbTestFixture.ViewerId,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 8,
                IsNew = true,
                BuildEndDate = DateTimeOffset.MaxValue,
                BuildStartDate = DateTimeOffset.MinValue,
                LastIncomeDate = DateTimeOffset.UnixEpoch,
                PositionX = 3,
                PositionZ = 4,
            };

        await this.fixture.AddToDatabase(build);

        (await this.fortRepository.GetBuilding(8))
            .Should()
            .BeEquivalentTo(
                build,
                opts => opts.WithDateTimeTolerance().WithTimeSpanTolerance(TimeSpan.FromSeconds(1))
            );
    }

    [Fact]
    public async Task GetBuilding_NotFound_Throws()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = DbTestFixture.ViewerId + 1,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 9,
                IsNew = true,
                BuildEndDate = DateTimeOffset.MaxValue,
                BuildStartDate = DateTimeOffset.MinValue,
                LastIncomeDate = DateTimeOffset.UnixEpoch,
                PositionX = 3,
                PositionZ = 4,
            };

        await this.fixture.AddToDatabase(build);

        await this
            .fortRepository.Invoking(x => x.GetBuilding(9))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddBuild_Adds()
    {
        await this.fortRepository.AddBuild(
            new() { ViewerId = DbTestFixture.ViewerId, BuildId = 12 }
        );

        (
            await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(
                12L,
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task DeleteBuild_Deletes()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = DbTestFixture.ViewerId,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 15,
            };

        await this.fixture.AddToDatabase(build);

        this.fortRepository.DeleteBuild(build);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (
            await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(
                44L,
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task GetActiveCarpenters_ReturnsActiveCarpenters()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.PalmTree,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue,
                },
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.Lectern,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue,
                },
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.Snowdrake,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(22),
                },
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.Wishmill,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                },
                new()
                {
                    ViewerId = DbTestFixture.ViewerId + 1,
                    PlantId = FortPlants.FafnirStatueFlame,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.MaxValue,
                },
            }
        );

        (await this.fortRepository.GetActiveCarpenters()).Should().Be(3);
    }

    [Fact]
    public async Task GetActiveCarpenters_ExcludesFinishedBuildings()
    {
        this.fakeTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.PalmTree,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.UtcNow.AddDays(-1),
                },
                new()
                {
                    ViewerId = DbTestFixture.ViewerId,
                    PlantId = FortPlants.Lectern,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.UtcNow.AddDays(-1),
                },
            }
        );

        (await this.fortRepository.GetActiveCarpenters()).Should().Be(0);
    }

    [Fact]
    public async Task InitializeFort_InitializesFort()
    {
        await this.fortRepository.InitializeFort();
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.fixture.ApiContext.PlayerFortDetails.Should()
            .ContainEquivalentOf(
                new DbFortDetail() { ViewerId = DbTestFixture.ViewerId, CarpenterNum = 2 }
            );

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .Contain(x =>
                x.PlantId == FortPlants.TheHalidom && x.ViewerId == DbTestFixture.ViewerId
            );

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task InitializeFort_DataExists_DoesNotThrow()
    {
        await this.fortRepository.InitializeFort();
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.fortRepository.Invoking(x => x.InitializeFort()).Should().NotThrowAsync();

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task AddDojos_AddsDojos()
    {
        await this.fortRepository.AddDojos();
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        FortPlants[] plants =
        {
            FortPlants.SwordDojo,
            FortPlants.AxeDojo,
            FortPlants.BladeDojo,
            FortPlants.BowDojo,
            FortPlants.DaggerDojo,
            FortPlants.LanceDojo,
            FortPlants.ManacasterDojo,
            FortPlants.StaffDojo,
            FortPlants.WandDojo,
        };

        foreach (FortPlants plant in plants)
        {
            this.fixture.ApiContext.PlayerFortBuilds.Should()
                .Contain(x =>
                    x.PlantId == plant
                    && x.ViewerId == DbTestFixture.ViewerId
                    && x.PositionX == -1
                    && x.PositionZ == -1
                    && x.Level == 1
                );
        }

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task AddToStorage_IsTotalQuantity_AddsBuilds()
    {
        this.fixture.ApiContext.PlayerFortBuilds.AddRange(
            new List<DbFortBuild>()
            {
                new()
                {
                    ViewerId = DbTestFixture.ViewerId + 1,
                    PlantId = FortPlants.FlameDracolith,
                },
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.fortRepository.AddToStorage(
            FortPlants.FlameDracolith,
            quantity: 1,
            isTotalQuantity: true,
            level: 4
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .Contain(x =>
                x.ViewerId == DbTestFixture.ViewerId
                && x.Level == 4
                && x.PositionX == -1
                && x.PositionZ == -1
            );
    }

    [Fact]
    public async Task AddToStorage_IsTotalQuantity_AlreadyOwned_DoesNotAddBuilds()
    {
        this.fixture.ApiContext.PlayerFortBuilds.AddRange(
            new List<DbFortBuild>()
            {
                new() { ViewerId = DbTestFixture.ViewerId, PlantId = FortPlants.WindDracolith },
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.fortRepository.AddToStorage(
            FortPlants.WindDracolith,
            quantity: 1,
            isTotalQuantity: true,
            level: 4
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .ContainSingle(x => x.PlantId == FortPlants.WindDracolith);
    }
}
