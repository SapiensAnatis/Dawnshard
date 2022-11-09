using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListResponse(EulaGetVersionListData data)
    : BaseResponse<EulaGetVersionListData>;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListData(List<EulaVersion> version_hash_list);
