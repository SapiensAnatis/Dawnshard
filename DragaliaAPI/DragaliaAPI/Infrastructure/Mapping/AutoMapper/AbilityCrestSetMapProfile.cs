using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

public class AbilityCrestSetMapProfile : Profile
{
    public AbilityCrestSetMapProfile()
    {
        this.CreateMap<DbAbilityCrestSet, AbilityCrestSetList>();
    }
}
