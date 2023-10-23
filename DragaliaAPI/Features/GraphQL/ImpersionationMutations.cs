using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class ImpersionationMutations : MutationBase
{
    private readonly ApiContext apiContext;

    public ImpersionationMutations(ApiContext apiContext, IPlayerIdentityService identityService)
        : base(apiContext, identityService)
    {
        this.apiContext = apiContext;
    }

    [GraphQLMutation]
    public async Task<Expression<Func<ApiContext, DbUserImpersonation>>> StartImpersonation(
        long viewerId,
        long targetViewerId
    )
    {
        DbPlayer player = this.GetPlayer(viewerId);
        string impersonatedAccountId = this.apiContext.Players
            .Include(x => x.UserData)
            .Where(x => x.UserData!.ViewerId == targetViewerId)
            .Select(x => x.AccountId)
            .First();

        this.apiContext.UserImpersonations.Add(
            new()
            {
                DeviceAccountId = player.AccountId,
                ImpersonatedDeviceAccountId = impersonatedAccountId,
                ImpersonatedViewerId = targetViewerId
            }
        );

        await this.apiContext.SaveChangesAsync();

        return context =>
            context.UserImpersonations.First(x => x.DeviceAccountId == player.AccountId);
    }

    [GraphQLMutation]
    public async Task<Expression<Func<ApiContext, DbPlayer>>> ClearImpersonation(long viewerId)
    {
        DbPlayer player = this.GetPlayer(viewerId);

        await this.apiContext.UserImpersonations
            .Where(x => x.DeviceAccountId == player.AccountId)
            .ExecuteDeleteAsync();

        return context => context.Players.First(x => x.AccountId == player.AccountId);
    }
}
