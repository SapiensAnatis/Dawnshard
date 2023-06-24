using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace DragaliaAPI.Features.GraphQL;

public class PresentMutations : MutationBase
{
    private readonly IPresentRepository presentRepository;
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<PresentMutations> logger;

    public PresentMutations(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<PresentMutations> logger
    )
        : base(apiContext, playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    [GraphQLMutation("Give a player a present")]
    public Expression<Func<ApiContext, DbPlayerPresent>> GivePresent(
        ApiContext db,
        long viewerId,
        EntityTypes entityType,
        int entityId,
        int entityQuantity
    )
    {
        DbPlayer player = this.GetPlayer(viewerId, query => query.Include(x => x.Presents));

        DbPlayerPresent present =
            new()
            {
                DeviceAccountId = player.AccountId,
                EntityId = entityId,
                EntityType = entityType,
                MessageId = PresentMessage.DragaliaLostTeam,
            };

        player.Presents.Add(present);
        db.SaveChanges();

        return (ctx) => ctx.PlayerPresents.First(x => x.PresentId == present.PresentId);
    }

    [GraphQLMutation("Clear a player's presents")]
    public Expression<Func<ApiContext, DbPlayer>> ClearPresents(ApiContext db, long viewerId)
    {
        DbPlayer player = this.GetPlayer(viewerId, query => query.Include(x => x.Presents));

        this.logger.LogInformation("Clearing all player presents");

        player.Presents.Clear();
        db.SaveChanges();

        return (ctx) => ctx.Players.First(x => x.AccountId == player.AccountId);
    }
}
