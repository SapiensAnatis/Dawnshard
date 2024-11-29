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
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            );

        data.Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new ResultCodeResponse(ResultCode.AbilityCrestBuildupPieceShortLevel),
                    new DataHeaders(ResultCode.AbilityCrestBuildupPieceShortLevel)
                )
            );
    }
}
