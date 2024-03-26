using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestSchedule;

[MemoryPackable]
public partial record QuestScheduleInfo(
    int Id,
    int ScheduleGroupId,
    QuestGroupIntervalType IntervalType,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int DropBonusCount = 0
);
