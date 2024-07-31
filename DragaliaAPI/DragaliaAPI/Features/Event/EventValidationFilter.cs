using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Event;

/// <summary>
/// Runs for event-related requests to validate the relevant event can currently be accessed.
/// </summary>
public partial class EventValidationFilter(
    IOptionsMonitor<EventOptions> optionsMonitor,
    TimeProvider timeProvider,
    ILogger<EventValidationFilter> logger
) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        IEventRequest? request = context
            .ActionArguments.Values.OfType<IEventRequest>()
            .FirstOrDefault();

        if (request is null)
        {
            Log.EventRequestNotFound(logger, context.ActionDescriptor.DisplayName);
            return;
        }

        if (
            MasterAsset.EventData.TryGetValue(request.EventId, out EventData? masterAssetItem)
            && masterAssetItem.IsMemoryEvent
        )
        {
            return;
        }

        EventRunInformation? runInfo = optionsMonitor.CurrentValue.EventList.Find(x =>
            x.Id == request.EventId
        );

        if (runInfo is null)
        {
            Log.EventNotFound(logger, request.EventId);
            SetValidationFailedResponse(context);

            return;
        }

        if (!runInfo.IsActive(timeProvider))
        {
            Log.EventOutOfPeriod(logger, runInfo);
            SetValidationFailedResponse(context);

            return;
        }

        Log.EventIsValid(logger);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void SetValidationFailedResponse(ActionExecutingContext context) =>
        context.Result = new DragaliaResult<ResultCodeResponse>(
            new ResultCodeResponse(ResultCode.EventOutThePeriod),
            ResultCode.EventOutThePeriod
        ).Convert();

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Trace, "No IEventRequest parameter found for action {ActionName}")]
        public static partial void EventRequestNotFound(ILogger logger, string? actionName);

        [LoggerMessage(
            LogLevel.Information,
            "Rejecting request: Event ID {EventID} not found in configuration"
        )]
        public static partial void EventNotFound(ILogger logger, int eventId);

        [LoggerMessage(
            LogLevel.Information,
            "Rejecting request: Event {@RunInfo} is not currently active"
        )]
        public static partial void EventOutOfPeriod(ILogger logger, EventRunInformation runInfo);

        [LoggerMessage(LogLevel.Trace, "Event request validated successfully")]
        public static partial void EventIsValid(ILogger logger);
    }
}
