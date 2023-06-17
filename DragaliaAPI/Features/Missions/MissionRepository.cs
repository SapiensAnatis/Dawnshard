using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
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
        this.apiContext.PlayerMissions.Where(x => x.DeviceAccountId == this.playerIdentityService.AccountId);

    public IQueryable<DbPlayerMission> GetMissionsByType(MissionType type)
    {
        return this.Missions.Where(x => x.Type == type);
    }

    public async Task<DbPlayerMission> GetMissionByIdAsync(int id)
    {
        return await this.apiContext.PlayerMissions.FindAsync(this.playerIdentityService.AccountId, id) ??
               throw new DragaliaException(ResultCode.MissionIdNotFound, "Mission not found");
    }

    public async Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync()
    {
        return (await Missions.ToListAsync()).ToLookup(x => x.Type);
    }

    public async Task<DbPlayerMission> AddMission(int id, MissionType type, DateTimeOffset startTime = default,
        DateTimeOffset endTime = default, int groupId = -1)
    {
        if (await this.apiContext.PlayerMissions.FindAsync(this.playerIdentityService.AccountId, id) != null)
            throw new DragaliaException(ResultCode.CommonDbError, "Mission already exists");

        return this.apiContext.PlayerMissions.Add(new DbPlayerMission
        {
            DeviceAccountId = this.playerIdentityService.AccountId,
            Id = id,
            Type = type,
            Start = startTime,
            End = endTime,
            State = MissionState.InProgress,
            GroupId = groupId
        }).Entity;
    }

    public async Task SetProgress(int id, int progress)
    {
        DbPlayerMission mission = await GetMissionByIdAsync(id);
        mission.Progress = progress;
    }

    public async Task SetState(int id, MissionState state)
    {
        DbPlayerMission mission = await GetMissionByIdAsync(id);
        mission.State = state;
    }
}