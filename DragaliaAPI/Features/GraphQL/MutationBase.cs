using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.GraphQL;

public abstract class MutationBase
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService identityService;

    protected MutationBase(ApiContext apiContext, IPlayerIdentityService identityService)
    {
        this.apiContext = apiContext;
        this.identityService = identityService;
    }

    protected DbPlayer GetPlayer(
        long viewerId,
        Func<IQueryable<DbPlayer>, IQueryable<DbPlayer>>? include = null
    )
    {
        IQueryable<DbPlayer> query = this.apiContext.Players.Where(
            x => x.UserData != null && x.UserData.ViewerId == viewerId
        );

        if (include is not null)
            query = include.Invoke(query);

        DbPlayer? player = query.FirstOrDefault();

        if (player is null)
            throw new ArgumentException($"No player found for viewer ID {viewerId}");

        this.identityService.StartUserImpersonation(player.AccountId, viewerId);

        return player;
    }
}
