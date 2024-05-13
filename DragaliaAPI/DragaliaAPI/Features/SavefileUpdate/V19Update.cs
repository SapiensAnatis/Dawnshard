using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Savefile update to consolidate summon ticket quantities for tenfold and single tickets.
/// </summary>
/// <param name="context">Instance of <see cref="ApiContext"/>.</param>
/// <param name="playerIdentityService">Instance of <see cref="IPlayerIdentityService"/>.</param>
[UsedImplicitly]
public class V19Update(ApiContext context, IPlayerIdentityService playerIdentityService)
    : ISavefileUpdate
{
    public int SavefileVersion => 19;

    public async Task Apply()
    {
        List<DbSummonTicket> allTickets = await context.PlayerSummonTickets.ToListAsync();

        foreach (SummonTickets ticketType in Enum.GetValues<SummonTickets>())
        {
            List<DbSummonTicket> ticketsOfType = allTickets
                .Where(x => x.SummonTicketId == ticketType)
                .ToList();

            if (ticketsOfType.Count < 2)
                continue;

            context.Add(
                new DbSummonTicket()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    SummonTicketId = ticketType,
                    Quantity = ticketsOfType.Sum(x => x.Quantity)
                }
            );
            context.RemoveRange(ticketsOfType);
        }
    }
}
