using System.Security.Claims;

namespace DragaliaAPI.Features.Tool;

internal interface IAuthService
{
    Task<AuthResult> DoLogin(ClaimsPrincipal claimsPrincipal);
    Task<long> DoSignup();
    Task ImportSaveIfPending(ClaimsPrincipal claimsPrincipal);
}
