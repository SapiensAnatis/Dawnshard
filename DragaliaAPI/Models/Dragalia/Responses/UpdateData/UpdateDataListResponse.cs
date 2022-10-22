using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record UpdateDataListResponse(UpdateDataListData data) : BaseResponse<UpdateDataListData>;

[MessagePackObject(true)]
public record UpdateDataListData(UpdateDataList data);
