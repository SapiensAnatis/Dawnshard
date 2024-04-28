using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

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
