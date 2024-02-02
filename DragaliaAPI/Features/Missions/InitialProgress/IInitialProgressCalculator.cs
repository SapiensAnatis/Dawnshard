using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions.InitialProgress;

public interface IInitialProgressCalculator
{
    public Task<int> GetInitialProgress(MissionProgressionInfo progressionInfo);
}
