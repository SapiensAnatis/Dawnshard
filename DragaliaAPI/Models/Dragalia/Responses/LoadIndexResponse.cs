using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexResponse(LoadIndexData data) : BaseResponse<LoadIndexData>;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexData(
    UserData user_data,
    IEnumerable<Chara> chara_list,
    IEnumerable<Dragon> dragon_list,
    IEnumerable<Party> party_list,
    IEnumerable<QuestStory> quest_story_list,
    IEnumerable<object> current_main_story_mission,
    IEnumerable<object> ability_crest_list
);
