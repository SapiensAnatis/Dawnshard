using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.News;

[Route("/api/news")]
[AllowAnonymous]
public sealed class NewsController(NewsService newsService) : ControllerBase
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

        int totalCount = await newsService.GetNewsItemCountAsync(cancellationToken);

        return new()
        {
            Pagination = new() { TotalCount = totalCount, },
            Data = data
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

        return item;
    }
}

public sealed record NewsRequest
{
    public int Offset { get; init; }

    public int PageSize { get; init; }
}
