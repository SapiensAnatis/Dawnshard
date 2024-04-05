using System.Linq.Expressions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class UserDataMutations : MutationBase
{
    private readonly ITutorialService tutorialService;
    private readonly ILogger<UserDataMutations> logger;

    public UserDataMutations(
        ApiContext apiContext,
        IPlayerIdentityService identityService,
        ITutorialService tutorialService,
        ILogger<UserDataMutations> logger
    )
        : base(apiContext, identityService)
    {
        this.tutorialService = tutorialService;
        this.logger = logger;
    }

    [GraphQLMutation("Update a player's TutorialStatus.")]
    public Expression<Func<ApiContext, DbPlayerUserData>> UpdateTutorialStatus(
        ApiContext db,
        UpdateTutorialStatusArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(
            args.ViewerId,
            query => query.Include(x => x.UserData)
        );

        if (this.Player.UserData is null)
            throw new InvalidOperationException("UserData was null!");

        this.logger.LogInformation("Setting tutorial status to {newStatus}", args.NewStatus);
        this.Player.UserData.TutorialStatus = args.NewStatus;
        db.SaveChanges();

        return (ctx) =>
            ctx.PlayerUserData.IgnoreQueryFilters().First(x => x.ViewerId == this.Player.ViewerId);
    }

    [GraphQLMutation("Add a tutorial flag to a user.")]
    public Expression<Func<ApiContext, DbPlayerUserData>> AddTutorialFlag(
        ApiContext db,
        UpdateTutorialFlagArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(
            args.ViewerId,
            query => query.Include(x => x.UserData)
        );

        if (this.Player.UserData is null)
            throw new InvalidOperationException("UserData was null!");

        this.logger.LogInformation("Adding tutorial flag {flag}", args.Flag);

        tutorialService.AddTutorialFlag(args.Flag);
        db.SaveChanges();

        return (ctx) =>
            ctx.PlayerUserData.IgnoreQueryFilters().First(x => x.ViewerId == this.Player.ViewerId);
    }

    [GraphQLMutation("Remove a tutorial flag to a user.")]
    public Expression<Func<ApiContext, DbPlayerUserData>> RemoveTutorialFlag(
        ApiContext db,
        UpdateTutorialFlagArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(
            args.ViewerId,
            query => query.Include(x => x.UserData)
        );

        if (this.Player.UserData is null)
            throw new InvalidOperationException("UserData was null!");

        this.logger.LogInformation("Removing tutorial flag {flag}", args.Flag);
        ISet<int> flagList = this.Player.UserData.TutorialFlagList;

        if (!flagList.Remove(args.Flag))
            throw new ArgumentException($"Failed to remove flag {args.Flag}");

        this.Player.UserData.TutorialFlagList = flagList;
        db.SaveChanges();

        return (ctx) =>
            ctx.PlayerUserData.IgnoreQueryFilters().First(x => x.ViewerId == this.Player.ViewerId);
    }

    [GraphQLArguments]
    public record UpdateTutorialStatusArgs(long ViewerId, int NewStatus);

    [GraphQLArguments]
    public record UpdateTutorialFlagArgs(long ViewerId, int Flag);
}
