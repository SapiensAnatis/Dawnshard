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
        // FortDetail is mapped manually due to requiring an additional DB call

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
