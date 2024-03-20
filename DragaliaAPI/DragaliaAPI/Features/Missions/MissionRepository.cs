using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Helpers;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    IResetHelper resetHelper
) : IMissionRepository
{
    private readonly ApiContext apiContext = apiContext;
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;

    public IQueryable<DbPlayerMission> Missions =>
        this.apiContext.PlayerMissions.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbCompletedDailyMission> CompletedDailyMissions =>
        this.apiContext.CompletedDailyMissions.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbPlayerMission> GetMissionsByType(MissionType type)
    {
        return this.Missions.Where(x => x.Type == type);
    }

    public async Task<DbPlayerMission?> FindMissionByIdAsync(MissionType type, int id)
    {
        return await this.apiContext.PlayerMissions.FindAsync(
            this.playerIdentityService.ViewerId,
            id,
            type
        );
    }

    public async Task<DbPlayerMission> GetMissionByIdAsync(MissionType type, int id)
    {
        return await this.FindMissionByIdAsync(type, id)
            ?? throw new DragaliaException(ResultCode.MissionIdNotFound, "Mission not found");
    }

    public async Task<ILookup<MissionType, DbPlayerMission>> GetActiveMissionsPerTypeAsync()
    {
        return (
            await Missions
                .Where(x =>
                    (x.Start == DateTimeOffset.UnixEpoch || x.Start < resetHelper.UtcNow)
                    && (x.End == DateTimeOffset.UnixEpoch || x.End > resetHelper.UtcNow)
                )
                .ToListAsync()
        )
            .Where(HasProgressionInfo)
            .ToLookup(x => x.Type);
    }

    public DbPlayerMission AddMission(
        MissionType type,
        int id,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? groupId = null
    )
    {
        return this.AddMission(
            type,
            id,
            progress: 0,
            state: MissionState.InProgress,
            startTime: startTime,
            endTime: endTime,
            groupId: groupId
        );
    }

    public DbPlayerMission AddMission(
        MissionType type,
        int id,
        int progress,
        MissionState state,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? groupId = null
    )
    {
        return this
            .apiContext.PlayerMissions.Add(
                new DbPlayerMission
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    Id = id,
                    Type = type,
                    Start = startTime ?? DateTimeOffset.UnixEpoch,
                    End = endTime ?? DateTimeOffset.UnixEpoch,
                    State = state,
                    GroupId = groupId,
                    Progress = progress
                }
            )
            .Entity;
    }

    public async Task AddCompletedDailyMission(DbPlayerMission originalMission)
    {
        long viewerId = this.playerIdentityService.ViewerId;
        int id = originalMission.Id;
        DateOnly date = DateOnly.FromDateTime(resetHelper.LastDailyReset.UtcDateTime);

        if (await this.apiContext.CompletedDailyMissions.FindAsync(viewerId, id, date) != null)
            return;

        this.apiContext.CompletedDailyMissions.Add(
            new DbCompletedDailyMission()
            {
                ViewerId = viewerId,
                Id = id,
                Date = date,
                StartDate = originalMission.Start,
                EndDate = originalMission.End,
                Progress = originalMission.Progress
            }
        );
    }

    public async Task ClearDailyMissions()
    {
        await foreach (
            DbPlayerMission mission in this.GetMissionsByType(MissionType.Daily).AsAsyncEnumerable()
        )
        {
            this.apiContext.Remove(mission);
        }
    }

    public void RemoveCompletedDailyMission(DbCompletedDailyMission completedDailyMission) =>
        this.apiContext.CompletedDailyMissions.Remove(completedDailyMission);

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
