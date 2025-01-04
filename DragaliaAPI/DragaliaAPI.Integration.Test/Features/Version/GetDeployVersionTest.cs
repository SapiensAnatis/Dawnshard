namespace DragaliaAPI.Integration.Test.Features.Version;

public class GetDeployVersionTest : TestFixture
{
    public GetDeployVersionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetDeployVersion_ReturnsCorrectResponse()
    {
        DeployGetDeployVersionResponse response = (
            await this.Client.PostMsgpack<DeployGetDeployVersionResponse>(
                "deploy/get_deploy_version",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.DeployHash.Should().Be("13bb2827ce9e6a66015ac2808112e3442740e862");
    }
}
