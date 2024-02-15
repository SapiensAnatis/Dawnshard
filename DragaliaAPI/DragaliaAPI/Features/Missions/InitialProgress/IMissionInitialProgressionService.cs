using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions.InitialProgress;

public interface IMissionInitialProgressionService
{
    Task GetInitialMissionProgress(DbPlayerMission mission);

    Task<DbPlayerMission> StartMission(
        MissionType type,
        int id,
        int groupId = 0,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null
    );
}
