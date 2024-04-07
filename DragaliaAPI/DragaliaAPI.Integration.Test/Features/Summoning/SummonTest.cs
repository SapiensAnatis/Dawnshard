using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Summoning;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Summoning;

/// <summary>
/// Tests <see cref="SummonController"/>
/// </summary>
public class SummonTest : TestFixture
{
    private const int TestBannerId = 1020121;

    public SummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        SummonExcludeGetListResponse response = (
            await this.Client.PostMsgpack<SummonExcludeGetListResponse>(
                "summon_exclude/get_list",
                new SummonExcludeGetListRequest(TestBannerId)
            )
        ).Data;

        response.SummonExcludeUnitList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetOddsData_ReturnsExpectedData()
    {
        SummonGetOddsDataResponse response = (
            await this.Client.PostMsgpack<SummonGetOddsDataResponse>(
                "summon/get_odds_data",
                new SummonGetOddsDataRequest(TestBannerId)
            )
        ).Data;

        OddsRate normalOdds = response.OddsRateList.Normal;
        OddsRate guaranteeOdds = response.OddsRateList.Guarantee;

        normalOdds
            .RarityList.Should()
            .BeEquivalentTo(
                [
                    new AtgenRarityList { Rarity = 5, TotalRate = "4.00%" },
                    new AtgenRarityList { Rarity = 4, TotalRate = "16.00%" },
                    new AtgenRarityList { Rarity = 3, TotalRate = "80.00%" },
                ]
            );

        guaranteeOdds
            .RarityList.Should()
            .BeEquivalentTo(
                [
                    new AtgenRarityList { Rarity = 5, TotalRate = "4.00%" },
                    new AtgenRarityList { Rarity = 4, TotalRate = "96.00%" },
                ]
            );

        normalOdds
            .RarityGroupList.Should()
            .BeEquivalentTo(
                [
                    new AtgenRarityGroupList
                    {
                        Rarity = 5,
                        CharaRate = "1.00%",
                        DragonRate = "0.80%",
                        Pickup = true,
                        TotalRate = "1.80%"
                    },
                    new AtgenRarityGroupList
                    {
                        Rarity = 5,
                        CharaRate = "1.10%",
                        DragonRate = "1.10%",
                        Pickup = false,
                        TotalRate = "2.20%"
                    },
                    new AtgenRarityGroupList
                    {
                        Rarity = 4,
                        CharaRate = "8.55%",
                        DragonRate = "7.45%",
                        Pickup = false,
                        TotalRate = "16.00%"
                    },
                    new AtgenRarityGroupList
                    {
                        Rarity = 3,
                        CharaRate = "48.00%",
                        DragonRate = "32.00%",
                        Pickup = false,
                        TotalRate = "80.00%"
                    }
                ]
            );

        guaranteeOdds
            .RarityGroupList.Should()
            .BeEquivalentTo(
                [
                    new AtgenRarityGroupList
                    {
                        Rarity = 5,
                        CharaRate = "1.00%",
                        DragonRate = "0.80%",
                        Pickup = true,
                        TotalRate = "1.80%"
                    },
                    new AtgenRarityGroupList
                    {
                        Rarity = 5,
                        CharaRate = "1.10%",
                        DragonRate = "1.10%",
                        Pickup = false,
                        TotalRate = "2.20%"
                    },
                    new AtgenRarityGroupList
                    {
                        Rarity = 4,
                        CharaRate = "51.30%",
                        DragonRate = "44.70%",
                        Pickup = false,
                        TotalRate = "96.00%"
                    }
                ]
            );

        normalOdds.Unit.CharaOddsList.Should().HaveCount(4);

        normalOdds
            .Unit.CharaOddsList.ElementAt(0)
            .UnitList.Should()
            .BeEquivalentTo(
                [
                    new AtgenUnitList { Id = (int)Charas.Joker, Rate = "0.500%" },
                    new AtgenUnitList { Id = (int)Charas.Mona, Rate = "0.500%" }
                ]
            );
        normalOdds
            .Unit.DragonOddsList.ElementAt(0)
            .UnitList.Should()
            .BeEquivalentTo([new AtgenUnitList { Id = (int)Dragons.Arsene, Rate = "0.800%" }]);
    }

    [Fact]
    public async Task SummonGetSummonHistory_ReturnsAnyData()
    {
        DbPlayerSummonHistory historyEntry =
            new()
            {
                ViewerId = this.ViewerId,
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
                "summon/get_summon_history"
            )
        ).Data;

        // Too lazy to set up automapper to check exact result and it is covered more or less in SummonRepositoryTests.cs
        response.SummonHistoryList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsDataWithBannerInformation()
    {
        int dailyCount = 1;
        int summonCount = 10;

        await this.AddToDatabase(
            new DbPlayerBannerData()
            {
                SummonBannerId = TestBannerId,
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
            await this.Client.PostMsgpack<SummonGetSummonListResponse>("summon/get_summon_list")
        ).Data;

        response
            .SummonList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new SummonList()
                {
                    SummonId = TestBannerId,
                    SummonType = 2,
                    SingleCrystal = 120,
                    SingleDiamond = 120,
                    MultiCrystal = 1200,
                    MultiDiamond = 1200,
                    LimitedCrystal = 0,
                    LimitedDiamond = 30,
                    SummonPointId = TestBannerId,
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
                    IsBeginnerCampaign = false,
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
    public async Task SummonGetSummonPointTrade_NoBannerData_CreatesDefaultData()
    {
        this.ApiContext.PlayerBannerData.Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.SummonBannerId == TestBannerId);

        SummonGetSummonPointTradeResponse response = (
            await this.Client.PostMsgpack<SummonGetSummonPointTradeResponse>(
                "summon/get_summon_point_trade",
                new SummonGetSummonPointTradeRequest(TestBannerId)
            )
        ).Data;

        response
            .Should()
            .BeEquivalentTo(
                new SummonGetSummonPointTradeResponse()
                {
                    SummonPointTradeList =
                    [
                        new()
                        {
                            TradeId = int.Parse($"{TestBannerId}100"),
                            EntityId = (int)Charas.Mona,
                            EntityType = EntityTypes.Chara
                        },
                        new()
                        {
                            TradeId = int.Parse($"{TestBannerId}700"),
                            EntityId = (int)Dragons.Arsene,
                            EntityType = EntityTypes.Dragon
                        }
                    ],
                    SummonPointList = [new() { SummonPointId = TestBannerId, SummonPoint = 0, }],
                    UpdateDataList = new()
                    {
                        SummonPointList = [new() { SummonPointId = TestBannerId, SummonPoint = 0, }]
                    },
                    EntityResult = new(),
                },
                opts => opts.Excluding(x => x.Name.Contains("CsPoint"))
            );

        this.ApiContext.PlayerBannerData.Should()
            .Contain(x => x.ViewerId == this.ViewerId && x.SummonBannerId == TestBannerId);
    }

    [Fact]
    public async Task SummonGetSummonPointTrade_ExistingBannerData_ReturnsData()
    {
        await this.AddToDatabase(
            new DbPlayerBannerData() { SummonBannerId = TestBannerId, SummonPoints = 400, }
        );

        SummonGetSummonPointTradeResponse response = (
            await this.Client.PostMsgpack<SummonGetSummonPointTradeResponse>(
                "summon/get_summon_point_trade",
                new SummonGetSummonPointTradeRequest(TestBannerId)
            )
        ).Data;

        response
            .Should()
            .BeEquivalentTo(
                new SummonGetSummonPointTradeResponse()
                {
                    SummonPointTradeList =
                    [
                        new()
                        {
                            TradeId = int.Parse($"{TestBannerId}100"),
                            EntityId = (int)Charas.Mona,
                            EntityType = EntityTypes.Chara
                        },
                        new()
                        {
                            TradeId = int.Parse($"{TestBannerId}700"),
                            EntityId = (int)Dragons.Arsene,
                            EntityType = EntityTypes.Dragon
                        }
                    ],
                    SummonPointList = [new() { SummonPointId = TestBannerId, SummonPoint = 400, }],
                    UpdateDataList = new(),
                    EntityResult = new(),
                },
                opts => opts.Excluding(x => x.Name.Contains("CsPoint"))
            );
    }

    [Fact]
    public async Task SummonRequest_SingleSummonWyrmite_ReturnsValidResult()
    {
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == this.ViewerId
        );

        await this.ApiContext.Entry(userData).ReloadAsync();

        SummonRequestResponse response = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Single,
                    1,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal, 120) // TODO: Change when banners are implemented otherwise this test breaks
                )
            )
        ).Data;

        response.ResultUnitList.Count().Should().Be(1);

        await this.CheckRewardInDb(response.ResultUnitList.ElementAt(0));
    }

    [Fact]
    public async Task SummonRequest_TenSummonWyrmite_ReturnsValidResult()
    {
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == this.ViewerId
        );

        SummonRequestResponse response = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal, 1200)
                )
            )
        ).Data;

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

        DragaliaResponse<SummonRequestResponse> response =
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Single,
                    1,
                    PaymentTypes.Ticket,
                    new PaymentTarget(1, 1)
                ),
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
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

        DragaliaResponse<SummonRequestResponse> response =
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Single,
                    5,
                    PaymentTypes.Ticket,
                    new PaymentTarget(5, 5)
                ),
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
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

        DragaliaResponse<SummonRequestResponse> response =
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Tenfold,
                    0,
                    PaymentTypes.Ticket,
                    new PaymentTarget(1, 1)
                ),
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task SummonRequest_IncrementsWyrmsigilPoints()
    {
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == this.ViewerId
        );

        SummonRequestResponse response = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Tenfold,
                    1,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal, 1200)
                )
            )
        ).Data;

        response.ResultSummonPoint.Should().Be(10);
        response
            .UpdateDataList.SummonPointList.Should()
            .Contain(x => x.SummonPointId == TestBannerId && x.SummonPoint == 10);

        SummonRequestResponse singleResponse = (
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    SummonExecTypes.Single,
                    3,
                    PaymentTypes.Wyrmite,
                    new PaymentTarget(userData.Crystal - 1200, 360)
                )
            )
        ).Data;

        singleResponse.ResultSummonPoint.Should().Be(3);
        singleResponse
            .UpdateDataList.SummonPointList.Should()
            .Contain(x => x.SummonPointId == TestBannerId && x.SummonPoint == 13);
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

        DragaliaResponse<SummonRequestResponse> response =
            await this.Client.PostMsgpack<SummonRequestResponse>(
                "summon/request",
                new SummonRequestRequest(
                    TestBannerId,
                    types,
                    0,
                    PaymentTypes.Ticket,
                    new PaymentTarget(0, 1)
                ),
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.CommonMaterialShort);
    }

    [Fact]
    public async Task SummonPointTrade_Chara_Success_ReturnsData()
    {
        int monaTradeId = int.Parse($"{TestBannerId}100");

        await this.AddToDatabase(
            new DbPlayerBannerData() { SummonBannerId = TestBannerId, SummonPoints = 400, }
        );

        SummonSummonPointTradeResponse response = (
            await this.Client.PostMsgpack<SummonSummonPointTradeResponse>(
                "summon/summon_point_trade",
                new SummonSummonPointTradeRequest(TestBannerId, monaTradeId)
            )
        ).Data;

        response
            .ExchangeEntityList.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityId = (int)Charas.Mona,
                    EntityType = EntityTypes.Chara,
                    EntityQuantity = 1,
                }
            );

        response.UpdateDataList.CharaList.Should().Contain(x => x.CharaId == Charas.Mona);

        this.ApiContext.PlayerCharaData.Should()
            .Contain(x => x.ViewerId == this.ViewerId && x.CharaId == Charas.Mona);
    }

    [Fact]
    public async Task SummonPointTrade_Dragon_Success_ReturnsData()
    {
        int arseneTradeId = int.Parse($"{TestBannerId}700");

        await this.AddToDatabase(
            new DbPlayerBannerData() { SummonBannerId = TestBannerId, SummonPoints = 400, }
        );

        SummonSummonPointTradeResponse response = (
            await this.Client.PostMsgpack<SummonSummonPointTradeResponse>(
                "summon/summon_point_trade",
                new SummonSummonPointTradeRequest(TestBannerId, arseneTradeId)
            )
        ).Data;

        response
            .ExchangeEntityList.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityId = (int)Dragons.Arsene,
                    EntityType = EntityTypes.Dragon,
                    EntityQuantity = 1,
                }
            );

        response.UpdateDataList.DragonList.Should().Contain(x => x.DragonId == Dragons.Arsene);

        this.ApiContext.PlayerDragonData.Should()
            .Contain(x => x.ViewerId == this.ViewerId && x.DragonId == Dragons.Arsene);
    }

    private async Task CheckRewardInDb(AtgenResultUnitList reward)
    {
        if (reward.EntityType == EntityTypes.Dragon)
        {
            List<DbPlayerDragonData> dragonData = await this
                .ApiContext.PlayerDragonData.Where(x => x.ViewerId == this.ViewerId)
                .ToListAsync();

            dragonData.Where(x => (int)x.DragonId == reward.Id).Should().NotBeEmpty();
        }
        else
        {
            List<DbPlayerCharaData> charaData = await this
                .ApiContext.PlayerCharaData.Where(x => x.ViewerId == this.ViewerId)
                .ToListAsync();

            charaData.Where(x => (int)x.CharaId == reward.Id).Should().NotBeEmpty();
        }
    }
}
