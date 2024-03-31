using System.Text.Json.Serialization.Metadata;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DragaliaAPI.Controllers.Other;

public class ApiJsonOutputAttribute : ResultFilterAttribute
{
    static ApiJsonOutputAttribute()
    {
        JsonSerializerOptions jsonOptions =
            new(ApiJsonOptions.Instance) { TypeInfoResolver = new DefaultJsonTypeInfoResolver() };

        OutputFormatter = new(jsonOptions);
    }

    private static readonly SystemTextJsonOutputFormatter OutputFormatter;

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            objectResult.Formatters.RemoveType<SystemTextJsonOutputFormatter>();
            objectResult.Formatters.Add(OutputFormatter);
        }
    }
}
