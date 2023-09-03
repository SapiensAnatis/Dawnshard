using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

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
        EulaGetVersionListData response = (
            await this.Client.PostMsgpack<EulaGetVersionListData>(
                "eula/get_version_list",
                new EulaGetVersionListRequest()
            )
        ).data;

        response.version_hash_list
            .Should()
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
        EulaGetVersionData response = (
            await this.Client.PostMsgpack<EulaGetVersionData>(
                "eula/get_version",
                new EulaGetVersionRequest("id_token", "gb", "en_eu")
            )
        ).data;

        response
            .Should()
            .BeEquivalentTo(
                new EulaGetVersionData(new AtgenVersionHash("gb", "en_eu", 1, 1), false, 1)
            );
    }

    [Fact]
    public async Task EulaGetVersion_InvalidRegionOrLocale_ReturnsDefault()
    {
        EulaGetVersionData response = (
            await this.Client.PostMsgpack<EulaGetVersionData>(
                "eula/get_version",
                new EulaGetVersionRequest("id_token", "not even a country", "c#")
            )
        ).data;

        response
            .Should()
            .BeEquivalentTo(
                new EulaGetVersionData(new AtgenVersionHash("gb", "en_us", 1, 1), false, 1)
            );
    }
}
