using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionRepository
{
    IQueryable<DbPlayerMission> Missions { get; }

    IQueryable<DbPlayerMission> GetMissionsByType(MissionType type);

    Task<DbPlayerMission> GetMissionByIdAsync(int id);
    Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync();

    Task<DbPlayerMission> AddMission(
        int id,
        MissionType type,
        DateTimeOffset startTime = default,
        DateTimeOffset endTime = default,
        int groupId = -1
    );

    Task SetProgress(int id, int progress);
    Task SetState(int id, MissionState state);
}
