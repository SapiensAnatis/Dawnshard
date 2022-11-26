using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, DragonList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());
        this.CreateMap<DbPlayerCharaData, CharaList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());
        this.CreateMap<DbPlayerDragonReliability, DragonReliabilityList>()
            .ForMember(
                nameof(DragonReliabilityList.reliability_level),
                o => o.MapFrom(src => src.Level)
            )
            .ForMember(
                nameof(DragonReliabilityList.reliability_total_exp),
                o => o.MapFrom(src => src.Exp)
            );

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
