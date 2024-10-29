using DragaliaAPI.Features.Version;

namespace DragaliaAPI.Integration.Test.Features.Version;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Version.VersionController"/>
/// </summary>
public class VersionTest : TestFixture
{
    private readonly HttpClient httpClient;
    
    public VersionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { this.httpClient = this.CreateClient(); }

    [Theory]
    [InlineData(Platform.Ios, "b1HyoeTFegeTexC0")]
    [InlineData(Platform.Android, "y2XM6giU6zz56wCm")]
    public async Task GetResourceVersion_ReturnsCorrectResponse(
        Platform platform,
        string expectedVersion
    )
    {
        VersionGetResourceVersionResponse response = (
            await this.Client.PostMsgpack<VersionGetResourceVersionResponse>(
                "version/get_resource_version",
                new VersionGetResourceVersionRequest(platform, "whatever")
            )
        ).Data;

        response.ResourceVersion.Should().Be(expectedVersion);
    }

    [Fact]
    public async Task ResourceVersionMismatch_ReturnsError()
    {
        this.httpClient.DefaultRequestHeaders.Remove("Res-Ver");
        this.httpClient.DefaultRequestHeaders.Add("Res-Ver", "aaaaaaa");

        (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "fort/get_data",
                ensureSuccessHeader: false
            )
        )
            .DataHeaders.ResultCode.Should()
            .Be(ResultCode.CommonResourceVersionError);
    }

    [Fact]
    public async Task ResourceVersionMismatch_ExemptController_ReturnsSuccess()
    {
        this.httpClient.DefaultRequestHeaders.Remove("Res-Ver");
        this.httpClient.DefaultRequestHeaders.Add("Res-Ver", "aaaaaaa");

        (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "tool/get_service_status",
                ensureSuccessHeader: false
            )
        )
            .DataHeaders.ResultCode.Should()
            .Be(ResultCode.Success);
    }
}
