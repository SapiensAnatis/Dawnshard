using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record GetSupportCharaResponse(GetSupportCharaData data) : BaseResponse<GetSupportCharaData>;

[MessagePackObject(true)]
public record GetSupportCharaData(
    int result,
    SettingSupport setting_support,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record SettingSupport(
    int last_active_time,
    int chara_id,
    int equip_dragon_key_id,
    int equip_weapon_body_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    int equip_talisman_key_id,
    int user_level_group
);

public static class GetSupportCharaFactory
{
    public static GetSupportCharaData CreateData()
    {
        return new(
            1,
            new SettingSupport(1661984335, 10140101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            new()
        );
    }
}
