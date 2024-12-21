using DragaliaAPI.Features.Shared.Options;

namespace DragaliaAPI.Features.Dragalipatch;

/// <summary>
/// Response from /dragalipatch/config endpoint, containing additional field
/// not covered by config section.
/// </summary>
public class DragalipatchResponse : DragalipatchOptions
{
    public bool UseUnifiedLogin { get; set; }

    public DragalipatchResponse(DragalipatchOptions baseOptions)
    {
        this.Mode = baseOptions.Mode;
        this.ConeshellKey = baseOptions.ConeshellKey;
        this.CdnUrl = baseOptions.CdnUrl;
        this.UseUnifiedLogin = true;
    }

    public DragalipatchResponse() { }
}
