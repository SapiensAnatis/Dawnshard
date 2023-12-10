using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DragaliaAPI.Features.Missions;

public class MissionRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    TimeProvider timeProvider
) : IMissionRepository
{
    private readonly ApiContext apiContext = apiContext;
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;

    public IQueryable<DbPlayerMission> Missions =>
        this.apiContext
            .PlayerMissions
            .Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbCompletedDailyMission> CompletedDailyMissions =>
        this.apiContext
            .CompletedDailyMissions
            .Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbPlayerMission> GetMissionsByType(MissionType type)
    {
        return this.Missions.Where(x => x.Type == type);
    }

    public async Task<DbPlayerMission> GetMissionByIdAsync(MissionType type, int id)
    {
        return await this.apiContext
                .PlayerMissions
                .FindAsync(this.playerIdentityService.ViewerId, id, type)
            ?? throw new DragaliaException(ResultCode.MissionIdNotFound, "Mission not found");
    }

    public async Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync()
    {
        return (await Missions.ToListAsync()).Where(HasProgressionInfo).ToLookup(x => x.Type);
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
            await this.apiContext
                .PlayerMissions
                .FindAsync(this.playerIdentityService.ViewerId, id, type) != null
        )
            throw new DragaliaException(ResultCode.CommonDbError, "Mission already exists");

        return this.apiContext
            .PlayerMissions
            .Add(
                new DbPlayerMission
                {
                    ViewerId = this.playerIdentityService.ViewerId,
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

    public void AddCompletedDailyMissionAsync(int id) =>
        this.apiContext
            .CompletedDailyMissions
            .Add(
                new DbCompletedDailyMission()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    Id = id,
                    Date = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date)
                }
            );

    private static bool HasProgressionInfo(DbPlayerMission mission)
    {
        // Fully complete types
        if (mission.Type is MissionType.Drill or MissionType.MainStory)
            return true;

        int missionProgressionId = MasterAssetUtils.GetMissionProgressionId(
            mission.Id,
            mission.Type
        );

        return MasterAsset.MissionProgressionInfo.TryGetValue(missionProgressionId, out _);
    }
}
