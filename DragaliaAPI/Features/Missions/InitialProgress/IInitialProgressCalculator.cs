using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions.InitialProgress;

public interface IInitialProgressCalculator
{
    public bool Validate(MissionProgressionInfo progressionInfo);

    public Task<int> GetInitialProgress(MissionProgressionInfo progressionInfo);
}
