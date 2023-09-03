using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.SummonController"/>
/// </summary>
public class SummonTest : TestFixture
{
    public SummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        SummonExcludeGetListData response = (
            await this.Client.PostMsgpack<SummonExcludeGetListData>(
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
            await this.Client.PostMsgpack<SummonGetOddsDataData>(
                "summon/get_odds_data",
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

        await this.ApiContext.PlayerSummonHistory.AddAsync(historyEntry);
        await this.ApiContext.SaveChangesAsync();

        SummonGetSummonHistoryData response = (
            await this.Client.PostMsgpack<SummonGetSummonHistoryData>(
                "summon/get_summon_history",
                new SummonGetSummonHistoryRequest()
            )
        ).data;

        // Too lazy to set up automapper to check exact result and it is covered more or less in SummonRepositoryTests.cs
        response.summon_history_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsAnyData()
    {
        SummonGetSummonListData response = (
            await this.Client.PostMsgpack<SummonGetSummonListData>(
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
            await this.Client.PostMsgpack<SummonGetSummonPointTradeData>(
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
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(
            x => x.DeviceAccountId == DeviceAccountId
        );

        await this.ApiContext.Entry(userData).ReloadAsync();

        SummonRequestData response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1,
                    SummonExecTypes.Single,
                    1,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal, 120) // TODO: Change when banners are implemented otherwise this test breaks
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
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(
            x => x.DeviceAccountId == DeviceAccountId
        );

        SummonRequestData response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal, 1200)
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
        if (reward.entity_type == EntityTypes.Dragon)
        {
            List<DbPlayerDragonData> dragonData = await this.ApiContext.PlayerDragonData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .ToListAsync();

            dragonData.Where(x => (int)x.DragonId == reward.id).Should().NotBeEmpty();
        }
        else
        {
            List<DbPlayerCharaData> charaData = await this.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .ToListAsync();

            charaData.Where(x => (int)x.CharaId == reward.id).Should().NotBeEmpty();
        }
    }
}
