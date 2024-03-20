using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class SummonRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly SummonRepository summonRepository;

    public SummonRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.summonRepository = new SummonRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object
        );

        AssertionOptions.AssertEquivalencyUsing(options =>
            options.Excluding(x => x.Name == "Owner")
        );
    }

    [Fact]
    public async Task GetSummonHistory_ReturnsOnlyPlayerSummonHistory()
    {
        DbPlayerSummonHistory history =
            new()
            {
                ViewerId = ViewerId,
                SummonId = 1,
                SummonExecType = SummonExecTypes.DailyDeal,
                ExecDate = DateTimeOffset.UtcNow,
                PaymentType = PaymentTypes.Diamantium,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)Dragons.GalaRebornNidhogg,
                EntityQuantity = 1,
                EntityLevel = 1,
                EntityRarity = 5,
                EntityLimitBreakCount = 0,
                EntityHpPlusCount = 0,
                EntityAttackPlusCount = 0,
                SummonPrizeRank = SummonPrizeRanks.None,
                SummonPoint = 10,
                GetDewPointQuantity = 0,
            };

        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerSummonHistory>()
            {
                history,
                new()
                {
                    ViewerId = 2,
                    SummonId = 1,
                    SummonExecType = SummonExecTypes.DailyDeal,
                    ExecDate = DateTimeOffset.UtcNow,
                    PaymentType = PaymentTypes.Diamantium,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.GalaRebornNidhogg,
                    EntityQuantity = 1,
                    EntityLevel = 1,
                    EntityRarity = 5,
                    EntityLimitBreakCount = 0,
                    EntityHpPlusCount = 0,
                    EntityAttackPlusCount = 0,
                    SummonPrizeRank = SummonPrizeRanks.None,
                    SummonPoint = 10,
                    GetDewPointQuantity = 0,
                }
            }
        );

        (await this.summonRepository.SummonHistory.ToListAsync())
            .Should()
            .BeEquivalentTo(new List<DbPlayerSummonHistory>() { history });
    }

    [Fact]
    public async Task GetPlayerBannerData_ReturnsOnlyPlayerBannerDataWithRightId()
    {
        DbPlayerBannerData bannerData = DbPlayerBannerDataFactory.Create(1, 1);
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerBannerData>()
            {
                bannerData,
                DbPlayerBannerDataFactory.Create(2, 2),
                DbPlayerBannerDataFactory.Create(3, 1)
            }
        );

        (await this.summonRepository.GetPlayerBannerData(1)).Should().BeEquivalentTo(bannerData);
    }

    [Fact]
    public async Task GetPlayerBannerData_AddsIfNotFound()
    {
        (await this.summonRepository.GetPlayerBannerData(10))
            .Should()
            .BeEquivalentTo(
                DbPlayerBannerDataFactory.Create(ViewerId, 10),
                options =>
                    options
                        .Excluding(x => x.ConsecutionSummonPointsMinDate)
                        .Excluding(x => x.ConsecutionSummonPointsMaxDate)
            );
    }
}
