using System.Security.Claims;

namespace DragaliaAPI.Features.Tool;

internal interface IAuthService
{
    Task<AuthResult> DoLogin(ClaimsPrincipal claimsPrincipal);
    Task<long> DoSignup(ClaimsPrincipal user);
    Task ImportSaveIfPending(ClaimsPrincipal claimsPrincipal);
}
