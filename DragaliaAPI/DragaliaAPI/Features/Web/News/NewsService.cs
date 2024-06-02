using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Blazor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Web.News;

public class NewsService(ApiContext apiContext, IOptions<WebOptions> webOptions)
{
    public Task<int> GetNewsItemCountAsync(CancellationToken cancellationToken) =>
        apiContext.NewsItems.CountAsync(cancellationToken);

    public async Task<IList<NewsItem>> GetNewsItemsAsync(
        int offset,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        IQueryable<DbNewsItem> query = apiContext
            .NewsItems.Where(x => !x.Hidden)
            .OrderByDescending(x => x.Date)
            .Skip(offset)
            .Take(pageSize);

        List<DbNewsItem> list = await query.ToListAsync(cancellationToken);

        return list.Select(MapNewsItem).ToList();
    }

    public async Task<NewsItem?> GetNewsItem(int id)
    {
        DbNewsItem? item = await apiContext.NewsItems.FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return null;
        }

        return MapNewsItem(item);
    }

    private NewsItem MapNewsItem(DbNewsItem x) =>
        new()
        {
            Id = x.Id,
            Headline = x.Headline,
            Description = x.Description,
            Date = x.Date,
            HeaderImageSrc = webOptions.Value.GetImageSrc(x.HeaderImagePath),
            BodyImageSrc = webOptions.Value.GetImageSrc(x.BodyImagePath)
        };
}
