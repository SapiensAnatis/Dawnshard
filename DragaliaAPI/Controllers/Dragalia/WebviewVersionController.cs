using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("webview_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UrlListController : DragaliaControllerBase
{
    private const string PlaceholderUrl = "localhost";

    [HttpPost]
    [Route("url_list")]
    public DragaliaResult UrlList()
    {
        // Webview URLs such as localhost:5000/News are not considered acceptable by the game;
        // the webview pages will only load when the server is deployed to a dedicated domain.
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
                    new("information", "192.168.1.104:5000/News"),
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
