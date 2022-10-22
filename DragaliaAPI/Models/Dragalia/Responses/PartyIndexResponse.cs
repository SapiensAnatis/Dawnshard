using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record PartyIndexResponse(PartyIndexData data) : BaseResponse<PartyIndexData>;

[MessagePackObject(true)]
public record PartyIndexData(List<object> build_list, UpdateDataList update_data_list);
