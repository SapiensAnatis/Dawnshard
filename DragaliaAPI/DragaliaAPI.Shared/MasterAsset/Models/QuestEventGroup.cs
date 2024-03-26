using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record QuestEventGroup(
    int Id,
    int BaseQuestGroupId,
    DateTimeOffset ViewStartDate,
    DateTimeOffset ViewEndDate,
    QuestGroupIntervalType QuestGroupIntervalType
);
