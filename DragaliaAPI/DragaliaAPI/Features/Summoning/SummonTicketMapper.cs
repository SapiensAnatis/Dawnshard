using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Summoning;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SummonTicketMapper
{
    public static partial IQueryable<SummonTicketList> ProjectToSummonTicketList(
        this IQueryable<DbSummonTicket> ticket
    );
}
