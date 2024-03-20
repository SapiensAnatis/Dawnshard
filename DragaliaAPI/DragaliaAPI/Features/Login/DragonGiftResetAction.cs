using System.Collections.Frozen;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

public class DragonGiftResetAction(
    ApiContext apiContext,
    TimeProvider timeProvider,
    IPlayerIdentityService playerIdentityService
) : IDailyResetAction
{
    private static readonly FrozenSet<DragonGifts> AlwaysPurchasableGifts =
        Enum.GetValues<DragonGifts>().Where(x => x < DragonGifts.JuicyMeat).ToFrozenSet();

    public async Task Apply()
    {
        Dictionary<DragonGifts, DbPlayerDragonGift> dbGifts = await apiContext
            .PlayerDragonGifts.AsTracking()
            .ToDictionaryAsync(x => x.DragonGiftId);

        foreach (DragonGifts giftId in AlwaysPurchasableGifts)
        {
            if (!dbGifts.TryGetValue(giftId, out DbPlayerDragonGift? dbGift))
            {
                dbGift = new DbPlayerDragonGift()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonGiftId = giftId,
                };

                apiContext.PlayerDragonGifts.Add(dbGift);

                dbGifts[giftId] = dbGift;
            }

            dbGift.Quantity = 1;
        }

        DayOfWeek todayDayOfWeek = timeProvider.GetUtcNow().DayOfWeek;

        foreach (DragonGifts dailyGiftId in DragonConstants.RotatingGifts)
        {
            if (!dbGifts.TryGetValue(dailyGiftId, out DbPlayerDragonGift? dbGift))
            {
                dbGift = new DbPlayerDragonGift()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonGiftId = dailyGiftId,
                };

                apiContext.PlayerDragonGifts.Add(dbGift);

                dbGifts[dailyGiftId] = dbGift;
            }

            if (DragonConstants.RotatingGifts[(int)todayDayOfWeek] == dailyGiftId)
            {
                dbGift.Quantity = 1;
            }
            else
            {
                dbGift.Quantity = 0;
            }
        }
    }
}
