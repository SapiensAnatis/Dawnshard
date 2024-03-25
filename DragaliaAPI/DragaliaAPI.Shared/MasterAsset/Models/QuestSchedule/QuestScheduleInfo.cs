using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestSchedule;

public record QuestScheduleInfo(
    int Id,
    int ScheduleGroupId,
    QuestGroupIntervalType IntervalType,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int DropBonusCount = 0
);
