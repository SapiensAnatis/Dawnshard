using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

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
