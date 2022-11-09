using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

public record CharaBuildupResponse(CharaBuildupData data) : BaseResponse<CharaBuildupData>;

[MessagePackObject(true)]
public record CharaBuildupData(
    CharBuildupUpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AlbumPassiveNotice(
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_update_chara,
    [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_update_dragon
);

[MessagePackObject(true)]
public class CharBuildupUpdateDataList
{
    public AlbumPassiveNotice album_passive_notice { get; set; } =
        new AlbumPassiveNotice(false, false);

    //public PartyPowerData party_power_data { get; set; } = new(0);
    public List<Chara> chara_list { get; set; } = new();
    public List<Material> material_list { get; set; } = new();
    public UserData user_data { get; set; } = null!;

    //public PresentNotice present_notice { get; set; } = null!;
    public MissionNoticeData mission_notice { get; set; } = null!;
    public List<object> current_main_story_mission { get; set; } = new();
    public List<object> functional_maintanance_list { get; set; } = new();
}
