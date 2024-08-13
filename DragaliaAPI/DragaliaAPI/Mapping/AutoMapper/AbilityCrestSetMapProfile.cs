using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class AbilityCrestSetMapProfile : Profile
{
    public AbilityCrestSetMapProfile()
    {
        this.CreateMap<DbAbilityCrestSet, AbilityCrestSetList>();
    }
}
