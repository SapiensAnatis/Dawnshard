using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class ImpersionationMutations : MutationBase
{
    private readonly ISessionService sessionService;
    private readonly ApiContext apiContext;

    public ImpersionationMutations(
        ISessionService sessionService,
        ApiContext apiContext,
        IPlayerIdentityService identityService
    )
        : base(apiContext, identityService)
    {
        this.sessionService = sessionService;
        this.apiContext = apiContext;
    }

    [GraphQLMutation]
    public async Task<Expression<Func<ApiContext, DbPlayer>>> StartImpersonation(
        long viewerId,
        long targetViewerId
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(viewerId);

        string targetAccountId = this
            .apiContext.Players.Include(x => x.UserData)
            .Where(x => x.UserData!.ViewerId == targetViewerId)
            .Select(x => x.AccountId)
            .First();

        await this.sessionService.StartUserImpersonation(targetAccountId, targetViewerId);

        return context =>
            context.Players.IgnoreQueryFilters().First(x => x.AccountId == targetAccountId);
    }

    [GraphQLMutation]
    public async Task<Expression<Func<ApiContext, DbPlayer>>> ClearImpersonation(long viewerId)
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(viewerId);

        await this.sessionService.ClearUserImpersonation();

        return context => context.Players.First(x => x.AccountId == this.Player.AccountId);
    }
}
