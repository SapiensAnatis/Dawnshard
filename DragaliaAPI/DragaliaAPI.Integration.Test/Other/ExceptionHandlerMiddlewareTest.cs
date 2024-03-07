using System.Net;
using DragaliaAPI.Models;
using MessagePack;

namespace DragaliaAPI.Integration.Test.Other;

public class ExceptionHandlerMiddlewareTest : TestFixture
{
    public const string Controller = "test";

    public ExceptionHandlerMiddlewareTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
#if !DEBUG && !TEST
        throw new InvalidOperationException(
            "These tests must be run in a debug build as they use a conditionally compiled controller"
        );
#endif
    }

    [Fact]
    public async Task DragaliaException_ReturnsSerializedResponse()
    {
        DragaliaResponse<ResultCodeResponse> data =
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{Controller}/dragalia",
                new { },
                ensureSuccessHeader: false
            );

        data.Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new DataHeaders(ResultCode.AbilityCrestBuildupPieceShortLevel),
                    new ResultCodeResponse(ResultCode.AbilityCrestBuildupPieceShortLevel)
                )
            );
    }

    [Fact]
    public async Task SecurityTokenExpiredException_ReturnsRefreshRequest_ThenSerializedException()
    {
        this.Client.DefaultRequestHeaders.Add("DeviceId", "id 2");

        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            $"{Controller}/securitytokenexpired",
            new { }
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response
            .Headers.Should()
            .ContainKey("Is-Required-Refresh-Id-Token")
            .WhoseValue.Should()
            .Contain("true");

        HttpResponseMessage secondResponse = await this.Client.PostMsgpackBasic(
            $"{Controller}/securitytokenexpired",
            new { }
        );

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DragaliaResponse<ResultCodeResponse> responseBody = MessagePackSerializer.Deserialize<
            DragaliaResponse<ResultCodeResponse>
        >(await secondResponse.Content.ReadAsByteArrayAsync());

        responseBody
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new DataHeaders(ResultCode.CommonAuthError),
                    new ResultCodeResponse(ResultCode.CommonAuthError)
                )
            );
    }
}
