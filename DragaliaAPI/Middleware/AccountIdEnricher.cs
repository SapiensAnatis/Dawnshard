using System.Security.Claims;
using DragaliaAPI.Shared.PlayerDetails;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace DragaliaAPI.Middleware;

public class AccountIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor contextAccessor;

    public AccountIdEnricher(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string? accountId = contextAccessor.HttpContext?.User.FindFirstValue(
            CustomClaimType.AccountId
        );

        if (!string.IsNullOrEmpty(accountId))
        {
            LogEventProperty property = propertyFactory.CreateProperty(
                CustomClaimType.AccountId,
                accountId
            );
            logEvent.AddPropertyIfAbsent(property);
        }

        string? viewerId = contextAccessor.HttpContext?.User.FindFirstValue(
            CustomClaimType.ViewerId
        );

        if (!string.IsNullOrEmpty(viewerId))
        {
            LogEventProperty property = propertyFactory.CreateProperty(
                CustomClaimType.ViewerId,
                long.Parse(viewerId)
            );
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
