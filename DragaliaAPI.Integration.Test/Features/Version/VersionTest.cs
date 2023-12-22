using DragaliaAPI.Features.Version;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Version.VersionController"/>
/// </summary>
public class VersionTest : TestFixture
{
    public VersionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Theory]
    [InlineData(Platform.Ios, "b1HyoeTFegeTexC0")]
    [InlineData(Platform.Android, "y2XM6giU6zz56wCm")]
    public async Task GetResourceVersion_ReturnsCorrectResponse(
        Platform platform,
        string expectedVersion
    )
    {
        VersionGetResourceVersionData response = (
            await this.Client.PostMsgpack<VersionGetResourceVersionData>(
                "version/get_resource_version",
                new VersionGetResourceVersionRequest(platform, "whatever")
            )
        ).data;

        response.resource_version.Should().Be(expectedVersion);
    }

    [Fact]
    public async Task ResourceVersionMismatch_ReturnsError()
    {
        this.Client.DefaultRequestHeaders.Remove("Res-Ver");
        this.Client.DefaultRequestHeaders.Add("Res-Ver", "aaaaaaa");

        (
            await this.Client.PostMsgpack<ResultCodeData>(
                "fort/get_data",
                new FortGetDataRequest(),
                ensureSuccessHeader: false
            )
        )
            .data_headers.result_code.Should()
            .Be(ResultCode.CommonResourceVersionError);
    }

    [Fact]
    public async Task ResourceVersionMismatch_ExemptController_ReturnsSuccess()
    {
        this.Client.DefaultRequestHeaders.Remove("Res-Ver");
        this.Client.DefaultRequestHeaders.Add("Res-Ver", "aaaaaaa");

        (
            await this.Client.PostMsgpack<ResultCodeData>(
                "tool/get_service_status",
                new FortGetDataRequest(),
                ensureSuccessHeader: false
            )
        )
            .data_headers.result_code.Should()
            .Be(ResultCode.Success);
    }
}
