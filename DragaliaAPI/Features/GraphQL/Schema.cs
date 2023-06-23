using DragaliaAPI.Database;
using EntityGraphQL.AspNet;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public static class Schema
{
    public static IServiceCollection ConfigureGraphQlSchema(this IServiceCollection collection) =>
        collection.AddGraphQLSchema<ApiContext>(options =>
        {
            options.AutoBuildSchemaFromContext = true;
            options.PreBuildSchemaFromContext = (schema) =>
                schema.AddScalarType<TimeSpan>("TimeSpan", "time span");
            options.ConfigureSchema = (schema) =>
            {
                schema
                    .Query()
                    .AddField(
                        "player",
                        new { viewerId = ArgumentHelper.Required<long>() },
                        (ctx, args) =>
                            ctx.Players
                                .Include(x => x.UserData)
                                .Include(x => x.AbilityCrestList)
                                .Include(x => x.CharaList)
                                .Include(x => x.DragonList)
                                .Include(x => x.BuildList)
                                .Include(x => x.Presents)
                                .Include(x => x.PresentHistory)
                                .AsSplitQuery()
                                .First(
                                    x => x.UserData != null && x.UserData.ViewerId == args.viewerId
                                ),
                        "Fetch player by viewer id"
                    );

                schema.AddMutationsFrom<PresentMutations>();
            };
        });
}
