using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

public class FortRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IFortRepository fortRepository;

    public FortRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.fortRepository = new FortRepository(this.fixture.ApiContext);

        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<DateTimeOffset>(
                        ctx =>
                            ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))
                    )
                    .WhenTypeIs<DateTimeOffset>()
        );

        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<TimeSpan>(
                        ctx =>
                            ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))
                    )
                    .WhenTypeIs<TimeSpan>()
        );

        AssertionOptions.AssertEquivalencyUsing(
            options => options.Excluding(x => x.Name == "Owner")
        );
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
}
