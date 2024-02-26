using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Login;

public class LoginTest : TestFixture
{
    public LoginTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public void IDailyResetAction_HasExpectedCount()
    {
        // Update this test when adding a new reset action
        this.Services.GetServices<IDailyResetAction>().Should().HaveCount(5);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsItemSummonCount()
    {
        await this
            .ApiContext.PlayerShopInfos.Where(x => x.ViewerId == ViewerId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.DailySummonCount, 5));

        (await this.GetSummonCount()).Should().Be(5);

        await this.Client.PostMsgpack<LoginIndexResponse>("/login/index", new LoginIndexRequest());

        (await this.GetSummonCount()).Should().Be(0);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsDragonGiftCount()
    {
        await this
            .ApiContext.PlayerDragonGifts.Where(x => x.ViewerId == ViewerId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.Quantity, 0));

        (await this.GetDragonGifts()).Should().AllSatisfy(x => x.Quantity.Should().Be(0));

        await this.Client.PostMsgpack<LoginIndexResponse>("/login/index", new LoginIndexRequest());

        (await this.GetDragonGifts())
            .Should()
            .BeEquivalentTo(
                new List<DbPlayerDragonGift>()
                {
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.FreshBread,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.TastyMilk,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.StrawberryTart,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.HeartyStew,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.Kaleidoscope,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.FloralCirclet,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.CompellingBook,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.JuicyMeat,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.ManaEssence,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.GoldenChalice,
                        Quantity = 1
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.FourLeafClover,
                        Quantity = 0
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.DragonyuleCake,
                        Quantity = 0
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.ValentinesCard,
                        Quantity = 0
                    },
                    new()
                    {
                        ViewerId = ViewerId,
                        DragonGiftId = DragonGifts.PupGrub,
                        Quantity = 0
                    }
                }
            );
    }

    [Fact]
    public async Task LoginIndex_GrantsLoginBonusBasedOnDb_GrantsEachDayReward()
    {
        /*
        int oldSkipTickets = (
            await this.ApiContext.PlayerUserData
                .AsNoTracking()
                .FirstAsync(x => x.ViewerId == ViewerId)
        ).QuestSkipPoint;
        */

        await this.AddToDatabase(
            new DbLoginBonus()
            {
                ViewerId = ViewerId,
                CurrentDay = 4,
                Id = 17 // Standard daily login bonus
            }
        );

        DragaliaResponse<LoginIndexResponse> response =
            await this.Client.PostMsgpack<LoginIndexResponse>(
                "/login/index",
                new LoginIndexRequest()
            );

        response
            .Data.LoginBonusList.Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    LoginBonusId = 17,
                    RewardDay = 5,
                    TotalLoginDay = 5,
                    EntityType = EntityTypes.Rupies,
                    EntityQuantity = 30_000,
                }
            );

        // Skip tickets can't yet be rewarded
        // response.data.update_data_list.user_data.quest_skip_point.Should().Be(oldSkipTickets + 12);
    }

    [Fact]
    public async Task LoginIndex_LoginBonusLastDay_IsLoopTrue_RollsOver()
    {
        await this.AddToDatabase(
            new DbLoginBonus()
            {
                ViewerId = ViewerId,
                CurrentDay = 10,
                Id = 17 // Standard daily login bonus
            }
        );

        DragaliaResponse<LoginIndexResponse> response =
            await this.Client.PostMsgpack<LoginIndexResponse>(
                "/login/index",
                new LoginIndexRequest()
            );

        response
            .Data.LoginBonusList.Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    LoginBonusId = 17,
                    RewardDay = 1,
                    TotalLoginDay = 1,
                    EntityType = EntityTypes.Material,
                    EntityId = 101001003,
                    EntityQuantity = 10,
                }
            );
    }

    [Fact]
    public async Task LoginIndex_LoginBonusLastDay_IsLoopFalse_SetsIsComplete()
    {
        this.MockDateTimeProvider.SetupGet(x => x.UtcNow)
            .Returns(DateTime.Parse("2018/09/28").ToUniversalTime());

        await this.AddToDatabase(
            new DbLoginBonus()
            {
                ViewerId = ViewerId,
                CurrentDay = 6,
                Id = 2 // Launch Celebration Daily Bonus
            }
        );

        DragaliaResponse<LoginIndexResponse> response =
            await this.Client.PostMsgpack<LoginIndexResponse>(
                "/login/index",
                new LoginIndexRequest()
            );

        response
            .Data.LoginBonusList.Should()
            .ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    LoginBonusId = 2,
                    RewardDay = 7,
                    TotalLoginDay = 7,
                    EntityType = EntityTypes.Wyrmite,
                    EntityId = 0,
                    EntityQuantity = 150,
                }
            );

        (
            await this
                .ApiContext.LoginBonuses.AsNoTracking()
                .FirstAsync(x => x.ViewerId == ViewerId && x.Id == 2)
        )
            .IsComplete.Should()
            .BeTrue();

        DragaliaResponse<LoginIndexResponse> secondResponse =
            await this.Client.PostMsgpack<LoginIndexResponse>(
                "/login/index",
                new LoginIndexRequest()
            );

        secondResponse.Data.LoginBonusList.Should().NotContain(x => x.LoginBonusId == 2);
    }

    [Fact]
    public async Task LoginIndex_DragonGift_GrantsReward()
    {
        await this.AddToDatabase(
            new DbLoginBonus()
            {
                ViewerId = ViewerId,
                Id = 17,
                CurrentDay = 7,
            }
        );

        int oldCloverQuantity = await this
            .ApiContext.PlayerDragonGifts.AsNoTracking()
            .Where(x => x.DragonGiftId == DragonGifts.FourLeafClover && x.ViewerId == ViewerId)
            .Select(x => x.Quantity)
            .FirstAsync();

        LoginIndexResponse response = (
            await this.Client.PostMsgpack<LoginIndexResponse>(
                "login/index",
                new LoginIndexRequest() { JwsResult = string.Empty }
            )
        ).Data;

        response
            .LoginBonusList.Should()
            .ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    EntityId = 30001,
                    EntityQuantity = 3,
                    EntityType = EntityTypes.DragonGift,
                    EntityLevel = 0,
                    EntityLimitBreakCount = 0,
                    LoginBonusId = 17,
                    RewardDay = 8,
                    TotalLoginDay = 8
                }
            );

        response
            .UpdateDataList.DragonGiftList.Should()
            .Contain(x =>
                x.DragonGiftId == DragonGifts.FourLeafClover && x.Quantity == oldCloverQuantity + 3
            );
    }

    [Fact]
    public async Task LoginIndex_AddsNewDailyEndeavours()
    {
        int oldMissionId = 1;
        int starryDragonyuleEventId = 22903;

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = oldMissionId,
                Type = MissionType.Daily
            }
        );
        await this.AddToDatabase(
            new DbPlayerEventData() { ViewerId = this.ViewerId, EventId = starryDragonyuleEventId }
        );

        await this.Client.PostMsgpack(
            "login/index",
            new LoginIndexRequest() { JwsResult = string.Empty }
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Should()
            .NotContain(x => x.Id == oldMissionId);

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.GroupId == 0)
            .Should()
            .BeEquivalentTo(
                [
                    new DbPlayerMission() { Id = 15070101 },
                    new DbPlayerMission() { Id = 15070201 },
                    new DbPlayerMission() { Id = 15070301 },
                    new DbPlayerMission() { Id = 15070401 },
                    new DbPlayerMission() { Id = 15070501 },
                    new DbPlayerMission() { Id = 15070601 },
                ],
                opts => opts.Including(x => x.Id),
                "these are the standard daily endeavours"
            );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.GroupId == starryDragonyuleEventId)
            .Should()
            .BeEquivalentTo(
                [
                    new DbPlayerMission() { Id = 11190101 },
                    new DbPlayerMission() { Id = 11190102 },
                    new DbPlayerMission() { Id = 11190103 },
                    new DbPlayerMission() { Id = 11190104 },
                    new DbPlayerMission() { Id = 11190105 },
                    new DbPlayerMission() { Id = 11190201 },
                    new DbPlayerMission() { Id = 11190202 },
                    new DbPlayerMission() { Id = 11190301 },
                ],
                opts => opts.Including(x => x.Id),
                "these are the event daily endeavours"
            );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .ToList()
            .Should()
            .AllSatisfy(x =>
            {
                x.Type.Should().Be(MissionType.Daily);
                x.Start.Should().Be(this.LastDailyReset);
                x.End.Should().Be(this.LastDailyReset.AddDays(1));
            });
    }

    [Fact]
    public async Task LoginIndex_EventNotStarted_DoesAddEventDailyEndeavours()
    {
        int oldMissionId = 1;
        int starryDragonyuleEventId = 22903;

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = oldMissionId,
                Type = MissionType.Daily
            }
        );

        await this.Client.PostMsgpack(
            "login/index",
            new LoginIndexRequest() { JwsResult = string.Empty }
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Should()
            .NotContain(x => x.Id == oldMissionId);

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.GroupId == 0)
            .Should()
            .BeEquivalentTo(
                [
                    new DbPlayerMission() { Id = 15070101 },
                    new DbPlayerMission() { Id = 15070201 },
                    new DbPlayerMission() { Id = 15070301 },
                    new DbPlayerMission() { Id = 15070401 },
                    new DbPlayerMission() { Id = 15070501 },
                    new DbPlayerMission() { Id = 15070601 },
                ],
                opts => opts.Including(x => x.Id),
                "these are the standard daily endeavours"
            );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Should()
            .NotContain(x => x.GroupId == starryDragonyuleEventId);
    }

    [Fact]
    public async Task LoginVerifyJws_ReturnsOK()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "/login/verify_jws",
                new LoginVerifyJwsRequest()
            )
        ).Data;

        response.Should().BeEquivalentTo(new ResultCodeResponse(ResultCode.Success, string.Empty));
    }

    private async Task<int> GetSummonCount() =>
        (
            await this
                .ApiContext.PlayerShopInfos.AsNoTracking()
                .FirstAsync(x => x.ViewerId == ViewerId)
        ).DailySummonCount;

    private async Task<IEnumerable<DbPlayerDragonGift>> GetDragonGifts() =>
        await this
            .ApiContext.PlayerDragonGifts.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .ToListAsync();
}
