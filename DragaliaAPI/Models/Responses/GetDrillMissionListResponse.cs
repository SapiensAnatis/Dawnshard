using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record GetDrillMissionListResponse(GetDrillMissionListData data)
    : BaseResponse<GetDrillMissionListData>;

[MessagePackObject(true)]
public record GetDrillMissionListData(
    List<DrillMission> drill_mission_list,
    MissionNoticeData mission_notice
);
