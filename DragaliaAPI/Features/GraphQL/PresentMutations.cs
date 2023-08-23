using System.Linq.Expressions;
using System.Text.Json.Serialization;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ILogger = Serilog.ILogger;

namespace DragaliaAPI.Features.GraphQL;

public class PresentMutations : MutationBase
{
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

    [GraphQLMutation("Give a player a present.")]
    public Expression<Func<ApiContext, DbPlayerPresent>> GivePresent(
        ApiContext db,
        GivePresentArgs args
    )
    {
        DbPlayer player = this.GetPlayer(args.ViewerId, query => query.Include(x => x.Presents));

        DbPlayerPresent present =
            new()
            {
                DeviceAccountId = player.AccountId,
                EntityId = args.EntityId,
                EntityType = args.EntityType,
                EntityQuantity = args.EntityQuantity ?? 1,
                EntityLevel = args.EntityLevel ?? 1,
                MessageId = PresentMessage.DragaliaLostTeam,
            };

        this.logger.LogInformation("Granting present {@present}", present);
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

    [GraphQLArguments]
    public record GivePresentArgs(
        long ViewerId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EntityTypes EntityType,
        int EntityId,
        int? EntityQuantity = 1,
        int? EntityLevel = 1
    );
}
