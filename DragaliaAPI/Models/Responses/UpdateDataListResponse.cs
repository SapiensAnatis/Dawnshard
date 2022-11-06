using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record UpdateDataListResponse(UpdateDataListData data) : BaseResponse<UpdateDataListData>;

[MessagePackObject(true)]
public record UpdateDataListData(UpdateDataList data);
