using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("webview_version")]
[AllowAnonymous]
public class WebviewVersionController : DragaliaControllerBase
{
    private const string PlaceholderUrl = "localhost";

    [HttpPost("url_list")]
    public DragaliaResult UrlList()
    {
        string baseAddress = this.HttpContext.Request.Host.ToString();

        // Use this URL instead if using mitmproxy and developing locally
        // (replacing it with <host local IP>:<container port> as needed)
        // baseAddress = "dd-api.lukefz.xyz";

        return Ok(
            new WebviewVersionUrlListData(
                new List<AtgenWebviewUrlList>()
                {
                    new("ability_crest_advice", PlaceholderUrl),
                    new("battle_royal_how_to", PlaceholderUrl),
                    new("comic", PlaceholderUrl),
                    new("plotsynopsis", PlaceholderUrl),
                    new("time_attack_ranking", PlaceholderUrl),
                    new("faq", PlaceholderUrl),
                    new("help_comic", PlaceholderUrl),
                    new("help", PlaceholderUrl),
                    new("information", $"https://{baseAddress}/news?hideappbar=true"),
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
                }
            )
        );
    }
}
