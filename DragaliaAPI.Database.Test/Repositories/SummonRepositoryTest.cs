﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class SummonRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly ISummonRepository summonRepository;

    public SummonRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.summonRepository = new SummonRepository(this.fixture.ApiContext);

        AssertionOptions.AssertEquivalencyUsing(
            options => options.Excluding(x => x.Name == "Owner")
        );
    }

    [Fact]
    public async Task GetSummonHistory_ReturnsOnlyPlayerSummonHistory()
    {
        DbPlayerSummonHistory history =
            new()
            {
                DeviceAccountId = DeviceAccountId,
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
                    DeviceAccountId = "id 2",
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

        (await this.summonRepository.GetSummonHistory(DeviceAccountId))
            .Should()
            .BeEquivalentTo(new List<DbPlayerSummonHistory>() { history });
    }

    [Fact]
    public async Task GetPlayerBannerData_ReturnsOnlyPlayerBannerDataWithRightId()
    {
        DbPlayerBannerData bannerData = DbPlayerBannerDataFactory.Create(DeviceAccountId, 1);
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerBannerData>()
            {
                bannerData,
                DbPlayerBannerDataFactory.Create(DeviceAccountId, 2),
                DbPlayerBannerDataFactory.Create("id 2", 1)
            }
        );

        (await this.summonRepository.GetPlayerBannerData(DeviceAccountId, 1))
            .Should()
            .BeEquivalentTo(bannerData);
    }

    [Fact]
    public async Task GetPlayerBannerData_AddsIfNotFound()
    {
        (await this.summonRepository.GetPlayerBannerData(DeviceAccountId, 10))
            .Should()
            .BeEquivalentTo(
                DbPlayerBannerDataFactory.Create(DeviceAccountId, 10),
                options =>
                    options
                        .Excluding(x => x.ConsecutionSummonPointsMinDate)
                        .Excluding(x => x.ConsecutionSummonPointsMaxDate)
            );
    }
}
