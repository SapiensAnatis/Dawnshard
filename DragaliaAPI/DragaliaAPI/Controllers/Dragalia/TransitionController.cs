using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("transition")]
[AllowAnonymous]
public class TransitionController : DragaliaControllerBase
{
    private readonly IAuthService authService;

    public TransitionController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("transition_by_n_account")]
    public async Task<DragaliaResult> TransitionByNAccount(
        TransitionTransitionByNAccountRequest request
    )
    {
        (long viewerId, _) = await this.authService.DoAuth(request.IdToken);

        return this.Ok(
            new TransitionTransitionByNAccountResponse()
            {
                TransitionResultData = new()
                {
                    AbolishedViewerId = 0,
                    LinkedViewerId = (ulong)viewerId,
                }
            }
        );
    }
}
