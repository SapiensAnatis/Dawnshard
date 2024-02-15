namespace DragaliaAPI.Features.Blazor;

public class BlazorOptions
{
    public Uri? BaseImagePath { get; set; }

    public string GetImageSrc(string path)
    {
        if (this.BaseImagePath is null)
            return path;

        return new Uri(this.BaseImagePath, path).AbsoluteUri;
    }
}
