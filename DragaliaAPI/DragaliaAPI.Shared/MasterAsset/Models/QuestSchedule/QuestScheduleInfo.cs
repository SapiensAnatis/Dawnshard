using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestSchedule;

public record QuestScheduleInfo(
    int Id,
    int ScheduleGroupId,
    QuestGroupIntervalType IntervalType,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int DropBonusCount = 0
);
