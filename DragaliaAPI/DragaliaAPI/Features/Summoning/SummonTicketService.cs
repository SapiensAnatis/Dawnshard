using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Summoning;

public class SummonTicketService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) { }
