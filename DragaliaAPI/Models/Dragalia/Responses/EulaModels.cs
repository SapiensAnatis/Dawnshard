using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record EulaVersion(string region, string lang, int eula_version, int privacy_policy_version);

    public static class EulaData
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

    [MessagePackObject(keyAsPropertyName: true)]
    public record EulaGetVersionListResponse : BaseResponse<EulaGetVersionListData>
    {
        public override EulaGetVersionListData data { get; init; } = new EulaGetVersionListData();
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public record EulaGetVersionListData
    {
        public List<EulaVersion> version_hash_list { get; init; } = EulaData.AllEulaVersions;
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public record EulaGetVersionRequest
    {
        public string region { get; init; }
        public string lang { get; init; }

        public EulaGetVersionRequest(string region, string lang)
        {
            this.region = region;
            this.lang = lang;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public record EulaGetVersionResponse : BaseResponse<EulaGetVersionData>
    {
        public override EulaGetVersionData data { get; init; }

        [SerializationConstructor]
        public EulaGetVersionResponse(EulaGetVersionData data)
        {
            this.data = data;
        }

        public EulaGetVersionResponse(EulaVersion version)
        {
            data = new(version);
        }
    }

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
}
