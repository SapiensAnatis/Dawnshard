using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dmode;

public class DmodeRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IDmodeRepository
{
    public IQueryable<DbPlayerDmodeInfo> Info =>
        apiContext.PlayerDmodeInfos.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDmodeChara> Charas =>
        apiContext.PlayerDmodeCharas.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDmodeDungeon> Dungeon =>
        apiContext.PlayerDmodeDungeons.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDmodeServitorPassive> ServitorPassives =>
        apiContext.PlayerDmodeServitorPassives.Where(
            x => x.ViewerId == playerIdentityService.ViewerId
        );

    public IQueryable<DbPlayerDmodeExpedition> Expedition =>
        apiContext.PlayerDmodeExpeditions.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public async Task<DbPlayerDmodeInfo> GetInfoAsync()
    {
        return await apiContext.PlayerDmodeInfos.FindAsync(playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("Failed to get player dmode info");
    }

    public async Task<DbPlayerDmodeDungeon> GetDungeonAsync()
    {
        return await apiContext.PlayerDmodeDungeons.FindAsync(playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("Failed to get player dmode dungeon");
    }

    public async Task<DbPlayerDmodeExpedition> GetExpeditionAsync()
    {
        return await apiContext.PlayerDmodeExpeditions.FindAsync(playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("Failed to get player dmode expedition");
    }

    public async Task<int> GetTotalMaxFloorAsync()
    {
        // To ensure that if Charas.Quantity == 0 this returns 0
        return await Charas.MaxAsync(x => (int?)x.MaxFloor) ?? 0;
    }

    public async Task<IEnumerable<DbPlayerDmodeChara>> GetCharasAsync()
    {
        return await Charas.ToListAsync();
    }

    public void InitializeForPlayer()
    {
        apiContext.PlayerDmodeInfos.Add(
            new DbPlayerDmodeInfo { ViewerId = playerIdentityService.ViewerId }
        );

        apiContext.PlayerDmodeDungeons.Add(
            new DbPlayerDmodeDungeon { ViewerId = playerIdentityService.ViewerId }
        );

        apiContext.PlayerDmodeExpeditions.Add(
            new DbPlayerDmodeExpedition { ViewerId = playerIdentityService.ViewerId }
        );
    }

    public DbPlayerDmodeChara AddChara(Charas charaId)
    {
        DbPlayerDmodeChara dmodeChara =
            new() { ViewerId = playerIdentityService.ViewerId, CharaId = charaId, };

        return apiContext.PlayerDmodeCharas.Add(dmodeChara).Entity;
    }

    public DbPlayerDmodeServitorPassive AddServitorPassive(
        DmodeServitorPassiveType passiveId,
        int level = 1
    )
    {
        return apiContext
            .PlayerDmodeServitorPassives.Add(
                new DbPlayerDmodeServitorPassive
                {
                    ViewerId = playerIdentityService.ViewerId,
                    PassiveId = passiveId,
                    Level = level
                }
            )
            .Entity;
    }
}
