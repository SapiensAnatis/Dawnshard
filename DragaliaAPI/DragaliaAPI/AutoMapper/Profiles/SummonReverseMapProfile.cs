using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class SummonReverseMapProfile : Profile
{
    public SummonReverseMapProfile()
    {
        this.CreateMap<SummonTicketList, DbSummonTicket>()
            .ForMember(x => x.SummonTicketId, o => o.MapFrom(src => src.SummonTicketId))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.UseLimitTime, o => o.MapFrom(src => src.UseLimitTime))
            .ForMember(x => x.KeyId, o => o.Ignore())
            .ForMember(x => x.ViewerId, o => o.Ignore())
            .ForMember(x => x.Owner, o => o.Ignore());
    }
}
