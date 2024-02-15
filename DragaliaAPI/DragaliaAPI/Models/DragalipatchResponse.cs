using DragaliaAPI.Models.Options;

namespace DragaliaAPI.Models;

/// <summary>
/// Response from /dragalipatch/config endpoint, containing additional field
/// not covered by config section.
/// </summary>
public class DragalipatchResponse : DragalipatchOptions
{
    public bool UseUnifiedLogin { get; set; }

    public DragalipatchResponse(LoginOptions loginOptions, DragalipatchOptions baseOptions)
    {
        this.Mode = baseOptions.Mode;
        this.ConeshellKey = baseOptions.ConeshellKey;
        this.CdnUrl = baseOptions.CdnUrl;
        this.UseUnifiedLogin = loginOptions.UseBaasLogin;
    }

    public DragalipatchResponse() { }
}
