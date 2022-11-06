using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexResponse(LoadIndexData data) : BaseResponse<LoadIndexData>;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexData(
    UserData user_data,
    IEnumerable<QuestStory> quest_story_list,
    IEnumerable<object> current_main_story_mission,
    PartyPowerData party_power_data,
    object friend_notice,
    object present_notice,
    object mission_notice,
    GuildNoticeData guild_notice,
    object shop_notice,
    IEnumerable<object> ability_crest_list,
    IEnumerable<Chara> chara_list,
    IEnumerable<Dragon> dragon_list,
    IEnumerable<Party> party_list,
    IEnumerable<Material> material_list,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset server_time,
    List<object> functional_maintenance_list,
    int spec_upgrade_time = 1548730800,
    int quest_bonus_stack_base_time = 1617775200,
    int stamina_multi_user_max = 12,
    int stamina_multi_system_max = 99
);

[MessagePackObject(true)]
public record PartyPowerData(int max_party_power);
