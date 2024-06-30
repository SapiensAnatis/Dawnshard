using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Webview;

[Route("webview_version")]
[AllowAnonymous]
public class WebviewVersionController(
    IWebHostEnvironment webHostEnvironment,
    IOptionsMonitor<PhotonOptions> photonOptions
) : DragaliaControllerBase
{
    private const string PlaceholderUrl = "localhost";
    private string photonTestUrl =
        $"http://{photonOptions.CurrentValue.ServerUrl.Split(':').FirstOrDefault()}";

    [HttpPost("url_list")]
    public DragaliaResult UrlList()
    {
        AtgenWebviewUrlList timeAttackRanking =
            new("time_attack_ranking", this.GetUrl("timeattack/rankings/webview"));

        AtgenWebviewUrlList timeAttackReward =
            new("time_attack_reward", this.GetUrl("timeattack/rewards/webview"));

        // TODO: Remove hardcoding. Consider making URLs configurable?
        AtgenWebviewUrlList news =
            new("information", "https://test.dawnshard.co.uk/webview/news/1");

        return this.Ok(
            new WebviewVersionUrlListResponse(
                new List<AtgenWebviewUrlList>()
                {
                    news,
                    timeAttackRanking,
                    timeAttackReward,
                    new("ability_crest_advice", PlaceholderUrl),
                    new("battle_royal_how_to", PlaceholderUrl),
                    new("comic", PlaceholderUrl),
                    new("plotsynopsis", PlaceholderUrl),
                    new("faq", PlaceholderUrl),
                    new("help_comic", this.photonTestUrl),
                    new("help", PlaceholderUrl),
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

    private string GetUrl(string relativePath)
    {
        string protocol = webHostEnvironment.IsDevelopment() ? "http" : "https";
        string url = $"{protocol}://{this.HttpContext.Request.Host.Host}/{relativePath}";
        return url;
    }
}
