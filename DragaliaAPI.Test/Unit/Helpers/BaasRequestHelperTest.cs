﻿using System.Net.Http.Json;
using System.Text.Json;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;

namespace DragaliaAPI.Test.Unit.Helpers;

public class BaasRequestHelperTest
{
    private readonly IBaasRequestHelper baasRequestHelper;
    private readonly Mock<IOptionsMonitor<BaasOptions>> mockOptions;
    private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
    private readonly Mock<ILogger<BaasRequestHelper>> mockLogger;

    public BaasRequestHelperTest()
    {
        this.mockOptions = new(MockBehavior.Strict);
        this.mockHttpMessageHandler = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.baasRequestHelper = new BaasRequestHelper(
            mockOptions.Object,
            new HttpClient(mockHttpMessageHandler.Object),
            mockLogger.Object
        );
    }

    [Fact]
    public async Task GetKeys_Success_ReturnsSecurityKey()
    {
        this.mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new BaasOptions() { BaasUrl = "https://www.taylorswift.com/" });
        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    x =>
                        x.RequestUri!.ToString()
                            == "https://www.taylorswift.com/.well-known/jwks.json"
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
    public async Task GetKeys_Fail_Throws()
    {
        this.mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new BaasOptions() { BaasUrl = "https://www.taylorswift.com/" });
        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, }
            );

        await this.baasRequestHelper
            .Invoking(x => x.GetKeys())
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == Models.ResultCode.COMMON_AUTH_ERROR);

        this.mockOptions.VerifyAll();
        this.mockHttpMessageHandler.VerifyAll();
    }

    [Fact]
    public async Task GetSavefile_Success_ReturnsSavefile()
    {
        string sampleSaveJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        this.mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new BaasOptions() { BaasUrl = "https://www.taylorswift.com/" });
        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    x =>
                        x.RequestUri!.ToString()
                            == "https://www.taylorswift.com/gameplay/v1/savefile"
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
                    .Deserialize<DragaliaResponse<LoadIndexData>>(
                        sampleSaveJson,
                        UnixDateTimeJsonConverter.Options
                    )!
                    .data
            );
    }

    [Fact]
    public async Task GetSavefile_Fail_Throws()
    {
        this.mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new BaasOptions() { BaasUrl = "https://www.taylorswift.com/" });
        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest, }
            );

        await this.baasRequestHelper
            .Invoking(x => x.GetSavefile("token"))
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == Models.ResultCode.TRANSITION_LINKED_DATA_NOT_FOUND);
    }
}
