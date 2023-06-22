using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class AbilityCrestSetMapProfile : Profile
{
    public AbilityCrestSetMapProfile()
    {
        this.CreateMap<DbAbilityCrestSet, AbilityCrestSetList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
