using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
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
        List<DbSummonTicket> allTickets = await context
            .PlayerSummonTickets.Where(x =>
                x.SummonTicketId == SummonTickets.SingleSummon
                || x.SummonTicketId == SummonTickets.TenfoldSummon
            )
            .ToListAsync();

        List<DbSummonTicket> singleTickets = allTickets
            .Where(x => x.SummonTicketId == SummonTickets.SingleSummon)
            .ToList();

        if (singleTickets.Count > 1)
            this.StackTickets(singleTickets);

        List<DbSummonTicket> tenfoldTickets = allTickets
            .Where(x => x.SummonTicketId == SummonTickets.TenfoldSummon)
            .ToList();

        if (tenfoldTickets.Count > 1)
            this.StackTickets(tenfoldTickets);
    }

    private void StackTickets(List<DbSummonTicket> tickets)
    {
        DbSummonTicket sampleTicket = tickets[0];

        context.RemoveRange(tickets);
        context.Add(
            new DbSummonTicket()
            {
                ViewerId = playerIdentityService.ViewerId,
                SummonTicketId = sampleTicket.SummonTicketId,
                Quantity = tickets.Sum(x => x.Quantity),
            }
        );
    }
}
