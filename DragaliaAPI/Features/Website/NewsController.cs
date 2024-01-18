using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Website;

[Route("[controller]")]
public class NewsController(ApiContext apiContext) : ControllerBase
{
    private readonly ApiContext apiContext = apiContext;

    [HttpGet]
    public async Task<List<NewsItem>> GetNewsItems()
    {
        List<NewsItem> items = await this.apiContext.NewsItems.AsNoTracking()
            .Select(x => new NewsItem(x.Id, x.Headline, x.Time, x.Description))
            .ToListAsync();

        return items;
    }
}

public record NewsItem(int Id, string Headline, DateTimeOffset Time, string Description) { }
