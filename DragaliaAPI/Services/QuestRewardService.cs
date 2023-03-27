using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Services;

public class QuestRewardService : IQuestRewardService
{
    private readonly ILogger<QuestRewardService> logger;

    public QuestRewardService(ILogger<QuestRewardService> logger)
    {
        this.logger = logger;
    }

    public IEnumerable<Materials> GetDrops(int questId)
    {
        if (!MasterAsset.QuestDrops.TryGetValue(questId, out QuestDropInfo? questDropInfo))
        {
            this.logger.LogWarning("Unable to retrieve drop list for quest id {quest}", questId);
            return Enumerable.Empty<Materials>();
        }

        return questDropInfo.Material;
    }
}
