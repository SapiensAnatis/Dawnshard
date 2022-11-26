using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class SummonMapProfile : Profile
{
    public SummonMapProfile()
    {
        this.CreateMap<DbPlayerSummonHistory, SummonHistoryList>()
            .ForCtorParam(
                nameof(SummonHistoryList.summon_point_id),
                o => o.MapFrom(x => x.SummonId)
            );

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
