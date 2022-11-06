using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

public static class CharaRequests
{
    [MessagePackObject(true)]
    public record CharaBuildupBaseRequest(Charas chara_id);

    [MessagePackObject(true)]
    public record CharaAwakeRequest(Charas chara_id, int next_rarity)
        : CharaBuildupBaseRequest(chara_id);

    [MessagePackObject(true)]
    public record CharaBuildupRequest(Charas chara_id, List<BuildupMaterial> material_list)
        : CharaBuildupBaseRequest(chara_id);

    [MessagePackObject(true)]
    public record BuildupMaterial(Materials id, int quantity);

    [MessagePackObject(true)]
    public record CharaUpdateManaRequest(
        Charas chara_id,
        int? next_limit_break_count,
        SortedSet<int>? mana_circle_piece_id_list,
        [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_use_grow_material
    );

    [MessagePackObject(true)]
    public record CharaResetPlusCountRequest(
        Charas chara_id,
        CharaUpgradeEnhanceTypes plus_count_type
    ) : CharaBuildupBaseRequest(chara_id);
}
