using DragaliaAPI.Features.Wall;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions.InitialProgress;

[UsedImplicitly]
public class WallCalculator(IWallRepository wallRepository) : IInitialProgressCalculator
{
    public async Task<int> GetInitialProgress(MissionProgressionInfo progressionInfo)
    {
        int level = progressionInfo.Parameter!.Value;
        QuestWallTypes? type = (QuestWallTypes?)progressionInfo.Parameter2;

        if (type == null)
        {
            // The mission is either "Clear the Mercurial Gauntlet" (with complete value 1) or
            // "Clear Lv. <Level> of The Mercurial Gauntlet in All Elements" (with complete value 5). We will return the
            // number of elements in which the required level has been reached.
            return await wallRepository.QuestWalls.Where(x => x.WallLevel >= level).CountAsync();
        }

        // The mission is "Clear The Mercurial Gauntlet (<Element>): Lv. <Level>".
        return await wallRepository
            .QuestWalls.Where(x => x.WallLevel >= level && x.WallId == (int)type)
            .AnyAsync()
            ? 1
            : 0;
    }
}
