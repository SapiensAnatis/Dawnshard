using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IQuestTreasureService
{
    Task<QuestOpenTreasureResponse> DoOpenTreasure(
        QuestOpenTreasureRequest request,
        CancellationToken cancellationToken
    );
}
