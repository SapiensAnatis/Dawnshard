namespace DragaliaAPI.Models.Dragalia.Responses;

public record UrlListResponse : BaseResponse<UrlListData>
{
    public override UrlListData data { get; init; } = new(UrlListStaticData.AllUrls);
}

public record UrlListData(List<WebviewUrl> webview_url_list);

public record WebviewUrl(string function_name, string url);

public static class UrlListStaticData
{
    public static string PlaceholderUrl = "localhost";
    public static List<WebviewUrl> AllUrls = new()
    {
        new("ability_crest_advice", PlaceholderUrl),
        new("battle_royal_how_to", PlaceholderUrl),
        new("comic", PlaceholderUrl),
        new("plotsynopsis", PlaceholderUrl),
        new("time_attack_ranking", PlaceholderUrl),
        new("faq", PlaceholderUrl),
        new("help_comic", PlaceholderUrl),
        new("help", PlaceholderUrl),
        new("information", PlaceholderUrl),
        new("inquiry_attention", PlaceholderUrl),
        new("dragon_battle_info", PlaceholderUrl),
        new("quest_info", PlaceholderUrl),
        new("copyright", PlaceholderUrl),
        new("health", PlaceholderUrl),
        new("payment_services_act", PlaceholderUrl),
        new("privacy_policy", PlaceholderUrl),
        new("specified_commercial_transactions_law", PlaceholderUrl),
        new("user_policy", PlaceholderUrl),
        new("summon_info", PlaceholderUrl),
    };
}