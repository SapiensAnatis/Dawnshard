using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class EmblemReverseMapProfile : Profile
{
    public EmblemReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<EmblemList, DbEmblem>()
            .ForMember(x => x.GetTime, o => o.MapFrom(src => src.GetTime));
    }
}
