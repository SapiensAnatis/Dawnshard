using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Test.Repositories;

public class FortRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IFortRepository fortRepository;

    public FortRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.fortRepository = new FortRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            LoggerTestUtils.Create<FortRepository>()
        );

        CommonAssertionOptions.ApplyTimeOptions();
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task GetBuilds_FiltersByAccountId()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbFortBuild>()
            {
                new() { DeviceAccountId = "id", PlantId = FortPlants.TheHungerdome, },
                new() { DeviceAccountId = "id", PlantId = FortPlants.CircusTent, },
                new() { DeviceAccountId = "id 2", PlantId = FortPlants.JackOLantern, },
                new() { DeviceAccountId = "id 3", PlantId = FortPlants.WaterAltar, },
            }
        );

        (await this.fortRepository.GetBuilds("id").ToListAsync())
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
    }

    [Fact]
    public async Task CheckPlantLevel_Success_ReturnsTrue()
    {
        await this.fixture.AddToDatabase(
            new DbFortBuild()
            {
                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                PlantId = FortPlants.Dragonata,
                Level = 10
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
                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                PlantId = FortPlants.BroadleafTree,
                Level = 3
            }
        );

        (await this.fortRepository.CheckPlantLevel(FortPlants.BroadleafTree, 10))
            .Should()
            .BeFalse();
    }
}
