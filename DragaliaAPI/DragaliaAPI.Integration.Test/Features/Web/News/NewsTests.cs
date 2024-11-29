using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Web;
using DragaliaAPI.Features.Web.News;

namespace DragaliaAPI.Integration.Test.Features.Web.News;

public class NewsTests : WebTestFixture
{
    public NewsTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper)
    {
        this.ApiContext.NewsItems.AddRange(
            Enumerable
                .Range(1, 6)
                .Select(x => new DbNewsItem()
                {
                    Headline = $"News Item {x}",
                    Description = $"News Item Description {x}",
                })
        );
        this.ApiContext.SaveChanges();
    }

    [Fact]
    public async Task GetNewsItems_Returns200PagedResponse()
    {
        HttpResponseMessage page1Response = await this.Client.GetAsync(
            "/api/news?offset=0&pageSize=5",
            TestContext.Current.CancellationToken
        );

        page1Response.Should().HaveStatusCode(HttpStatusCode.OK);

        (
            await page1Response.Content.ReadFromJsonAsync<OffsetPagedResponse<NewsItem>>(
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeEquivalentTo(
                new OffsetPagedResponse<NewsItem>()
                {
                    Pagination = new() { TotalCount = 6 },
                    Data = Enumerable
                        .Range(1, 5)
                        .Select(x => new NewsItem()
                        {
                            Headline = $"News Item {x}",
                            Description = $"News Item Description {x}",
                        })
                        .ToList(),
                },
                opts => opts.For(x => x.Data).Exclude(x => x.Id)
            );

        HttpResponseMessage page2Response = await this.Client.GetAsync(
            "/api/news?offset=5&pageSize=5",
            TestContext.Current.CancellationToken
        );

        page2Response.Should().HaveStatusCode(HttpStatusCode.OK);

        (
            await page2Response.Content.ReadFromJsonAsync<OffsetPagedResponse<NewsItem>>(
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeEquivalentTo(
                new OffsetPagedResponse<NewsItem>()
                {
                    Pagination = new() { TotalCount = 6 },
                    Data =
                    [
                        new()
                        {
                            Headline = $"News Item 6",
                            Description = $"News Item Description 6",
                        },
                    ],
                },
                opts => opts.For(x => x.Data).Exclude(x => x.Id)
            );
    }

    [Fact]
    public async Task GetNewsItems_DoesNotShowHiddenItem()
    {
        DbNewsItem hiddenItem =
            new()
            {
                Hidden = true,
                Headline = "hidden",
                Description = "hidden",
            };

        this.ApiContext.NewsItems.Add(hiddenItem);
        this.ApiContext.SaveChanges();

        HttpResponseMessage allItems = await this.Client.GetAsync(
            "/api/news?offset=0&pageSize=100",
            TestContext.Current.CancellationToken
        );

        (
            await allItems.Content.ReadFromJsonAsync<OffsetPagedResponse<NewsItem>>(
                cancellationToken: TestContext.Current.CancellationToken
            )
        )!
            .Data.Should()
            .NotContain(x => x.Id == hiddenItem.Id);
    }

    [Fact]
    public async Task GetNewsItem_CanViewHiddenItem()
    {
        DbNewsItem hiddenItem =
            new()
            {
                Hidden = true,
                Headline = "hidden",
                Description = "hidden",
            };

        this.ApiContext.NewsItems.Add(hiddenItem);
        this.ApiContext.SaveChanges();

        HttpResponseMessage specificItemResponse = await this.Client.GetAsync(
            $"/api/news/{hiddenItem.Id}",
            TestContext.Current.CancellationToken
        );

        (
            await specificItemResponse.Content.ReadFromJsonAsync<NewsItem>(
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeEquivalentTo(
                new NewsItem()
                {
                    Id = hiddenItem.Id,
                    Headline = hiddenItem.Headline,
                    Description = hiddenItem.Description,
                }
            );
    }
}
