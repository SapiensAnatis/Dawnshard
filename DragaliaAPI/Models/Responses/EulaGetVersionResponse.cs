using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionResponse(EulaGetVersionData data) : BaseResponse<EulaGetVersionData>;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionData(
    EulaVersion version_hash,
    int agrement_status = 0,
    bool is_required_agree = false
);
