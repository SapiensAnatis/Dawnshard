using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Features.Blazor;

public class WebOptions
{
    public Uri? BaseImagePath { get; init; }

    [return: NotNullIfNotNull(nameof(path))]
    public string? GetImageSrc(string? path)
    {
        if (path is null || this.BaseImagePath is null)
        {
            return path;
        }

        return new Uri(this.BaseImagePath, path).AbsoluteUri;
    }
}
