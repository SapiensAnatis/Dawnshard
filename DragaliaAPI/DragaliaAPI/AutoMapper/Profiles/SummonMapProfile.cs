using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class SummonMapProfile : Profile
{
    public SummonMapProfile()
    {
        this.CreateMap<DbPlayerSummonHistory, SummonHistoryList>()
            .ForMember(nameof(SummonHistoryList.summon_point_id), o => o.MapFrom(x => x.SummonId));

        this.CreateMap<DbSummonTicket, SummonTicketList>()
            .ForMember(x => x.key_id, o => o.MapFrom(src => src.TicketKeyId))
            .ForMember(x => x.summon_ticket_id, o => o.MapFrom(src => src.Type))
            .ForMember(x => x.quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.use_limit_time, o => o.MapFrom(src => src.ExpirationTime));

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
