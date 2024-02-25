using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Summoning;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="SummonController"/>
/// </summary>
public class SummonTest : TestFixture
{
    public SummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        SummonExcludeGetListResponse response = (
            await this.Client.PostMsgpack<SummonExcludeGetListResponse>(
                "summon_exclude/get_list",
                new SummonExcludeGetListRequest(1020203)
            )
        ).data;

        response.SummonExcludeUnitList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetOddsData_ReturnsAnyData()
    {
        SummonGetOddsDataResponse response = (
            await this.Client.PostMsgpack<SummonGetOddsDataResponse>(
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

        await this.ApiContext.PlayerSummonHistory.AddAsync(historyEntry);
        await this.ApiContext.SaveChangesAsync();

        SummonGetSummonHistoryResponse response = (
            await this.Client.PostMsgpack<SummonGetSummonHistoryResponse>(
                "summon/get_summon_history",
                new SummonGetSummonHistoryRequest()
            )
        ).data;

        // Too lazy to set up automapper to check exact result and it is covered more or less in SummonRepositoryTests.cs
        response.SummonHistoryList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsDataWithBannerInformation()
    {
        int bannerId = 1020010;
        int dailyCount = 1;
        int summonCount = 10;

        await this.AddToDatabase(
            new DbPlayerBannerData()
            {
                SummonBannerId = bannerId,
                DailyLimitedSummonCount = dailyCount,
                SummonCount = summonCount
            }
        );

        await this.AddToDatabase(
            new DbSummonTicket()
            {
                SummonTicketId = SummonTickets.SingleSummon,
                KeyId = 2,
                Quantity = 1,
                UseLimitTime = DateTimeOffset.UnixEpoch
            }
        );

        SummonGetSummonListResponse response = (
            await this.Client.PostMsgpack<SummonGetSummonListResponse>(
                "summon/get_summon_list",
                new SummonGetSummonListRequest()
            )
        ).data;

        response
            .SummonList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new SummonList()
                {
                    SummonId = bannerId,
                    SummonType = 2,
                    SingleCrystal = 120,
                    SingleDiamond = 120,
                    MultiCrystal = 1200,
                    MultiDiamond = 1200,
                    LimitedCrystal = 0,
                    LimitedDiamond = 30,
                    SummonPointId = bannerId,
                    AddSummonPoint = 1,
                    AddSummonPointStone = 2,
                    ExchangeSummonPoint = 300,
                    Status = 1,
                    CommenceDate = DateTimeOffset.Parse("2024-02-24T15:22:06Z"),
                    CompleteDate = DateTimeOffset.Parse("2037-02-24T15:22:06Z"),
                    DailyCount = dailyCount,
                    DailyLimit = 1,
                    TotalLimit = 0,
                    TotalCount = summonCount,
                    CampaignType = 0,
                    FreeCountRest = 0,
                    IsBeginnerCampaign = 0,
                    BeginnerCampaignCountRest = 0,
                    ConsecutionCampaignCountRest = 0,
                }
            );

        response
            .SummonTicketList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new SummonTicketList()
                {
                    SummonTicketId = SummonTickets.SingleSummon,
                    KeyId = 2,
                    Quantity = 1,
                    UseLimitTime = DateTimeOffset.UnixEpoch
                }
            );
    }

    [Fact]
    public async Task SummonRequest_GetSummonPointData_ReturnsAnyData()
    {
        SummonGetSummonPointTradeResponse response = (
            await this.Client.PostMsgpack<SummonGetSummonPointTradeResponse>(
                "summon/get_summon_point_trade",
                new SummonGetSummonPointTradeRequest(1020203)
            )
        ).data;

        response.Should().NotBeNull();

        response.SummonPointList.Should().NotBeEmpty();
        response.SummonPointTradeList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonRequest_SingleSummonWyrmite_ReturnsValidResult()
    {
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == ViewerId
        );

        await this.ApiContext.Entry(userData).ReloadAsync();

        SummonRequestResponse response = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
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

        response.ResultUnitList.Count().Should().Be(1);

        await this.CheckRewardInDb(response.ResultUnitList.ElementAt(0));
    }

    [Fact]
    public async Task SummonRequest_TenSummonWyrmite_ReturnsValidResult()
    {
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == ViewerId
        );

        SummonRequestResponse response = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
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

        response.ResultUnitList.Count().Should().Be(10);

        foreach (AtgenResultUnitList reward in response.ResultUnitList)
        {
            await this.CheckRewardInDb(reward);
        }
    }

    [Fact]
    public async Task SummonRequest_SingleSummonTicket_ReturnsValidResult()
    {
        await this.AddToDatabase(
            new DbSummonTicket()
            {
                SummonTicketId = SummonTickets.SingleSummon,
                KeyId = 1,
                Quantity = 1
            },
            new DbSummonTicket()
            {
                SummonTicketId = SummonTickets.SingleSummon,
                KeyId = 2,
                Quantity = 1
            }
        );

        DragaliaResponse<SummonRequestData> response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Single,
                    1,
                    PaymentTypes.Ticket,
                    new PaymentTarget(1, 1)
                ),
                ensureSuccessHeader: false
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task SummonRequest_MultiSingleSummonTicket_ReturnsValidResult()
    {
        await this.AddToDatabase(
            new DbSummonTicket()
            {
                SummonTicketId = SummonTickets.SingleSummon,
                KeyId = 1,
                Quantity = 5
            }
        );

        DragaliaResponse<SummonRequestData> response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Single,
                    5,
                    PaymentTypes.Ticket,
                    new PaymentTarget(5, 5)
                ),
                ensureSuccessHeader: false
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task SummonRequest_TenfoldSummonTicket_ReturnsValidResult()
    {
        await this.AddToDatabase(
            new DbSummonTicket()
            {
                SummonTicketId = SummonTickets.TenfoldSummon,
                KeyId = 1,
                Quantity = 1,
            }
        );

        DragaliaResponse<SummonRequestData> response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Ticket,
                    new PaymentTarget(1, 1)
                ),
                ensureSuccessHeader: false
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Theory]
    [InlineData(SummonExecTypes.Tenfold)]
    [InlineData(SummonExecTypes.Single)]
    public async Task SummonRequest_SummonTicket_NoMaterials_ReturnsMaterialShort(
        SummonExecTypes types
    )
    {
        await this.AddToDatabase(
            new DbSummonTicket() { SummonTicketId = SummonTickets.TenfoldSummon, KeyId = 1, }
        );

        DragaliaResponse<SummonRequestData> response = (
            await this.Client.PostMsgpack<SummonRequestData>(
                "summon/request",
                new SummonRequestRequest(
                    1020203,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Ticket,
                    new PaymentTarget(0, 1)
                ),
                ensureSuccessHeader: false
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.CommonMaterialShort);
    }

    private async Task CheckRewardInDb(AtgenResultUnitList reward)
    {
        if (reward.EntityType == EntityTypes.Dragon)
        {
            List<DbPlayerDragonData> dragonData = await this
                .ApiContext.PlayerDragonData.Where(x => x.ViewerId == ViewerId)
                .ToListAsync();

            dragonData.Where(x => (int)x.DragonId == reward.Id).Should().NotBeEmpty();
        }
        else
        {
            List<DbPlayerCharaData> charaData = await this
                .ApiContext.PlayerCharaData.Where(x => x.ViewerId == ViewerId)
                .ToListAsync();

            charaData.Where(x => (int)x.CharaId == reward.Id).Should().NotBeEmpty();
        }
    }
}
