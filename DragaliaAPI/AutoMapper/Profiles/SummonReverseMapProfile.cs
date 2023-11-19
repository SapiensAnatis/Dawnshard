using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class SummonReverseMapProfile : Profile
{
    public SummonReverseMapProfile()
    {
        this.CreateMap<SummonTicketList, DbSummonTicket>()
            .ForMember(x => x.Type, o => o.MapFrom(src => src.summon_ticket_id))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.quantity))
            .ForMember(x => x.ExpirationTime, o => o.MapFrom(src => src.use_limit_time))
            .ForMember(x => x.TicketKeyId, o => o.Ignore())
            .ForMember(x => x.ViewerId, o => o.Ignore())
            .ForMember(x => x.Owner, o => o.Ignore());

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
