using System.Security.Claims;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Tool;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace DragaliaAPI.Test.Features.Tool;

public class ToolControllerTest
{
    private readonly IAuthService mockAuthService;
    private readonly ToolController toolController;

    public ToolControllerTest()
    {
        this.mockAuthService = Substitute.For<IAuthService>();

        this.toolController = new(this.mockAuthService);

        this.toolController.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new(
                    new ClaimsIdentity(
                        new List<Claim>()
                        {
                            new Claim(CustomClaimType.AccountId, "AccountId"),
                            new Claim(CustomClaimType.ViewerId, "1"),
                        }
                    )
                    {
                        Label = AuthConstants.IdentityLabels.Dawnshard,
                    }
                ),
            },
        };
    }

    [Fact]
    public async Task Auth_CallsAuthService()
    {
        this.mockAuthService.DoLogin(Arg.Any<ClaimsPrincipal>())
            .Returns(new AuthResult(1, "session_id"));

        (await this.toolController.Auth())
            .GetData<ToolAuthResponse>()
            .Should()
            .BeEquivalentTo(
                new ToolAuthResponse()
                {
                    SessionId = "session_id",
                    ViewerId = 1,
                    Nonce = "placeholder nonce",
                }
            );
    }

    [Fact]
    public async Task Signup_CallsAuthService()
    {
        this.toolController.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new(
                    new ClaimsIdentity(
                        []
                    )
                ),
            },
        };
        
        this.mockAuthService.DoSignup(Arg.Any<ClaimsPrincipal>())
            .Returns(new DbPlayer() { ViewerId = 1, AccountId = "id" });

        (await this.toolController.Signup())
            .GetData<ToolSignupResponse>()
            .Should()
            .BeEquivalentTo(
                new ToolSignupResponse() { ViewerId = 1 },
                opts => opts.Excluding(x => x.ServerTime)
            );
    }
}
