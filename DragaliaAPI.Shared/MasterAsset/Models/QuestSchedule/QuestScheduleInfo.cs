using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestSchedule;

public record QuestScheduleInfo(
    int Id,
    int ScheduleGroupId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] QuestGroupIntervalType IntervalType,
    [property: JsonConverter(typeof(MasterAssetDateTimeOffsetConverter))] DateTimeOffset StartDate,
    [property: JsonConverter(typeof(MasterAssetDateTimeOffsetConverter))] DateTimeOffset EndDate,
    int DropBonusCount = 0
);
