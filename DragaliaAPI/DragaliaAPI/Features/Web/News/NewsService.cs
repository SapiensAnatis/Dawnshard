using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.News;

internal sealed class NewsService(ApiContext apiContext)
{
    public Task<int> GetNewsItemCountAsync(CancellationToken cancellationToken) =>
        apiContext.NewsItems.CountAsync(cancellationToken);

    public async Task<IList<NewsItem>> GetNewsItemsAsync(
        int offset,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        IQueryable<NewsItem> query = apiContext
            .NewsItems.Where(x => !x.Hidden)
            .ProjectToNewsItem()
            .OrderByDescending(x => x.Date)
            .Skip(offset)
            .Take(pageSize);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<NewsItem?> GetNewsItem(int id)
    {
        return await apiContext.NewsItems.ProjectToNewsItem().FirstOrDefaultAsync(x => x.Id == id);
    }
}

internal static class NewsMappingExtensions
{
    public static IQueryable<NewsItem> ProjectToNewsItem(this IQueryable<DbNewsItem> newsItems) =>
        newsItems.Select(x => new NewsItem()
        {
            Id = x.Id,
            Headline = x.Headline,
            Description = x.Description,
            Date = x.Date,
            HeaderImagePath = x.HeaderImagePath,
            HeaderImageAltText = x.HeaderImageAltText,
            BodyImagePath = x.BodyImagePath,
            BodyImageAltText = x.BodyImageAltText,
        });
}
