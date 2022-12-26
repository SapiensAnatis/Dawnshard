namespace DragaliaAPI.Services.Options;

public class DragaliaAuthOptions
{
    public string TokenIssuer { get; set; } = string.Empty;

    public string TokenAudience { get; set; } = string.Empty;

    public string BaasUrl { get; set; } = string.Empty;

    public bool UseLegacyLogin { get; set; }
}
