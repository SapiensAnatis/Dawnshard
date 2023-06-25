using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Integration.Test.Features.Login;

public class LoginTest : TestFixture
{
    public LoginTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LoginIndex_ReturnsSuccess()
    {
        await this.Client
            .Invoking(x => x.PostMsgpack<LoginIndexData>("/login/index", new()))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task LoginVerifyJws_ReturnsOK()
    {
        ResultCodeData response = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "/login/verify_jws",
                new LoginVerifyJwsRequest()
            )
        ).data;

        response.Should().BeEquivalentTo(new ResultCodeData(ResultCode.Success, string.Empty));
    }
}
