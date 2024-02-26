using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;

namespace DragaliaAPI.Test.Controllers;

public class ToolControllerTest
{
    private readonly Mock<IAuthService> mockAuthService;
    private readonly ToolController toolController;

    public ToolControllerTest()
    {
        this.mockAuthService = new Mock<IAuthService>();

        this.toolController = new(this.mockAuthService.Object);
    }

    [Fact]
    public async Task Auth_CallsAuthService()
    {
        this.mockAuthService.Setup(x => x.DoAuth("id token")).ReturnsAsync((1, "session_id"));

        (await this.toolController.Auth("id token"))
            .GetData<ToolAuthResponse>()
            .Should()
            .BeEquivalentTo(
                new ToolAuthResponse()
                {
                    SessionId = "session_id",
                    ViewerId = 1,
                    Nonce = "placeholder nonce"
                }
            );
    }

    [Fact]
    public async Task Signup_CallsAuthService()
    {
        this.mockAuthService.Setup(x => x.DoAuth("id token")).ReturnsAsync((1, "session_id"));

        (await this.toolController.Signup("id token"))
            .GetData<ToolSignupResponse>()
            .Should()
            .BeEquivalentTo(
                new ToolSignupResponse() { ViewerId = 1, },
                opts => opts.Excluding(x => x.ServerTime)
            );
    }
}
