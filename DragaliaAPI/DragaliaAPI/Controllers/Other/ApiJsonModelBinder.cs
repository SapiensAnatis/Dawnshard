using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Controllers.Other;

public class ApiJsonModelBinder : IModelBinder
{
    private readonly BodyModelBinder bodyModelBinder;

    public ApiJsonModelBinder(
        IHttpRequestStreamReaderFactory readerFactory,
        ILoggerFactory loggerFactory,
        IOptions<MvcOptions> options
    )
    {
        JsonOptions jsonOptions = new();
        ApiJsonOptions.Action.Invoke(jsonOptions.JsonSerializerOptions);

        SystemTextJsonInputFormatter inputFormatter =
            new(jsonOptions, loggerFactory.CreateLogger<SystemTextJsonInputFormatter>());

        this.bodyModelBinder = new([inputFormatter], readerFactory, loggerFactory, options.Value);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext) =>
        this.bodyModelBinder.BindModelAsync(bindingContext);
}
