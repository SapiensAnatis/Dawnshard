using DragaliaAPI.Features.Shared.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Dragalipatch;

public static class WebApplicationExtensions
{
    public static WebApplication MapDragalipatchConfigEndpoint(this WebApplication app)
    {
        app.MapGet(
            "/dragalipatch/config",
            ([FromServices] IOptionsMonitor<DragalipatchOptions> patchOptions) =>
                new DragalipatchResponse(patchOptions.CurrentValue)
        );

        return app;
    }
}
