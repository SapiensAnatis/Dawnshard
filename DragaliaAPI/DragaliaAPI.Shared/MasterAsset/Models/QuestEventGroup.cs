using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestEventGroup(
    int Id,
    int BaseQuestGroupId,
    DateTimeOffset ViewStartDate,
    DateTimeOffset ViewEndDate,
    QuestGroupIntervalType QuestGroupIntervalType
);
