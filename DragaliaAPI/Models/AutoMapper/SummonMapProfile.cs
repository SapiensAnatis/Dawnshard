using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.AutoMapper;

public class SummonMapProfile : Profile
{
    public SummonMapProfile()
    {
        this.CreateMap<DbPlayerSummonHistory, SummonHistory>()
            .ForCtorParam(nameof(SummonHistory.summon_point_id), o => o.MapFrom(x => x.SummonId));

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
