using DragaliaAPI.Models;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Other;

public class ExceptionHandlerMiddlewareTest : TestFixture
{
    public const string Controller = "/test";

    public ExceptionHandlerMiddlewareTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task DragaliaException_ReturnsSerializedResponse()
    {
        DragaliaResponse<ResultCodeData> data = await this.Client.PostMsgpack<ResultCodeData>(
            $"{Controller}/dragalia",
            new { },
            ensureSuccessHeader: false
        );

        data.Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.AbilityCrestBuildupPieceShortLevel),
                    new ResultCodeData(ResultCode.AbilityCrestBuildupPieceShortLevel)
                )
            );
    }
}
