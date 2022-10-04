using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListResponse(EulaGetVersionListData data)
    : BaseResponse<EulaGetVersionListData>;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListData(List<EulaVersion> version_hash_list);
