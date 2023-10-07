using DragaliaAPI.Database;
using DragaliaAPI.Middleware;
using EntityGraphQL;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.GraphQL;

[Route("savefile/graphql")]
[Authorize(AuthenticationSchemes = SchemeName.Developer)]
public class GraphQlController : ControllerBase
{
    private readonly ApiContext apiContext;
    private readonly SchemaProvider<ApiContext> schemaProvider;

    public GraphQlController(ApiContext apiContext, SchemaProvider<ApiContext> schemaProvider)
    {
        this.apiContext = apiContext;
        this.schemaProvider = schemaProvider;
    }

    [HttpPost]
    public async Task<ActionResult<object>> Post([FromBody] QueryRequest query)
    {
        QueryResult results = await this.schemaProvider.ExecuteRequestAsync(
            query,
            HttpContext.RequestServices,
            null
        );

        return results.HasErrors() ? this.BadRequest(results) : this.Ok(results);
    }
}
