namespace DragaliaAPI.Models.Options;

public class BaasOptions
{
    public string TokenIssuer { get; set; } = string.Empty;

    public string TokenAudience { get; set; } = string.Empty;

    public string BaasUrl { get; set; } = string.Empty;

    public Uri BaasUrlParsed => new(this.BaasUrl);

    public string ClientId { get; set; } = "default client id";

    public string ChallengeString { get; set; } = "challenge";
}
