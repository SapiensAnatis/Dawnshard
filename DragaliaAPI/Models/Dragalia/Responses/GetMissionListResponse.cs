using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record GetMissionListResponse(GetMissionListData data) : BaseResponse<GetMissionListData>;

[MessagePackObject(true)]
public record GetMissionListData(
    IReadOnlyList<NormalMission> normal_mission_list,
    IReadOnlyList<DailyMission> daily_mission_list,
    IReadOnlyList<PeriodMission> period_mission_list,
    IReadOnlyList<SpecialMission> special_mission_list,
    IReadOnlyList<AlbumMissionList> album_mission_list,
    MissionNoticeData mission_notice,
    IReadOnlyList<object> current_main_story_mission,
    IReadOnlyList<object> special_mission_purchased_group_id_list
);

[MessagePackObject(true)]
public record AlbumMissionList(
    int album_mission_id,
    int progress,
    int state,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record DailyMission(
    int daily_mission_id,
    int progress,
    int state,
    int day_no,
    int is_lock_receive_reward,
    int is_pickup,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record NormalMission(
    int normal_mission_id,
    int progress,
    int state,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record PeriodMission(
    int period_mission_id,
    int progress,
    int state,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record SpecialMission(
    int special_mission_id,
    int progress,
    int state,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record DrillMission(
    int drill_mission_id,
    int progress,
    int state,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record MissionNotice(
    int is_update,
    int receivable_reward_count,
    IReadOnlyList<object> new_complete_mission_id_list,
    int pickup_mission_count,
    int all_mission_count,
    int completed_mission_count,
    int current_mission_id
);

[MessagePackObject(true)]
public record MissionNoticeData(
    MissionNotice normal_mission_notice,
    MissionNotice daily_mission_notice,
    MissionNotice period_mission_notice,
    MissionNotice beginner_mission_notice,
    MissionNotice special_mission_notice,
    MissionNotice main_story_mission_notice,
    MissionNotice memory_event_mission_notice,
    MissionNotice drill_mission_notice,
    MissionNotice album_mission_notice
);

public static class GetMissionListFactory
{
    private static readonly List<NormalMission> normalMissions =
        new() { new(10000101, 0, 0, 0, 0) };
    private static readonly List<DailyMission> dailyMissions =
        new() { new(15070101, 0, 0, 0, 0, 0, 0, int.MaxValue) };
    private static readonly List<PeriodMission> periodMissions =
        new() { new(12050101, 0, 0, 0, int.MaxValue) };
    private static readonly List<SpecialMission> specialMissions =
        new() { new(10000101, 0, 0, 0, 0) };
    private static readonly List<AlbumMissionList> albumMissions =
        new() { new(10010101, 0, 0, 0, 0) };
    private static readonly MissionNotice emptyMissionNotice =
        new(0, 0, new List<object>(), 0, 0, 0, 0);
    public static readonly MissionNoticeData emptyMissionNoticeData =
        new(
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice,
            emptyMissionNotice
        );

    public static GetMissionListData CreateData() // TODO: replace stub data with actual implementation
    {
        return new GetMissionListData(
            normalMissions,
            dailyMissions,
            periodMissions,
            specialMissions,
            albumMissions,
            emptyMissionNoticeData,
            new List<object>(),
            new List<object>()
        );
    }
}
