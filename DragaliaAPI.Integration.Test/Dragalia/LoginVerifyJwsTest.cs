using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.LoginController"/>
/// </summary>
public class LoginVerifyJwsTest : TestFixture
{
    public LoginVerifyJwsTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task VerifyJws_ReturnsOK()
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
