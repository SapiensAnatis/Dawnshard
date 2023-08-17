using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public record DrillMissionGroup(
    int Id,
    string TextName,
    string TextNameShort,
    int Step,
    string TextTitle,
    string TextDetail,
    EntityTypes UnlockEntityType1,
    int UnlockEntityId1,
    int UnlockEntityQuantity1,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
);
