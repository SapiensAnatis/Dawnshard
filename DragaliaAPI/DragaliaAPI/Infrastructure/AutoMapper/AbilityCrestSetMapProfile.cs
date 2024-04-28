using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class AbilityCrestSetMapProfile : Profile
{
    public AbilityCrestSetMapProfile()
    {
        this.CreateMap<DbAbilityCrestSet, AbilityCrestSetList>();
    }
}
