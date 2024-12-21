using System.Security.Claims;
using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Login.Auth;

internal interface IAuthService
{
    Task<AuthResult> DoLogin(ClaimsPrincipal claimsPrincipal);
    Task<DbPlayer> DoSignup(ClaimsPrincipal claimsPrincipal);
    Task ImportSaveIfPending(ClaimsPrincipal claimsPrincipal);
}
