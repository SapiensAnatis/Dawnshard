using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Login.Auth;

[Route("transition")]
internal sealed class TransitionController : DragaliaControllerBaseCore
{
    private readonly IAuthService authService;

    public TransitionController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("transition_by_n_account")]
    [Authorize(AuthenticationSchemes = AuthConstants.SchemeNames.GameJwt)]
    public async Task<DragaliaResult> TransitionByNAccount()
    {
        if (!this.User.HasDawnshardIdentity())
        {
            // This too can apparently be called before /tool/signup
            DbPlayer player = await this.authService.DoSignup(this.User);
            this.User.InitializeDawnshardIdentity(player.AccountId, player.ViewerId);
        }

        (long viewerId, _) = await this.authService.DoLogin(this.User);

        return this.Ok(
            new TransitionTransitionByNAccountResponse()
            {
                TransitionResultData = new()
                {
                    AbolishedViewerId = 0,
                    LinkedViewerId = (ulong)viewerId,
                },
            }
        );
    }
}
