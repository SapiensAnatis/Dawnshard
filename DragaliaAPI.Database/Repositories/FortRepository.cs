using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class FortRepository : IFortRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<FortRepository> logger;

    public FortRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<FortRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
        this.logger = logger;
    }

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public IQueryable<DbFortBuild> GetBuilds(string accountId) =>
        this.apiContext.PlayerFortBuilds.Where(x => x.DeviceAccountId == accountId);

    public IQueryable<DbFortBuild> Builds =>
        this.apiContext.PlayerFortBuilds.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public async Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel)
    {
        int level = await this.Builds
            .Where(x => x.PlantId == plant)
            .Select(x => x.Level)
            .FirstOrDefaultAsync();
        bool result = level >= requiredLevel;

        if (!result)
        {
            this.logger.LogDebug(
                "Failed build level check: requested plant {plant} at level {requestLevel}, but had level {actualLevel}",
                plant,
                requiredLevel,
                level
            );
        }

        return result;
    }
}
