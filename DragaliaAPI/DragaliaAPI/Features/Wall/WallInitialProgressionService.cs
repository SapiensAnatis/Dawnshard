using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Wall;

public class WallInitialProgressionService(IWallRepository wallRepository)
{
    private Dictionary<QuestWallTypes, int>? wallLevelCache;

    public ValueTask<int> GetWallLevel(QuestWallTypes type) =>
        this.wallLevelCache is not null
            ? ValueTask.FromResult(this.wallLevelCache[type])
            : new ValueTask<int>(this.GetWallLevelSlow(type));

    public ValueTask<Dictionary<QuestWallTypes, int>> GetAllWallLevels() =>
        this.wallLevelCache is not null
            ? ValueTask.FromResult(new Dictionary<QuestWallTypes, int>(this.wallLevelCache))
            : new ValueTask<Dictionary<QuestWallTypes, int>>(this.GetAllWallLevelsSlow());

    private async Task<int> GetWallLevelSlow(QuestWallTypes type)
    {
        Debug.Assert(Enum.IsDefined(type));

        await this.PopulateWallLevelCache();
        return this.wallLevelCache[type];
    }

    private async Task<Dictionary<QuestWallTypes, int>> GetAllWallLevelsSlow()
    {
        await this.PopulateWallLevelCache();
        return new(this.wallLevelCache);
    }

    [MemberNotNull(nameof(wallLevelCache))]
    private async Task PopulateWallLevelCache() =>
        this.wallLevelCache = await wallRepository.QuestWalls.ToDictionaryAsync(
            x => (QuestWallTypes)x.WallId,
            x => x.WallLevel
        );
}
