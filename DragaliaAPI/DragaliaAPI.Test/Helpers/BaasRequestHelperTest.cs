using System.Text.Json;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;

namespace DragaliaAPI.Test.Helpers;

public class BaasRequestHelperTest
{
    private readonly IBaasApi baasRequestHelper;
    private readonly Mock<IOptionsMonitor<BaasOptions>> mockOptions;
    private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
    private readonly Mock<ILogger<BaasApi>> mockLogger;
    private IDistributedCache cache;

    public BaasRequestHelperTest()
    {
        this.mockOptions = new(MockBehavior.Strict);
        this.mockHttpMessageHandler = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.mockOptions.SetupGet(x => x.CurrentValue)
            .Returns(new BaasOptions() { BaasUrl = "https://www.taylorswift.com/" });

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );
        this.cache = new MemoryDistributedCache(opts);

        this.baasRequestHelper = new BaasApi(
            mockOptions.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            this.cache,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task GetKeys_Success_ReturnsSecurityKey()
    {
        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri!.ToString() == "https://www.taylorswift.com/.well-known/jwks.json"
                    && x.Method == HttpMethod.Get
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(
                        """
                        {
                          "keys": [
                            {
                              "alg": "RS256",
                              "kty": "RSA",
                              "use": "sig",
                              "x5c": [],
                              "n": "waYgIYhr3xvlbRgEjGwJO4hwMiPUqdKHfo6WIRJZsBGXwty8MBI7p8rIIhYFaLP_4rqVYGgAwP_X5gqd7lGUvg9xMrpHglzTQhwzE4TTmiVunkihj0ICpClboKV1Hd_owZwazeDSfAgrFS9fx8EGAc1qQY7KjlWPli9SjyZjSgJQzzVyiCEAu9Cuhc-4gtREePPYrUO9DUB2a_TWaonh2VLY4-2HhIFMC8-Kf2PVzIrxezNY6e1Dk0Jk__RtM8sGsq-TO-Rr1q3CXAgLstC7RXhwWRggaZ29or-OrkoLeIvCodAnPElrZWjXj9j0cJxDLJt-H4jCGdou_miIN-w3KQ",
                              "e": "AQAB",
                              "kid": "396f3735-4fa4-4a75-8924-f4e7a21ee62d",
                              "x5t": null
                            }
                          ]
                        }
                        """
                    )
                }
            );

        (await this.baasRequestHelper.GetKeys()).Should().ContainSingle();

        this.mockOptions.VerifyAll();
        this.mockHttpMessageHandler.VerifyAll();
    }

    [Fact]
    public async Task GetKeys_Cached_UsesCache()
    {
        this.cache.SetString(
            ":jwks:baas",
            """
            {
                "keys": [
                {
                    "alg": "RS256",
                    "kty": "RSA",
                    "use": "sig",
                    "x5c": [],
                    "n": "waYgIYhr3xvlbRgEjGwJO4hwMiPUqdKHfo6WIRJZsBGXwty8MBI7p8rIIhYFaLP_4rqVYGgAwP_X5gqd7lGUvg9xMrpHglzTQhwzE4TTmiVunkihj0ICpClboKV1Hd_owZwazeDSfAgrFS9fx8EGAc1qQY7KjlWPli9SjyZjSgJQzzVyiCEAu9Cuhc-4gtREePPYrUO9DUB2a_TWaonh2VLY4-2HhIFMC8-Kf2PVzIrxezNY6e1Dk0Jk__RtM8sGsq-TO-Rr1q3CXAgLstC7RXhwWRggaZ29or-OrkoLeIvCodAnPElrZWjXj9j0cJxDLJt-H4jCGdou_miIN-w3KQ",
                    "e": "AQAB",
                    "kid": "396f3735-4fa4-4a75-8924-f4e7a21ee62d",
                    "x5t": null
                }
                ]
            }
            """
        );

        (await this.baasRequestHelper.GetKeys()).Should().ContainSingle();

        this.mockOptions.VerifyAll();
        this.mockHttpMessageHandler.VerifyAll();
    }

    [Fact]
    public async Task GetKeys_Fail_Throws()
    {
        this.mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, }
            );

        await this
            .baasRequestHelper.Invoking(x => x.GetKeys())
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.CommonAuthError);

        this.mockOptions.VerifyAll();
        this.mockHttpMessageHandler.VerifyAll();
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
                    Content = new StringContent(sampleSaveJson)
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
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest, }
            );

        await this
            .baasRequestHelper.Invoking(x => x.GetSavefile("token"))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.TransitionLinkedDataNotFound);
    }
}
