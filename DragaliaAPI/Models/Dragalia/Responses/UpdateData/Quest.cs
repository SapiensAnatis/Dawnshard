using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record Quest(
    int quest_id,
    int state,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_mission_clear_1,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_mission_clear_2,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_mission_clear_3,
    int play_count,
    int daily_play_count,
    int weekly_play_count,
    int last_daily_reset_time,
    int last_weekly_reset_time,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_appear,
    float best_clear_time
);
