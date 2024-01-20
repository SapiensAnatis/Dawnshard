namespace DragaliaAPI.Features.Website;

public record NewsItem(int Id, string Headline, DateTimeOffset Time, string Description) { }
