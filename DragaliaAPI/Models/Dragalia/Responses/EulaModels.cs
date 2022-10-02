using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaVersion(string region, string lang, int eula_version, int privacy_policy_version);

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListResponse(EulaGetVersionListData data) : BaseResponse<EulaGetVersionListData>;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionListData(List<EulaVersion> version_hash_list);

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionRequest(string? region, string? lang);

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionResponse(EulaGetVersionData data) : BaseResponse<EulaGetVersionData>;

public record EulaGetVersionData
{
    public EulaVersion version_hash { get; init; }
    public int agreement_status { get; init; } = 0;
    public bool is_required_agree { get; init; } = false;

    public EulaGetVersionData(EulaVersion version_hash)
    {
        this.version_hash = version_hash;
    }
}

public static class EulaStatic
{
    public static List<EulaVersion> AllEulaVersions { get; } = new()
    {
        // TODO: Add the complete list of versions
        new("gb", "en_us", 1, 1),
        new("gb", "en_eu", 1, 1),
        new("us", "en_us", 1, 6),
        new("us", "en_eu", 1, 6)
    };
}