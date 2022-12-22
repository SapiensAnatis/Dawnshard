using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Factories;

public static class DbPlayerBannerDataFactory
{
    public static DbPlayerBannerData Create(string deviceAccountId, int bannerId)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            //TODO Probably get all this and more from bannerInfo
            SummonBannerId = bannerId,
            ConsecutionSummonPointsMinDate = DateTimeOffset.UtcNow,
            ConsecutionSummonPointsMaxDate = DateTimeOffset.UtcNow.AddDays(7),
        };
    }
}
