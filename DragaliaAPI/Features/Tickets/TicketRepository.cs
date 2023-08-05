using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Tickets;

public class TicketRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : ITicketRepository
{
    public IQueryable<DbSummonTicket> Tickets =>
        apiContext.PlayerSummonTickets.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public DbSummonTicket AddTicket(
        SummonTickets ticketId,
        int quantity = 1,
        DateTimeOffset expirationTime = default
    )
    {
        return apiContext.PlayerSummonTickets
            .Add(
                new DbSummonTicket
                {
                    DeviceAccountId = playerIdentityService.AccountId,
                    Type = ticketId,
                    Quantity = quantity,
                    ExpirationTime =
                        expirationTime == default ? DateTimeOffset.UnixEpoch : expirationTime,
                }
            )
            .Entity;
    }

    public async Task<IEnumerable<DbSummonTicket>> GetTicketsAsync()
    {
        return await Tickets.ToListAsync();
    }
}
