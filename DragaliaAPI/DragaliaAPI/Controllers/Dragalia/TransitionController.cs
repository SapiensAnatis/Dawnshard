using DragaliaAPI.Features.Tool;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("transition")]
[AllowAnonymous]
internal sealed class TransitionController : DragaliaControllerBase
{
    private readonly IAuthService authService;

    public TransitionController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("transition_by_n_account")]
    [Authorize(AuthenticationSchemes = AuthConstants.SchemeNames.GameJwt)]
    public async Task<DragaliaResult> TransitionByNAccount(
        TransitionTransitionByNAccountRequest request
    )
    {
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
