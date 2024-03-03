using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class SummonTicketHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.SummonTicket];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        SummonTickets ticketId = (SummonTickets)entity.Id;
        int quantity = entity.Quantity;

        switch (ticketId)
        {
            case SummonTickets.SingleSummon:
            case SummonTickets.TenfoldSummon:
            case SummonTickets.AdventurerSummon:
            case SummonTickets.DragonSummon:
            case SummonTickets.AdventurerSummonPlus:
            case SummonTickets.DragonSummonPlus:
                await this.AddStackableTicket(ticketId, quantity);
                break;
            case SummonTickets.TenfoldSummonLimited:
                // TODO: Support expiring tickets.
                // Not possible with Entity record, needs a wider rethink of reward handling.
                throw new NotImplementedException(
                    "Expiring summon tickets are not yet implemented"
                );
            case SummonTickets.None:
            default:
                throw new ArgumentException($"Invalid ticket type: {ticketId}");
        }

        return new GrantReturn(RewardGrantResult.Added);
    }

    private async Task AddStackableTicket(SummonTickets ticketId, int quantity)
    {
        DbSummonTicket ticket =
            await apiContext
                .PlayerSummonTickets.Where(x => x.SummonTicketId == ticketId)
                .FirstOrDefaultAsync() ?? this.InitializeEmptyStackableTicket(ticketId);

        ticket.Quantity += quantity;
    }

    private DbSummonTicket InitializeEmptyStackableTicket(SummonTickets ticketId) =>
        apiContext
            .PlayerSummonTickets.Add(
                new DbSummonTicket()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    SummonTicketId = ticketId,
                }
            )
            .Entity;
}
