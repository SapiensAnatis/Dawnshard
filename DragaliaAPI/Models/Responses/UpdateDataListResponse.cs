using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record UpdateDataListResponse(UpdateDataListData data) : BaseResponse<UpdateDataListData>;

[MessagePackObject(true)]
public record UpdateDataListData(UpdateDataList data);
