using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record QuestEventGroup(
    int Id,
    int BaseQuestGroupId,
    DateTimeOffset ViewStartDate,
    DateTimeOffset ViewEndDate,
    QuestGroupIntervalType QuestGroupIntervalType
);
