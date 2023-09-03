using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class GetDeployVersionTest : TestFixture
{
    public GetDeployVersionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetDeployVersion_ReturnsCorrectResponse()
    {
        DeployGetDeployVersionData response = (
            await this.Client.PostMsgpack<DeployGetDeployVersionData>(
                "deploy/get_deploy_version",
                new DeployGetDeployVersionRequest()
            )
        ).data;

        response.deploy_hash.Should().Be("13bb2827ce9e6a66015ac2808112e3442740e862");
    }
}
