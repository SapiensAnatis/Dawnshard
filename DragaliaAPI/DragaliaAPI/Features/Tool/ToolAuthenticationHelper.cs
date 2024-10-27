using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Features.Tool;

internal static class ToolAuthenticationHelper
{
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        if (context.Request.Headers.TryGetValue("ID-TOKEN", out StringValues idToken))
        {
            context.Token = idToken.FirstOrDefault();
        }

        return Task.CompletedTask;
    }
}
