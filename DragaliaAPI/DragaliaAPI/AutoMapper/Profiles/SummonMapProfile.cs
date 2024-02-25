using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class SummonMapProfile : Profile
{
    public SummonMapProfile()
    {
        this.CreateMap<DbPlayerSummonHistory, SummonHistoryList>()
            .ForMember(nameof(SummonHistoryList.SummonPointId), o => o.MapFrom(x => x.SummonId));

        this.CreateMap<DbSummonTicket, SummonTicketList>()
            .ForMember(x => x.KeyId, o => o.MapFrom(src => src.KeyId))
            .ForMember(x => x.SummonTicketId, o => o.MapFrom(src => src.SummonTicketId))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.UseLimitTime, o => o.MapFrom(src => src.UseLimitTime));
    }
}
