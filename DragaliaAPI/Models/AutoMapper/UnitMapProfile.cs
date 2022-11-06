using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, Dragon>();
        this.CreateMap<DbPlayerCharaData, Chara>();
        this.CreateMap<DbPlayerDragonReliability, DragonReliability>();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
