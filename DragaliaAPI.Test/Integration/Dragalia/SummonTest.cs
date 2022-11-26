using MessagePack.Resolvers;
using MessagePack;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.SummonController"/>
/// </summary>
public class SummonTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;
    private readonly ITestOutputHelper outputHelper;

    public SummonTest(IntegrationTestFixture fixture, ITestOutputHelper outputHelper)
    {
        this.fixture = fixture;
        this.outputHelper = outputHelper;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        SummonExcludeGetListData response = (
            await client.PostMsgpack<SummonExcludeGetListData>(
                "summon_exclude/get_list",
                new SummonExcludeGetListRequest(1020203)
            )
        ).data;

        response.summon_exclude_unit_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetOddsData_ReturnsAnyData()
    {
        SummonGetOddsDataData response = (
            await client.PostMsgpack<SummonGetOddsDataData>(
                "summon/get_odds",
                new SummonGetOddsDataRequest(1020203)
            )
        ).data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task SummonGetSummonHistory_ReturnsAnyData()
    {
        DbPlayerSummonHistory historyEntry =
            new()
            {
                DeviceAccountId = fixture.DeviceAccountId,
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

        using (IServiceScope scope = fixture.Services.CreateScope())
        {
            ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();

            await context.PlayerSummonHistory.AddAsync(historyEntry);
            await context.SaveChangesAsync();
        }

        SummonGetSummonHistoryData response = (
            await client.PostMsgpack<SummonGetSummonHistoryData>("summon/get_odds", new { })
        ).data;

        // Too lazy to set up automapper to check exact result and it is covered more or less in SummonRepositoryTests.cs
        response.summon_history_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsAnyData()
    {
        SummonGetSummonListData response = (
            await client.PostMsgpack<SummonGetSummonListData>(
                "summon/get_summon_list",
                new SummonGetSummonListRequest()
            )
        ).data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task SummonRequest_GetSummonPointData_ReturnsAnyData()
    {
        SummonGetSummonPointTradeData response = (
            await client.PostMsgpack<SummonGetSummonPointTradeData>(
                "summon/get_summon_point_trade",
                new SummonGetSummonPointTradeRequest(1020203)
            )
        ).data;

        response.Should().NotBeNull();

        response.summon_point_list.Should().NotBeEmpty();
        response.summon_point_trade_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonRequest_SingleSummonWyrmite_ReturnsValidResult()
    {
        SummonRequestData response = (
            await client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1,
                    SummonExecTypes.Single,
                    1,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(1, 1)
                )
            )
        ).data;

        response.result_unit_list.Count().Should().Be(1);

        await this.CheckRewardInDb(response.result_unit_list.ElementAt(0));
    }

    /// Multisummon tests fail on testDB when saving 2+ new dragonData because sqlLite can't generate new Dragon_Key_Ids (always returns 0) via sequence
    /// TODO: Low priority since it works with the actual DB, but maybe figure out how to change the generation so it works in tests too
    [Fact]
    public async Task SummonRequest_TenSummonWyrmite_ReturnsValidResult()
    {
        SummonRequestData response = (
            await client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(1, 1)
                )
            )
        ).data;

        response.result_unit_list.Count().Should().Be(10);

        foreach (AtgenResultUnitList reward in response.result_unit_list)
        {
            await this.CheckRewardInDb(reward);
        }
    }

    private async Task CheckRewardInDb(AtgenResultUnitList reward)
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        if (reward.entity_type == (int)EntityTypes.Dragon)
        {
            List<DbPlayerDragonData> dragonData = await apiContext.PlayerDragonData
                .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
                .ToListAsync();

            dragonData.Where(x => (int)x.DragonId == reward.id).Should().NotBeEmpty();
        }
        else
        {
            List<DbPlayerCharaData> charaData = await apiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
                .ToListAsync();

            charaData.Where(x => (int)x.CharaId == reward.id).Should().NotBeEmpty();
        }
    }
}
