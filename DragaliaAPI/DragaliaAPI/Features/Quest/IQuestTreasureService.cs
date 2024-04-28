using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Quest;

public interface IQuestTreasureService
{
    Task<QuestOpenTreasureResponse> DoOpenTreasure(
        QuestOpenTreasureRequest request,
        CancellationToken cancellationToken
    );
}
