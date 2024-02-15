using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Factories;

public static class DbPlayerBannerDataFactory
{
    public static DbPlayerBannerData Create(long viewerId, int bannerId)
    {
        return new()
        {
            ViewerId = viewerId,
            //TODO Probably get all this and more from bannerInfo
            SummonBannerId = bannerId,
            ConsecutionSummonPointsMinDate = DateTimeOffset.UtcNow,
            ConsecutionSummonPointsMaxDate = DateTimeOffset.UtcNow.AddDays(7),
        };
    }
}
