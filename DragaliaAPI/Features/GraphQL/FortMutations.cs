using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class FortMutations : MutationBase
{
    private readonly ILogger<FortMutations> logger;

    public FortMutations(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<FortMutations> logger
    )
        : base(apiContext, playerIdentityService)
    {
        this.logger = logger;
    }

    [GraphQLMutation("Add a building to a player's storage")]
    public Expression<Func<ApiContext, DbFortBuild>> GiveBuild(
        ApiContext db,
        long viewerId,
        FortPlants plantId,
        int level
    )
    {
        DbPlayer player = this.GetPlayer(viewerId, query => query.Include(x => x.BuildList));
        this.logger.LogInformation("Adding build {plantId} at level {level}", plantId, level);

        DbFortBuild newBuild =
            new()
            {
                DeviceAccountId = player.AccountId,
                PlantId = plantId,
                Level = level,
                PositionX = -1,
                PositionZ = -1
            };
        player.BuildList.Add(newBuild);

        db.SaveChanges();

        return (ctx) => ctx.PlayerFortBuilds.First(x => x.BuildId == newBuild.BuildId);
    }
}
