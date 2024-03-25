using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record QuestEvent(
    int Id,
    QuestEventType QuestEventType,
    QuestBonusReceiveType QuestBonusReceiveType,
    QuestResetIntervalType QuestBonusType,
    int QuestBonusCount,
    int QuestBonusStackCountMax
);
