using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;
using static DragaliaAPI.Controllers.Dragalia.CharaController;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexResponse(LoadIndexData data) : BaseResponse<LoadIndexData>;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexData(
    UserData user_data,
    IEnumerable<Chara> chara_list,
    IEnumerable<Dragon> dragon_list,
    IEnumerable<Party> party_list,
    IEnumerable<Material> material_list,
    IEnumerable<object> ability_crest_list
);
