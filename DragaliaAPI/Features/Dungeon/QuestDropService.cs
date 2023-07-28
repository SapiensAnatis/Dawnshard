using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Dungeon;

public class QuestDropService(ILogger<QuestEnemyService> logger) : IQuestDropService
{
    public IEnumerable<Materials> GetDrops(int questId)
    {
        if (!MasterAsset.QuestDrops.TryGetValue(questId, out QuestDropInfo? questDropInfo))
        {
            logger.LogWarning("Unable to retrieve drop list for quest id {quest}", questId);
            return Enumerable.Empty<Materials>();
        }

        return questDropInfo.Material;
    }
}
