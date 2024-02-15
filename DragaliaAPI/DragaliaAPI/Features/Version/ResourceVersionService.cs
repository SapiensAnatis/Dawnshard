using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Version;

public class ResourceVersionService(IOptionsMonitor<ResourceVersionOptions> options)
    : IResourceVersionService
{
    public string GetResourceVersion(Platform platform) =>
        platform switch
        {
            Platform.Ios => options.CurrentValue.Ios,
            Platform.Android => options.CurrentValue.Android,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(platform),
                    "Invalid platform identifier"
                )
        };
}
