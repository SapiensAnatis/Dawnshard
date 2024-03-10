using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class SummonTicketMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial IQueryable<SummonTicketList> ProjectToSummonTicketList(
        this IQueryable<DbSummonTicket> ticket
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial SummonTicketList ToSummonTicketList(this DbSummonTicket summonTicket);
}
