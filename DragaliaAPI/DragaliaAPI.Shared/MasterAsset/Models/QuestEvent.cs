using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record QuestEvent(
    int Id,
    QuestEventType QuestEventType,
    QuestBonusReceiveType QuestBonusReceiveType,
    QuestResetIntervalType QuestBonusType,
    int QuestBonusCount,
    int QuestBonusStackCountMax
);
