namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.EulaController"/>
/// </summary>
public class EulaTest : TestFixture
{
    public EulaTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task EulaGetVersionList_ReturnsAllVersions()
    {
        EulaGetVersionListResponse response = (
            await this.Client.PostMsgpack<EulaGetVersionListResponse>("eula/get_version_list")
        ).Data;

        response
            .VersionHashList.Should()
            .BeEquivalentTo(
                new List<AtgenVersionHash>()
                {
                    new("gb", "en_us", 1, 1),
                    new("gb", "en_eu", 1, 1),
                    new("us", "en_us", 1, 6),
                    new("us", "en_eu", 1, 6)
                }
            );
    }

    [Fact]
    public async Task EulaGetVersion_ValidRegionAndLocale_ReturnsEulaData()
    {
        EulaGetVersionResponse response = (
            await this.Client.PostMsgpack<EulaGetVersionResponse>(
                "eula/get_version",
                new EulaGetVersionRequest("id_token", "gb", "en_eu")
            )
        ).Data;

        response
            .Should()
            .BeEquivalentTo(
                new EulaGetVersionResponse(new AtgenVersionHash("gb", "en_eu", 1, 1), false, 1)
            );
    }

    [Fact]
    public async Task EulaGetVersion_InvalidRegionOrLocale_ReturnsDefault()
    {
        EulaGetVersionResponse response = (
            await this.Client.PostMsgpack<EulaGetVersionResponse>(
                "eula/get_version",
                new EulaGetVersionRequest("id_token", "not even a country", "c#")
            )
        ).Data;

        response
            .Should()
            .BeEquivalentTo(
                new EulaGetVersionResponse(new AtgenVersionHash("gb", "en_us", 1, 1), false, 1)
            );
    }
}
