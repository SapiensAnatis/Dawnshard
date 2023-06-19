using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionRepository
{
    IQueryable<DbPlayerMission> Missions { get; }

    IQueryable<DbPlayerMission> GetMissionsByType(MissionType type);

    Task<DbPlayerMission> GetMissionByIdAsync(MissionType type, int id);
    Task<ILookup<MissionType, DbPlayerMission>> GetAllMissionsPerTypeAsync();

    Task<DbPlayerMission> AddMissionAsync(
        MissionType type,
        int id,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? groupId = null
    );
}
