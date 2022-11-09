using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record PartyIndexResponse(PartyIndexData data) : BaseResponse<PartyIndexData>;

[MessagePackObject(true)]
public record PartyIndexData(List<object> build_list, UpdateDataList update_data_list);
