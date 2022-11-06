using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, Dragon>();
        this.CreateMap<DbPlayerCharaData, Chara>();
        this.CreateMap<DbPlayerDragonReliability, DragonReliability>()
            .ForCtorParam(
                nameof(DragonReliability.reliability_level),
                o => o.MapFrom(src => src.Level)
            )
            .ForCtorParam(
                nameof(DragonReliability.reliability_total_exp),
                o => o.MapFrom(src => src.Exp)
            );

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
