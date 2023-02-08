using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class FortMapProfile : Profile
{
    public FortMapProfile()
    {
        this.CreateMap<DbFortBuild, FortPlantList>();
        this.CreateMap<DbFortBuild, BuildList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
