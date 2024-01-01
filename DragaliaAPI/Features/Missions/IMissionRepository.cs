using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionRepository
{
    IQueryable<DbPlayerMission> Missions { get; }
    IQueryable<DbCompletedDailyMission> CompletedDailyMissions { get; }

    IQueryable<DbPlayerMission> GetMissionsByType(MissionType type);

    Task<DbPlayerMission> GetMissionByIdAsync(MissionType type, int id);
    Task<ILookup<MissionType, DbPlayerMission>> GetActiveMissionsPerTypeAsync();

    DbPlayerMission AddMission(
        MissionType type,
        int id,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? groupId = null
    );

    Task AddCompletedDailyMission(DbPlayerMission originalMission);

    void RemoveCompletedDailyMission(DbCompletedDailyMission completedDailyMission);
    Task ClearDailyMissions();
}
