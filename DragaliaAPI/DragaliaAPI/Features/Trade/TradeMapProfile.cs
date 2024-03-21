using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Trade;

public class TradeMapProfile : Profile
{
    public TradeMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<UserTreasureTradeList, DbPlayerTrade>()
            .ForMember(x => x.Id, opts => opts.MapFrom(src => src.TreasureTradeId))
            .ForMember(x => x.Count, opts => opts.MapFrom(src => src.TradeCount))
            .ForMember(x => x.Type, opts => opts.MapFrom(src => TradeType.Treasure))
            .ForMember(x => x.LastTradeTime, opts => opts.MapFrom(src => src.LastResetTime));
    }
}
