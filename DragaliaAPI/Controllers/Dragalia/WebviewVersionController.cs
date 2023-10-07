﻿using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("webview_version")]
[AllowAnonymous]
public class WebviewVersionController : DragaliaControllerBase
{
    private const string PlaceholderUrl = "localhost";

    [HttpPost("url_list")]
    public DragaliaResult UrlList()
    {
        AtgenWebviewUrlList timeAttackRanking =
            new("time_attack_ranking", this.GetUrl("timeattack/rankings/webview"));

        AtgenWebviewUrlList timeAttackReward =
            new("time_attack_reward", this.GetUrl("timeattack/rewards/webview"));

        AtgenWebviewUrlList news = new("information", this.GetUrl("news/webview"));

        return this.Ok(
            new WebviewVersionUrlListData(
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
                    new("help_comic", PlaceholderUrl),
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
        string protocol = $"http{(this.HttpContext.Request.IsHttps ? "s" : "")}";
        string url = $"{protocol}://{this.HttpContext.Request.Host.Host}/{relativePath}";
        return url;
    }
}
