using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class PresentMutations
{
    [GraphQLMutation("Give a player a present")]
    public Expression<Func<ApiContext, DbPlayer>> GivePresent(ApiContext db, GivePresentArgs args)
    {
        DbPlayer player = db.Players
            .Include(x => x.UserData)
            .First(x => x.UserData != null && x.UserData.ViewerId == args.ViewerId);

        player.Presents.Add(
            new Present.Present(
                PresentMessage.DragaliaLostTeam,
                args.EntityType,
                args.EntityId,
                args.EntityQuantity
            ).ToEntity(player.AccountId)
        );

        db.SaveChanges();

        return (ctx) => player;
    }

    [GraphQLArguments]
    public class GivePresentArgs
    {
        public int ViewerId { get; set; }
        public EntityTypes EntityType { get; set; }
        public int EntityId { get; set; }
        public int EntityQuantity { get; set; }
    }
}
