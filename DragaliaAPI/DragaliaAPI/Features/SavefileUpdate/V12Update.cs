using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Missions.InitialProgress;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V12Update(
    IMissionRepository missionRepository,
    IMissionInitialProgressionService missionInitialProgressionService
) : ISavefileUpdate
{
    public int SavefileVersion => 12;

    private static readonly ImmutableHashSet<int> DrillGroup1MissionIds = MasterAsset
        .MissionDrillData.Enumerable.Where(x => x.MissionDrillGroupId == 1)
        .Select(x => x.Id)
        .ToImmutableHashSet();

    public async Task Apply()
    {
        List<DbPlayerMission> missions = await missionRepository
            .GetMissionsByType(MissionType.Drill)
            .Where(x => DrillGroup1MissionIds.Contains(x.Id) && x.State == MissionState.InProgress)
            .ToListAsync();

        if (missions.Count > 0)
        {
            foreach (DbPlayerMission mission in missions)
            {
                await missionInitialProgressionService.GetInitialMissionProgress(mission);
            }
        }
    }
}
