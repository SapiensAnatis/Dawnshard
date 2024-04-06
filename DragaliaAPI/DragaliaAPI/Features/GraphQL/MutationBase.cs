using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public abstract class MutationBase
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService identityService;

    protected DbPlayer? Player { get; private set; }

    protected MutationBase(ApiContext apiContext, IPlayerIdentityService identityService)
    {
        this.apiContext = apiContext;
        this.identityService = identityService;
    }

    [MemberNotNull(nameof(Player))]
    protected IDisposable StartUserImpersonation(
        long viewerId,
        Func<IQueryable<DbPlayer>, IQueryable<DbPlayer>>? include = null
    )
    {
        IQueryable<DbPlayer> query = this
            .apiContext.Players.IgnoreQueryFilters()
            .Where(x => x.UserData != null && x.UserData.ViewerId == viewerId);

        if (include is not null)
            query = include.Invoke(query);

        DbPlayer? player = query.FirstOrDefault();

        if (player is null)
            throw new ArgumentException($"No player found for viewer ID {viewerId}");

        this.Player = player;

        IDisposable context = this.identityService.StartUserImpersonation(
            viewerId,
            player.AccountId
        );

        return context;
    }
}
