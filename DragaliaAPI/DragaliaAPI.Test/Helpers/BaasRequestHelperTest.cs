using System.Text.Json;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq.Protected;

namespace DragaliaAPI.Test.Helpers;

public class BaasRequestHelperTest
{
    private readonly IBaasApi baasRequestHelper;
    private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
    private readonly IDistributedCache cache;

    public BaasRequestHelperTest()
    {
        this.mockHttpMessageHandler = new(MockBehavior.Strict);

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );
        this.cache = new MemoryDistributedCache(opts);

        this.baasRequestHelper = new BaasApi(
            new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://www.taylorswift.com"),
            },
            this.cache,
            NullLogger<BaasApi>.Instance
        );
    }

    [Fact]
    public async Task GetSavefile_Success_ReturnsSavefile()
    {
        string sampleSaveJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri!.ToString() == "https://www.taylorswift.com/gameplay/v1/savefile"
                    && x.Method == HttpMethod.Post
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(sampleSaveJson),
                }
            );

        (await this.baasRequestHelper.GetSavefile("token"))
            .Should()
            .BeEquivalentTo(
                JsonSerializer
                    .Deserialize<DragaliaResponse<LoadIndexResponse>>(
                        sampleSaveJson,
                        ApiJsonOptions.Instance
                    )!
                    .Data
            );
    }

    [Fact]
    public async Task GetSavefile_Fail_Throws()
    {
        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest }
            );

        await this
            .baasRequestHelper.Invoking(x => x.GetSavefile("token"))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.TransitionLinkedDataNotFound);
    }
}
