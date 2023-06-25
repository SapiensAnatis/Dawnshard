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
    public LoginTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LoginIndex_ReturnsSuccess()
    {
        await this.Client
            .Invoking(x => x.PostMsgpack<LoginIndexData>("/login/index", new()))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public void IDailyResetAction_HasExpectedCount()
    {
        // Update this test when adding a new reset action
        this.Services.GetServices<IDailyResetAction>().Should().HaveCount(3);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsItemSummonCount()
    {
        await this.ApiContext.PlayerShopInfos
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.DailySummonCount, 5));

        await this.ApiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(x => x.LastLoginTime, DateTimeOffset.UnixEpoch)
            );

        await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest());

        (
            await this.ApiContext.PlayerShopInfos
                .AsNoTracking()
                .FirstAsync(x => x.DeviceAccountId == DeviceAccountId)
        ).DailySummonCount
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task LoginIndex_LastLoginBeforeReset_ResetsDragonGiftCount()
    {
        await this.ApiContext.PlayerDragonGifts
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(entity => entity.SetProperty(x => x.Quantity, 0));

        await this.ApiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(x => x.LastLoginTime, DateTimeOffset.UnixEpoch)
            );

        await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest());

        (
            await this.ApiContext.PlayerDragonGifts
                .AsNoTracking()
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .ToListAsync()
        )
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
}
