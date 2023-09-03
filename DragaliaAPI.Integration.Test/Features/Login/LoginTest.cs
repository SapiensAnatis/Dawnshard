using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Login;

public class LoginTest : TestFixture
{
    public LoginTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ResetLastLoginTime();
        this.ClearLoginBonuses();
    }

    [Fact]
    public void IDailyResetAction_HasExpectedCount()
    {
        // Update this test when adding a new reset action
        this.Services.GetServices<IDailyResetAction>().Should().HaveCount(4);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsItemSummonCount()
    {
        await this.ApiContext.PlayerShopInfos
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.DailySummonCount, 5));

        (await this.GetSummonCount()).Should().Be(5);

        await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest());

        (await this.GetSummonCount()).Should().Be(0);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsDragonGiftCount()
    {
        await this.ApiContext.PlayerDragonGifts
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.Quantity, 0));

        (await this.GetDragonGifts()).Should().AllSatisfy(x => x.Quantity.Should().Be(0));

        await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest());

        (await this.GetDragonGifts())
            .Should()
            .BeEquivalentTo(
                new List<DbPlayerDragonGift>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.FreshBread,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.TastyMilk,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.StrawberryTart,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.HeartyStew,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.Kaleidoscope,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.FloralCirclet,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.CompellingBook,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.JuicyMeat,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.ManaEssence,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.GoldenChalice,
                        Quantity = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.FourLeafClover,
                        Quantity = 0
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.DragonyuleCake,
                        Quantity = 0
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        DragonGiftId = DragonGifts.ValentinesCard,
                        Quantity = 0
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
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
                .FirstAsync(x => x.DeviceAccountId == DeviceAccountId)
        ).QuestSkipPoint;
        */

        await this.AddToDatabase(
            new DbLoginBonus()
            {
                DeviceAccountId = DeviceAccountId,
                CurrentDay = 4,
                Id = 17 // Standard daily login bonus
            }
        );

        DragaliaResponse<LoginIndexData> response = await this.Client.PostMsgpack<LoginIndexData>(
            "/login/index",
            new LoginIndexRequest()
        );

        response.data.login_bonus_list
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    login_bonus_id = 17,
                    reward_day = 5,
                    total_login_day = 5,
                    entity_type = EntityTypes.Rupies,
                    entity_quantity = 30_000,
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
                DeviceAccountId = DeviceAccountId,
                CurrentDay = 10,
                Id = 17 // Standard daily login bonus
            }
        );

        DragaliaResponse<LoginIndexData> response = await this.Client.PostMsgpack<LoginIndexData>(
            "/login/index",
            new LoginIndexRequest()
        );

        response.data.login_bonus_list
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    login_bonus_id = 17,
                    reward_day = 1,
                    total_login_day = 1,
                    entity_type = EntityTypes.Material,
                    entity_id = 101001003,
                    entity_quantity = 10,
                }
            );
    }

    [Fact]
    public async Task LoginIndex_LoginBonusLastDay_IsLoopFalse_SetsIsComplete()
    {
        this.MockDateTimeProvider
            .SetupGet(x => x.UtcNow)
            .Returns(DateTime.Parse("2018/09/28").ToUniversalTime());

        await this.AddToDatabase(
            new DbLoginBonus()
            {
                DeviceAccountId = DeviceAccountId,
                CurrentDay = 6,
                Id = 2 // Launch Celebration Daily Bonus
            }
        );

        DragaliaResponse<LoginIndexData> response = await this.Client.PostMsgpack<LoginIndexData>(
            "/login/index",
            new LoginIndexRequest()
        );

        response.data.login_bonus_list
            .Should()
            .ContainEquivalentOf(
                new AtgenLoginBonusList()
                {
                    login_bonus_id = 2,
                    reward_day = 7,
                    total_login_day = 7,
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = 150,
                }
            );

        (
            await this.ApiContext.LoginBonuses
                .AsNoTracking()
                .FirstAsync(x => x.DeviceAccountId == DeviceAccountId && x.Id == 2)
        ).IsComplete
            .Should()
            .BeTrue();

        this.ResetLastLoginTime();

        DragaliaResponse<LoginIndexData> secondResponse =
            await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest());

        secondResponse.data.login_bonus_list.Should().NotContain(x => x.login_bonus_id == 2);
    }

    [Fact]
    public async Task LoginVerifyJws_ReturnsOK()
    {
        ResultCodeData response = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "/login/verify_jws",
                new LoginVerifyJwsRequest()
            )
        ).data;

        response.Should().BeEquivalentTo(new ResultCodeData(ResultCode.Success, string.Empty));
    }

    private async Task<int> GetSummonCount() =>
        (
            await this.ApiContext.PlayerShopInfos
                .AsNoTracking()
                .FirstAsync(x => x.DeviceAccountId == DeviceAccountId)
        ).DailySummonCount;

    private async Task<IEnumerable<DbPlayerDragonGift>> GetDragonGifts() =>
        await this.ApiContext.PlayerDragonGifts
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ToListAsync();

    private void ResetLastLoginTime() =>
        this.ApiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdate(
                entity => entity.SetProperty(x => x.LastLoginTime, DateTimeOffset.UnixEpoch)
            );

    private void ClearLoginBonuses()
    {
        this.ApiContext.LoginBonuses.ExecuteDelete();
        this.ApiContext.ChangeTracker.Clear();
    }
}
