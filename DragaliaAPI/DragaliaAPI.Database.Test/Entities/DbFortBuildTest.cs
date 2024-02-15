using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Test.Entities;

/// <summary>
/// Tests for getters in <see cref="DbFortBuild"/>
/// </summary>
public class DbFortBuildTest
{
    private static readonly TimeSpan AssertionTolerance = TimeSpan.FromMilliseconds(100);

    [Theory]
    [InlineData(FortPlants.AbyssalBell, 5, 10290105)]
    [InlineData(FortPlants.WaterAltar, 15, 10040215)]
    public void FortPlantDetailId_IsCorrect(FortPlants plantId, int level, int expectedDetailId)
    {
        DbFortBuild entity =
            new()
            {
                ViewerId = 1,
                PlantId = plantId,
                Level = level
            };

        entity.FortPlantDetailId.Should().Be(expectedDetailId);
    }

    [Fact]
    public void BuildStatus_BothDatesEpoch_IsNeutral()
    {
        DbFortBuild entity =
            new()
            {
                ViewerId = 1,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UnixEpoch
            };

        entity.BuildStatus.Should().Be(FortBuildStatus.Neutral);
    }

    [Theory]
    [InlineData(-2, -1, 2, FortBuildStatus.LevelUp)]
    [InlineData(-2, 1, 0, FortBuildStatus.Building)]
    public void BuildStatus_CalculatedCorrectly(
        int startOffsetSec,
        int endOffsetSec,
        int level,
        FortBuildStatus expected
    )
    {
        DbFortBuild entity =
            new()
            {
                ViewerId = 1,
                Level = level,
                BuildStartDate = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(startOffsetSec),
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(endOffsetSec)
            };

        entity.BuildStatus.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-1, 0)]
    [InlineData(100, 100)]
    [InlineData(-100, 0)]
    public void RemainTime_CalculatedCorrectly(int endOffsetSec, int expectedSec)
    {
        DbFortBuild entity =
            new()
            {
                ViewerId = 1,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(endOffsetSec)
            };

        entity.RemainTime.Should().BeCloseTo(TimeSpan.FromSeconds(expectedSec), AssertionTolerance);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void LastIncomeTime_CalculatedCorrectly(int incomeOffsetSec)
    {
        DbFortBuild entity =
            new()
            {
                ViewerId = 1,
                LastIncomeDate = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(incomeOffsetSec)
            };

        entity
            .LastIncomeTime.Should()
            .BeCloseTo(TimeSpan.FromSeconds(-incomeOffsetSec), AssertionTolerance);
    }
}
