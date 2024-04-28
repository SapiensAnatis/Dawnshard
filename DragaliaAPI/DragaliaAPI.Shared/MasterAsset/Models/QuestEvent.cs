using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestEvent(
    int Id,
    QuestEventType QuestEventType,
    QuestBonusReceiveType QuestBonusReceiveType,
    QuestResetIntervalType QuestBonusType,
    int QuestBonusCount,
    int QuestBonusStackCountMax
);
