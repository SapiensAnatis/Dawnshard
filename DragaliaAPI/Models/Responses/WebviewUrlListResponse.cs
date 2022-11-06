using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record WebviewUrlListResponse(WebviewUrlListData data) : BaseResponse<WebviewUrlListData>;

[MessagePackObject(true)]
public record WebviewUrlListData(List<WebviewUrl> webview_url_list);

[MessagePackObject(true)]
public record WebviewUrl(string function_name, string url);

public static class WebviewUrlListStatic
{
    private static readonly string PlaceholderUrl = "localhost";

    public static List<WebviewUrl> GetAllUrls(string host) =>
        new()
        {
            new("ability_crest_advice", PlaceholderUrl),
            new("battle_royal_how_to", PlaceholderUrl),
            new("comic", PlaceholderUrl),
            new("plotsynopsis", PlaceholderUrl),
            new("time_attack_ranking", PlaceholderUrl),
            new("faq", PlaceholderUrl),
            new("help_comic", PlaceholderUrl),
            new("help", PlaceholderUrl),
            new("information", "http://hyperphysics.phy-astr.gsu.edu/hbase/quantum/schr.html#c1"),
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
