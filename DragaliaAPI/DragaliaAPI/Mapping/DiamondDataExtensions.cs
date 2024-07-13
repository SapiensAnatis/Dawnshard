using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping;

public static class DiamondDataExtensions
{
    public static IQueryable<DiamondData> ProjectToDiamondData(
        this IQueryable<DbPlayerDiamondData> query
    ) => query.Select(x => new DiamondData(x.PaidDiamond, x.FreeDiamond));
}
