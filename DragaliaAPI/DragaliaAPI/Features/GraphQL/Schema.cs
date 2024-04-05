using DragaliaAPI.Database;
using EntityGraphQL.AspNet;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public static class Schema
{
    public static IServiceCollection ConfigureGraphQLSchema(this IServiceCollection collection) =>
        collection.AddGraphQLSchema<ApiContext>(options =>
        {
            options.AutoBuildSchemaFromContext = true;
            options.PreBuildSchemaFromContext = (schema) =>
            {
                schema.AddScalarType<TimeSpan>("TimeSpan", "time span");
                schema.AddScalarType<DateOnly>("DateOnly", "date only");
            };
            options.ConfigureSchema = (schema) =>
            {
                schema
                    .Query()
                    .AddField(
                        "player",
                        new { viewerId = ArgumentHelper.Required<long>() },
                        (ctx, args) =>
                            ctx
                                .Players.IgnoreQueryFilters()
                                .Include(x => x.UserData)
                                .Include(x => x.AbilityCrestList)
                                .Include(x => x.TalismanList)
                                .Include(x => x.StoryStates)
                                .Include(x => x.QuestList)
                                .Include(x => x.CharaList)
                                .Include(x => x.DragonList)
                                .Include(x => x.BuildList)
                                .Include(x => x.FortDetail)
                                .Include(x => x.Presents)
                                .Include(x => x.MaterialList)
                                .Include(x => x.WeaponBodyList)
                                .Include(x => x.WeaponSkinList)
                                .Include(x => x.EquippedStampList)
                                .Include(x => x.ShopInfo)
                                .Include(x => x.DmodeCharas)
                                .Include(x => x.DmodeDungeon)
                                .Include(x => x.DmodeExpedition)
                                .Include(x => x.DmodeInfo)
                                .Include(x => x.DmodeServitorPassives)
                                .Include(x => x.QuestEvents)
                                .Include(x => x.PartyPower)
                                .AsSplitQuery()
                                .First(x =>
                                    x.UserData != null && x.UserData.ViewerId == args.viewerId
                                ),
                        "Fetch player by viewer id"
                    );

                schema.AddMutationsFrom<MutationBase>();
            };
        });
}
