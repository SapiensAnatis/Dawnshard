using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionRepository : IMissionRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public MissionRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerMission> Missions =>
        this.apiContext.PlayerMissions.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerMission> GetMissionsByType(MissionType type)
    {
        return this.Missions.Where(x => x.Type == type);
    }

    public async Task<DbPlayerMission> GetMissionByIdAsync(MissionType type, int id)
    {
        return await this.apiContext.PlayerMissions.FindAsync(
                this.playerIdentityService.AccountId,
                id,
                type
            ) ?? throw new DragaliaException(ResultCode.MissionIdNotFound, "Mission not found");
    }

    public async Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync()
    {
        return (await Missions.ToListAsync()).ToLookup(x => x.Type);
    }

    public async Task<DbPlayerMission> AddMissionAsync(
        MissionType type,
        int id,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? groupId = null
    )
    {
        if (
            await this.apiContext.PlayerMissions.FindAsync(
                this.playerIdentityService.AccountId,
                id,
                type
            ) != null
        )
            throw new DragaliaException(ResultCode.CommonDbError, "Mission already exists");

        return this.apiContext.PlayerMissions
            .Add(
                new DbPlayerMission
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    Id = id,
                    Type = type,
                    Start = startTime ?? DateTimeOffset.UnixEpoch,
                    End = endTime ?? DateTimeOffset.UnixEpoch,
                    State = MissionState.InProgress,
                    GroupId = groupId
                }
            )
            .Entity;
    }
}
