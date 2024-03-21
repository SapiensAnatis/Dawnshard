using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class FortRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly FortRepository fortRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    public FortRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);

        this.fortRepository = new FortRepository(
            this.fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            LoggerTestUtils.Create<FortRepository>()
        );

        CommonAssertionOptions.ApplyTimeOptions();
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Builds_FiltersByAccountId()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new() { ViewerId = 1, PlantId = FortPlants.TheHungerdome, },
                new() { ViewerId = 1, PlantId = FortPlants.CircusTent, },
                new() { ViewerId = 2, PlantId = FortPlants.JackOLantern, },
                new() { ViewerId = 3, PlantId = FortPlants.WaterAltar, },
            }
        );

        (await this.fortRepository.Builds.ToListAsync())
            .Should()
            .AllSatisfy(x => x.ViewerId.Should().Be(1))
            .And.ContainEquivalentOf(
                new DbFortBuild() { ViewerId = 1, PlantId = FortPlants.TheHungerdome, },
                opts => opts.Excluding(x => x.Owner).Excluding(x => x.BuildId)
            )
            .And.ContainEquivalentOf(
                new DbFortBuild() { ViewerId = 1, PlantId = FortPlants.CircusTent },
                opts => opts.Excluding(x => x.Owner).Excluding(x => x.BuildId)
            );

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task CheckPlantLevel_Success_ReturnsTrue()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                ViewerId = 1,
                PlantId = FortPlants.Dragonata,
                Level = 10
            }
        );

        (await this.fortRepository.CheckPlantLevel(FortPlants.Dragonata, 10)).Should().BeTrue();

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task CheckPlantLevel_Fail_ReturnsFalse()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                ViewerId = 1,
                PlantId = FortPlants.BroadleafTree,
                Level = 3
            }
        );

        (await this.fortRepository.CheckPlantLevel(FortPlants.BroadleafTree, 10))
            .Should()
            .BeFalse();

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetFortDetail_ReturnsFortDetail()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        DbFortDetail detail = new DbFortDetail() { ViewerId = 1, CarpenterNum = 2, };
        await this.fixture.AddToDatabase(detail);

        (await this.fortRepository.GetFortDetail()).Should().BeEquivalentTo(detail);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetFortDetail_NotFound_CreatesNew()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(4);

        (await this.fortRepository.GetFortDetail())
            .Should()
            .BeEquivalentTo(new DbFortDetail() { ViewerId = 4, CarpenterNum = 2 });

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task UpdateFortMaximumCarpenter_UpdatesCarpenterNum()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(5);

        await this.fixture.AddToDatabase(new DbFortDetail() { ViewerId = 5, CarpenterNum = 2 });

        await this.fortRepository.UpdateFortMaximumCarpenter(4);

        (await this.fixture.ApiContext.PlayerFortDetails.FindAsync(5L))!
            .CarpenterNum.Should()
            .Be(4);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetBuilding_GetsBuilding()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 8,
                IsNew = true,
                BuildEndDate = DateTimeOffset.MaxValue,
                BuildStartDate = DateTimeOffset.MinValue,
                LastIncomeDate = DateTimeOffset.UnixEpoch,
                PositionX = 3,
                PositionZ = 4
            };

        await this.fixture.AddToDatabase(build);

        (await this.fortRepository.GetBuilding(8)).Should().BeEquivalentTo(build);
        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetBuilding_NotFound_Throws()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        DbFortBuild build =
            new()
            {
                ViewerId = 2,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 9,
                IsNew = true,
                BuildEndDate = DateTimeOffset.MaxValue,
                BuildStartDate = DateTimeOffset.MinValue,
                LastIncomeDate = DateTimeOffset.UnixEpoch,
                PositionX = 3,
                PositionZ = 4
            };

        await this.fixture.AddToDatabase(build);

        await this
            .fortRepository.Invoking(x => x.GetBuilding(9))
            .Should()
            .ThrowAsync<InvalidOperationException>();
        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task AddBuild_Adds()
    {
        await this.fortRepository.AddBuild(new() { ViewerId = 12, BuildId = 12 });

        (await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(12L)).Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteBuild_Deletes()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = 44,
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 15,
            };

        await this.fixture.AddToDatabase(build);

        this.fortRepository.DeleteBuild(build);
        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(44L)).Should().BeNull();
    }

    [Fact]
    public async Task GetActiveCarpenters_ReturnsActiveCarpenters()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(12);

        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new()
                {
                    ViewerId = 12,
                    PlantId = FortPlants.PalmTree,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue
                },
                new()
                {
                    ViewerId = 12,
                    PlantId = FortPlants.Lectern,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue
                },
                new()
                {
                    ViewerId = 12,
                    PlantId = FortPlants.Snowdrake,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(22)
                },
                new()
                {
                    ViewerId = 12,
                    PlantId = FortPlants.Wishmill,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch
                },
                new()
                {
                    ViewerId = 13,
                    PlantId = FortPlants.FafnirStatueFlame,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.MaxValue
                }
            }
        );

        (await this.fortRepository.GetActiveCarpenters()).Should().Be(3);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task InitializeFort_InitializesFort()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(13);

        await this.fortRepository.InitializeFort();
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerFortDetails.Should()
            .ContainEquivalentOf(new DbFortDetail() { ViewerId = 13, CarpenterNum = 2 });

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .Contain(x => x.PlantId == FortPlants.TheHalidom && x.ViewerId == 13);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task InitializeFort_DataExists_DoesNotThrow()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(1);

        await this.fortRepository.InitializeFort();
        await this.fixture.ApiContext.SaveChangesAsync();

        await this.fortRepository.Invoking(x => x.InitializeFort()).Should().NotThrowAsync();

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task AddDojos_AddsDojos()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(80);

        await this.fortRepository.AddDojos();
        await this.fixture.ApiContext.SaveChangesAsync();

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
            FortPlants.WandDojo
        };

        foreach (FortPlants plant in plants)
        {
            this.fixture.ApiContext.PlayerFortBuilds.Should()
                .Contain(x =>
                    x.PlantId == plant
                    && x.ViewerId == 80
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
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(2);

        this.fixture.ApiContext.PlayerFortBuilds.AddRange(
            new List<DbFortBuild>()
            {
                new() { ViewerId = 3, PlantId = FortPlants.FlameDracolith, }
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        await this.fortRepository.AddToStorage(
            FortPlants.FlameDracolith,
            quantity: 1,
            isTotalQuantity: true,
            level: 4
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .Contain(x =>
                x.ViewerId == 2 && x.Level == 4 && x.PositionX == -1 && x.PositionZ == -1
            );
    }

    [Fact]
    public async Task AddToStorage_IsTotalQuantity_AlreadyOwned_DoesNotAddBuilds()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(4);

        this.fixture.ApiContext.PlayerFortBuilds.AddRange(
            new List<DbFortBuild>()
            {
                new() { ViewerId = 4, PlantId = FortPlants.WindDracolith, }
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        await this.fortRepository.AddToStorage(
            FortPlants.WindDracolith,
            quantity: 1,
            isTotalQuantity: true,
            level: 4
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerFortBuilds.Should()
            .ContainSingle(x => x.PlantId == FortPlants.WindDracolith);
    }
}
