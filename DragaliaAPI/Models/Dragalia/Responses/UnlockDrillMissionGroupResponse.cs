using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record UnlockDrillMissionGroupResponse(UnlockDrillMissionGroupData data)
    : BaseResponse<UnlockDrillMissionGroupData>;

public record UnlockDrillMissionGroupData(
    List<DrillMission> drill_mission_list,
    object update_data_list
);
