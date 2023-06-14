using System.Runtime.InteropServices;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class FortRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IFortRepository fortRepository;
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
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new() { DeviceAccountId = "id", PlantId = FortPlants.TheHungerdome, },
                new() { DeviceAccountId = "id", PlantId = FortPlants.CircusTent, },
                new() { DeviceAccountId = "id 2", PlantId = FortPlants.JackOLantern, },
                new() { DeviceAccountId = "id 3", PlantId = FortPlants.WaterAltar, },
            }
        );

        (await this.fortRepository.Builds.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be("id"))
            .And.ContainEquivalentOf(
                new DbFortBuild() { DeviceAccountId = "id", PlantId = FortPlants.TheHungerdome, },
                opts => opts.Excluding(x => x.Owner).Excluding(x => x.BuildId)
            )
            .And.ContainEquivalentOf(
                new DbFortBuild() { DeviceAccountId = "id", PlantId = FortPlants.CircusTent },
                opts => opts.Excluding(x => x.Owner).Excluding(x => x.BuildId)
            );

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task CheckPlantLevel_Success_ReturnsTrue()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = "id",
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
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = "id",
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
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        DbFortDetail detail = new DbFortDetail() { DeviceAccountId = "id", CarpenterNum = 2, };
        await this.fixture.AddToDatabase(detail);

        (await this.fortRepository.GetFortDetail()).Should().BeEquivalentTo(detail);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetFortDetail_NotFound_CreatesNew()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("no fort");

        (await this.fortRepository.GetFortDetail())
            .Should()
            .BeEquivalentTo(new DbFortDetail() { DeviceAccountId = "no fort", CarpenterNum = 2 });

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task UpdateFortMaximumCarpenter_UpdatesCarpenterNum()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("carpenter");

        await this.fixture.AddToDatabase(
            new DbFortDetail() { DeviceAccountId = "carpenter", CarpenterNum = 2 }
        );

        await this.fortRepository.UpdateFortMaximumCarpenter(4);

        (await this.fixture.ApiContext.PlayerFortDetails.FindAsync("carpenter"))!.CarpenterNum
            .Should()
            .Be(4);

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetBuilding_GetsBuilding()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
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
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("id");

        DbFortBuild build =
            new()
            {
                DeviceAccountId = "other id",
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

        await this.fortRepository
            .Invoking(x => x.GetBuilding(9))
            .Should()
            .ThrowAsync<InvalidOperationException>();
        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task AddBuild_Adds()
    {
        await this.fortRepository.AddBuild(new() { DeviceAccountId = "some id", BuildId = 12 });

        (await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(12L)).Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteBuild_Deletes()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "deleted id",
                PlantId = FortPlants.DaggerDojo,
                Level = 1,
                BuildId = 15,
            };

        await this.fixture.AddToDatabase(build);

        this.fortRepository.DeleteBuild(build);
        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.fixture.ApiContext.PlayerFortBuilds.FindAsync(15L)).Should().BeNull();
    }

    [Fact]
    public async Task UpgradeAtOnce_Wyrmite_ConsumesPayment()
    {
        DbPlayerUserData userData = new() { DeviceAccountId = "wyrmite", Crystal = 1000 };

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("wyrmite");

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = "wyrmite",
                BuildId = 444,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            }
        );

        await this.fortRepository.UpgradeAtOnce(userData, 444, PaymentTypes.Wyrmite);

        userData.Crystal.Should().BeCloseTo(1000 - 840, 1);
        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task UpgradeAtOnce_HustleHammers_ConsumesPayment()
    {
        DbPlayerUserData userData = new() { DeviceAccountId = "hustler", BuildTimePoint = 4 };

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("hustler");

        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = "hustler",
                BuildId = 445,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            }
        );

        await this.fortRepository.UpgradeAtOnce(userData, 445, PaymentTypes.HalidomHustleHammer);

        userData.BuildTimePoint.Should().Be(3);
        this.mockPlayerIdentityService.VerifyAll();
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

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("broke");

        int buildId = Random.Shared.Next(500, 600);
        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = "broke",
                BuildId = buildId,
                PlantId = FortPlants.Smithy,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7)
            }
        );

        await this.fortRepository
            .Invoking(x => x.UpgradeAtOnce(userData, buildId, type))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        this.mockPlayerIdentityService.VerifyAll();
    }

    [Fact]
    public async Task GetActiveCarpenters_ReturnsActiveCarpenters()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("carpenter");

        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new()
                {
                    DeviceAccountId = "carpenter",
                    PlantId = FortPlants.PalmTree,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue
                },
                new()
                {
                    DeviceAccountId = "carpenter",
                    PlantId = FortPlants.Lectern,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.MaxValue
                },
                new()
                {
                    DeviceAccountId = "carpenter",
                    PlantId = FortPlants.Snowdrake,
                    BuildStartDate = DateTimeOffset.MinValue,
                    BuildEndDate = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(22)
                },
                new()
                {
                    DeviceAccountId = "carpenter",
                    PlantId = FortPlants.Wishmill,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch
                },
                new()
                {
                    DeviceAccountId = "some other id",
                    PlantId = FortPlants.FafnirStatueFlame,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.MaxValue
                }
            }
        );

        (await this.fortRepository.GetActiveCarpenters()).Should().Be(3);

        this.mockPlayerIdentityService.VerifyAll();
    }
}
