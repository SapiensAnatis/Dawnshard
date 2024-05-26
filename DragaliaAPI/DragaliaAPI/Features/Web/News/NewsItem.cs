namespace DragaliaAPI.Features.Web.News;

public record NewsItem
{
    public int Id { get; init; }

    public required string Headline { get; init; }

    public required string Description { get; init; }

    public DateTimeOffset Date { get; init; }

    public string? HeaderImageSrc { get; init; }

    public string? BodyImageSrc { get; init; }
}
