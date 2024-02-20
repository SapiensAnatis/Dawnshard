using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Zena;

public class ZenaService(
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext,
    ILogger<ZenaService> logger
) : IZenaService
{
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;
    private readonly ApiContext apiContext = apiContext;

    public async Task<GetTeamDataResponse?> GetTeamData(IEnumerable<int> partyNumbers)
    {
        string? playerName = await this
            .apiContext.PlayerUserData.Where(x => x.ViewerId == this.playerIdentityService.ViewerId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();

        if (playerName is null)
        {
            logger.LogWarning(
                "Failed to get team data: player with ID {ViewerId} does not exist.",
                this.playerIdentityService.ViewerId
            );

            return null;
        }

        Charas[] charas = await this
            .apiContext.PlayerPartyUnits.Where(x =>
                x.ViewerId == this.playerIdentityService.ViewerId
                && partyNumbers.Contains(x.PartyNo)
            )
            .OrderBy(x => x.PartyNo)
            .ThenBy(x => x.UnitNo)
            .Select(x => x.CharaId)
            .ToArrayAsync();

        if (charas.Length > 4)
        {
            return new()
            {
                Name = playerName,
                Unit1 = charas.ElementAtOrDefault(0),
                Unit2 = charas.ElementAtOrDefault(1),
                Unit3 = charas.ElementAtOrDefault(2),
                Unit4 = charas.ElementAtOrDefault(3),
                Unit5 = charas.ElementAtOrDefault(4),
                Unit6 = charas.ElementAtOrDefault(5),
                Unit7 = charas.ElementAtOrDefault(6),
                Unit8 = charas.ElementAtOrDefault(7),
            };
        }

        return new()
        {
            Name = playerName,
            Unit1 = charas.ElementAtOrDefault(0),
            Unit2 = charas.ElementAtOrDefault(1),
            Unit3 = charas.ElementAtOrDefault(2),
            Unit4 = charas.ElementAtOrDefault(3),
        };
    }
}
