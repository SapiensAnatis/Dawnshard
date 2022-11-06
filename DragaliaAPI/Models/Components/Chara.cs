using DragaliaAPI.Database.Entities;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Components;

// TODO: Change to CharaDTO to avoid confusion with Charas enum
[MessagePackObject(true)]
public record Chara(
    Charas chara_id,
    int exp,
    int level,
    int additional_max_level,
    int hp,
    int attack,
    int ex_ability_level,
    int ex_ability_2_level,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_new,
    int skill_1_level,
    int skill_2_level,
    int burst_attack_level,
    int rarity,
    int limit_break_count,
    int hp_plus_count,
    int attack_plus_count,
    int combo_buildup_count,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_unlock_edit_skill,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset gettime,
    SortedSet<int> mana_circle_piece_id_list,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_temporary,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool list_view_flag
);
