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
        apiContext.PlayerDmodeInfos.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDmodeChara> Charas =>
        apiContext.PlayerDmodeCharas.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDmodeDungeon> Dungeon =>
        apiContext.PlayerDmodeDungeons.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDmodeServitorPassive> ServitorPassives =>
        apiContext.PlayerDmodeServitorPassives.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDmodeExpedition> Expedition =>
        apiContext.PlayerDmodeExpeditions.Where(
            x => x.DeviceAccountId == playerIdentityService.AccountId
        );

    public async Task<DbPlayerDmodeInfo> GetInfoAsync()
    {
        return await apiContext.PlayerDmodeInfos.FindAsync(playerIdentityService.AccountId)
            ?? throw new InvalidOperationException("Failed to get player dmode info");
    }

    public async Task<DbPlayerDmodeDungeon> GetDungeonAsync()
    {
        return await apiContext.PlayerDmodeDungeons.FindAsync(playerIdentityService.AccountId)
            ?? throw new InvalidOperationException("Failed to get player dmode dungeon");
    }

    public async Task<DbPlayerDmodeExpedition> GetExpeditionAsync()
    {
        return await apiContext.PlayerDmodeExpeditions.FindAsync(playerIdentityService.AccountId)
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
            new DbPlayerDmodeInfo { DeviceAccountId = playerIdentityService.AccountId }
        );

        apiContext.PlayerDmodeDungeons.Add(
            new DbPlayerDmodeDungeon { DeviceAccountId = playerIdentityService.AccountId }
        );

        apiContext.PlayerDmodeExpeditions.Add(
            new DbPlayerDmodeExpedition { DeviceAccountId = playerIdentityService.AccountId }
        );
    }

    public DbPlayerDmodeChara AddChara(Charas charaId)
    {
        DbPlayerDmodeChara dmodeChara =
            new() { DeviceAccountId = playerIdentityService.AccountId, CharaId = charaId, };

        return apiContext.PlayerDmodeCharas.Add(dmodeChara).Entity;
    }

    public DbPlayerDmodeServitorPassive AddServitorPassive(
        DmodeServitorPassiveType passiveId,
        int level = 1
    )
    {
        return apiContext.PlayerDmodeServitorPassives
            .Add(
                new DbPlayerDmodeServitorPassive
                {
                    DeviceAccountId = playerIdentityService.AccountId,
                    PassiveId = passiveId,
                    Level = level
                }
            )
            .Entity;
    }
}
