using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, DragonList>();
        this.CreateMap<DbPlayerCharaData, CharaList>();
        this.CreateMap<DbPlayerDragonReliability, DragonReliabilityList>()
            .ForCtorParam(
                nameof(DragonReliabilityList.reliability_level),
                o => o.MapFrom(src => src.Level)
            )
            .ForCtorParam(
                nameof(DragonReliabilityList.reliability_total_exp),
                o => o.MapFrom(src => src.Exp)
            );

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
