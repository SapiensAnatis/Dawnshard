using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Missions;

public interface IMissionInitialProgressionService
{
    Task GetInitialMissionProgress(DbPlayerMission mission);
}
