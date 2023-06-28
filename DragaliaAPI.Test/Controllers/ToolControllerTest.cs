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

        (
            await this.toolController.Auth(
                new ToolAuthRequest() { id_token = "id token", uuid = "uuid" }
            )
        )
            .GetData<ToolAuthData>()
            .Should()
            .BeEquivalentTo(
                new ToolAuthData()
                {
                    session_id = "session_id",
                    viewer_id = 1,
                    nonce = "placeholder nonce"
                }
            );
    }

    [Fact]
    public async Task Signup_CallsAuthService()
    {
        this.mockAuthService.Setup(x => x.DoAuth("id token")).ReturnsAsync((1, "session_id"));

        (
            await this.toolController.Signup(
                new ToolSignupRequest() { id_token = "id token", uuid = "uuid" }
            )
        )
            .GetData<ToolSignupData>()
            .Should()
            .BeEquivalentTo(
                new ToolSignupData() { viewer_id = 1, },
                opts => opts.Excluding(x => x.servertime)
            );
    }
}
