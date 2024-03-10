using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Summoning;

public class SummonTicketService(ApiContext apiContext)
{
    public async Task<IEnumerable<SummonTicketList>> GetSummonTicketList() =>
        await apiContext.PlayerSummonTickets.ProjectToSummonTicketList().ToListAsync();
}
