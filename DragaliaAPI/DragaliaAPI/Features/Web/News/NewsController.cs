using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.News;

[ApiController]
[Route("/api/news")]
[AllowAnonymous]
internal sealed class NewsController(NewsService newsService) : ControllerBase
{
    [HttpGet]
    public async Task<OffsetPagedResponse<NewsItem>> GetNews(
        [FromQuery] NewsRequest request,
        CancellationToken cancellationToken
    )
    {
        IList<NewsItem> data = await newsService.GetNewsItemsAsync(
            request.Offset,
            request.PageSize,
            cancellationToken
        );
        
        data = data.Select(this.DoTemplateSubstitutions).ToList();

        int totalCount = await newsService.GetNewsItemCountAsync(cancellationToken);

        return new()
        {
            Pagination = new() { TotalCount = totalCount },
            Data = data,
        };
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewsItem>> GetNewsItem(int id)
    {
        NewsItem? item = await newsService.GetNewsItem(id);

        if (item is null)
        {
            return this.NotFound();
        }

        return this.DoTemplateSubstitutions(item);
    }

    private NewsItem DoTemplateSubstitutions(NewsItem newsItem)
    {
        return newsItem with { Description = newsItem.Description.Replace("{{Hostname}}", this.Request.Host.Host) };
    }
}

public sealed record NewsRequest
{
    public int Offset { get; init; }

    public int PageSize { get; init; }
}
