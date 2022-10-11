using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexResponse(LoadIndexData data) : BaseResponse<LoadIndexData>;

[MessagePackObject(keyAsPropertyName: true)]
public record LoadIndexData(UserData user_data);
