using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Version;
using DragaliaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DragaliaAPI.Middleware;

public class ResourceVersionActionFilter(
    IResourceVersionService resourceVersionService,
    ILogger<ResourceVersionActionFilter> logger
) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (
            context
                .HttpContext.GetEndpoint()
                ?.Metadata
                .GetMetadata<BypassResourceVersionCheckAttribute>()
            is not null
        )
        {
            return;
        }

        string? clientResourceVer = context.HttpContext.Request.Headers["Res-Ver"].FirstOrDefault();
        if (clientResourceVer is null)
            return;

        string? platformString = context.HttpContext.Request.Headers["Platform"].FirstOrDefault();
        if (!Enum.TryParse(platformString, out Platform platform) && Enum.IsDefined(platform))
            return;

        string serverResourceVer = resourceVersionService.GetResourceVersion(platform);

        if (clientResourceVer != serverResourceVer)
        {
            logger.LogInformation(
                "Request blocked due to resource version mismatch: client: {clientVer}, server: {serverVer}",
                clientResourceVer,
                serverResourceVer
            );

            context.Result = new OkObjectResult(
                new DragaliaResponse<object>(
                    data_headers: new DataHeaders(ResultCode.CommonResourceVersionError),
                    new ResultCodeData(ResultCode.CommonResourceVersionError)
                )
            );
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
