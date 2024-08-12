using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class FortMapProfile : Profile
{
    public FortMapProfile()
    {
        this.CreateMap<DbFortBuild, FortPlantList>();
        this.CreateMap<DbFortBuild, BuildList>();
        // FortDetail is mapped manually due to requiring an additional DB call
    }
}
