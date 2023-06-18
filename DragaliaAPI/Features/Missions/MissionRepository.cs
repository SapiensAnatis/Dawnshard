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
                type,
                id
            ) ?? throw new DragaliaException(ResultCode.MissionIdNotFound, "Mission not found");
    }

    public async Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync()
    {
        return (await Missions.ToListAsync()).ToLookup(x => x.Type);
    }

    public async Task<DbPlayerMission> AddMissionAsync(
        MissionType type,
        int id,
        DateTimeOffset startTime = default,
        DateTimeOffset endTime = default,
        int groupId = -1
    )
    {
        if (
            await this.apiContext.PlayerMissions.FindAsync(
                this.playerIdentityService.AccountId,
                type,
                id
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
                    Start = startTime == default ? DateTimeOffset.UnixEpoch : startTime,
                    End = endTime == default ? DateTimeOffset.MinValue : endTime,
                    State = MissionState.InProgress,
                    GroupId = groupId
                }
            )
            .Entity;
    }
}
