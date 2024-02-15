using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Tickets;

public interface ITicketRepository
{
    IQueryable<DbSummonTicket> Tickets { get; }

    DbSummonTicket AddTicket(
        SummonTickets ticketId,
        int quantity = 1,
        DateTimeOffset expirationTime = default
    );

    Task<IEnumerable<DbSummonTicket>> GetTicketsAsync();
}
