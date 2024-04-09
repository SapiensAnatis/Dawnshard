using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class SummonMapper
{
    public static UserSummonList MapToUserSummonList(this DbPlayerBannerData bannerData) =>
        new() { SummonId = bannerData.SummonBannerId, SummonCount = bannerData.SummonCount, };

    public static SummonPointList MapToSummonPointList(this DbPlayerBannerData bannerData) =>
        new()
        {
            SummonPointId = bannerData.SummonBannerId,
            SummonPoint = bannerData.SummonPoints,
            CsSummonPoint = bannerData.ConsecutionSummonPoints,
            CsPointTermMinDate = bannerData.ConsecutionSummonPointsMinDate,
            CsPointTermMaxDate = bannerData.ConsecutionSummonPointsMaxDate
        };

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial IQueryable<SummonTicketList> ProjectToSummonTicketList(
        this IQueryable<DbSummonTicket> ticket
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial SummonTicketList ToSummonTicketList(this DbSummonTicket summonTicket);
}
