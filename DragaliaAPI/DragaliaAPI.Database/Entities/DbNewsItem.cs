using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbNewsItem
{
    /// <summary>
    /// Gets or sets the ID of the news item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a property indicating if the item should be returned as part of the whole news list.
    /// If <see langword="true"/>, the news item is not returned from GET /news and only returned via GET /news/{id}.
    /// </summary>
    /// <remarks>
    /// This is used for the Mercurial Gauntlet rewards pop-up, which is just a webview that hijacks the news page with
    /// a specific item ID. It can be viewed by browsing to /webview/news/1#detail/20000 but is not normally shown in
    /// the news list.
    /// </remarks>
    public bool Hidden { get; set; }

    /// <summary>
    /// Gets or sets the headline of the news item.
    /// </summary>
    public required string Headline { get; set; }

    /// <summary>
    /// Gets or sets the date the news item was published.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Gets or sets the description / body text of the news item.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets a relative path to the summary header image.
    /// </summary>
    /// <remarks>
    /// Will be combined with the configured CDN base address to deliver a fully qualified URL in the response.
    /// </remarks>
    public string? HeaderImagePath { get; set; }

    /// <summary>
    /// Gets or sets a relative path to the body image.
    /// </summary>
    /// <remarks>
    /// Will be combined with the configured CDN base address to deliver a fully qualified URL in the response.
    /// </remarks>
    public string? BodyImagePath { get; set; }
}

public class DbNewsItemConfiguration : IEntityTypeConfiguration<DbNewsItem>
{
    public void Configure(EntityTypeBuilder<DbNewsItem> builder)
    {
        builder.Property(x => x.Headline).HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(4096);
        builder.Property(x => x.HeaderImagePath).HasMaxLength(512);
        builder.Property(x => x.BodyImagePath).HasMaxLength(512);

        builder.HasData(
            new DbNewsItem()
            {
                Id = 20000,
                Headline = "Mercurial Gauntlet Endeavour Rewards",
                Hidden = true,
                Description =
                    "The below infographic shows the endeavour rewards available for the progressing the Mercurial Gauntlet.",
                BodyImagePath = "/dawnshard/news/mg-endeavours.webp",
                Date = new DateTimeOffset(2024, 06, 02, 16, 07, 00, TimeSpan.FromHours(1))
            }
        );
    }
}
