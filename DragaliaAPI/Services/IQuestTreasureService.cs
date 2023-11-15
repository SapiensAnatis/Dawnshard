using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IQuestTreasureService
{
    Task<QuestOpenTreasureData> DoOpenTreasure(QuestOpenTreasureRequest request);
}
