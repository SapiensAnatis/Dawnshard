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
        (long viewerId, _) = await this.authService.DoAuth(request.id_token);

        return this.Ok(
            new TransitionTransitionByNAccountData()
            {
                transition_result_data = new()
                {
                    abolished_viewer_id = 0,
                    linked_viewer_id = (ulong)viewerId,
                }
            }
        );
    }
}
