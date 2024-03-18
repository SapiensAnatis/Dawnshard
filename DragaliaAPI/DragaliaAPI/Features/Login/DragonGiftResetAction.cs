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
            }

            dbGift.Quantity = 1;
        }

        DayOfWeek todayDayOfWeek = timeProvider.GetUtcNow().DayOfWeek;

        foreach (
            (DragonGifts dailyGiftId, int dayNo) in DragonConstants.RotatingGifts.Select(
                (x, index) => (x, index)
            )
        )
        {
            if (!dbGifts.TryGetValue(dailyGiftId, out DbPlayerDragonGift? dbGift))
            {
                dbGift = new DbPlayerDragonGift()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonGiftId = dailyGiftId,
                };

                apiContext.PlayerDragonGifts.Add(dbGift);
            }

            if (dayNo == (int)todayDayOfWeek)
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
