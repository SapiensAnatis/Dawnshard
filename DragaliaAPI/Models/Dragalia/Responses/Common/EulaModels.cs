using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.Common;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaVersion(string region, string lang, int eula_version, int privacy_policy_version);

public static class EulaStatic
{
    public static List<EulaVersion> AllEulaVersions { get; } =
        new()
        {
            // TODO: Add the complete list of versions
            new("gb", "en_us", 1, 1),
            new("gb", "en_eu", 1, 1),
            new("us", "en_us", 1, 6),
            new("us", "en_eu", 1, 6)
        };
}
