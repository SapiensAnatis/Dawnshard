using DragaliaAPI.Models;

namespace DragaliaAPI.Test.Integration.Other;

public class ExceptionHandlerMiddlewareTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public const string Controller = "/test";

    public ExceptionHandlerMiddlewareTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient();
    }

    [Theory]
    [InlineData("taskcancelled")]
    [InlineData("messagepackserialization/taskcancelled")]
    [InlineData("messagepackserialization/operationcancelled")]
    public async Task TransientException_Returns503(string route)
    {
        client.DefaultRequestHeaders.Add("SID", "session_id");

        HttpResponseMessage response = await client.GetAsync($"{Controller}/{route}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task DragaliaException_ReturnsSerializedResponse()
    {
        var data = await this.client.PostMsgpack<ResultCodeData>(
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
