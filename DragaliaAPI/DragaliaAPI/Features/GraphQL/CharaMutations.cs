using System.Linq.Expressions;
using System.Text.Json.Serialization;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class CharaMutations : MutationBase
{
    private readonly IUnitRepository unitRepository;
    private readonly ILogger<CharaMutations> logger;

    public CharaMutations(
        IUnitRepository unitRepository,
        ApiContext apiContext,
        IPlayerIdentityService identityService,
        ILogger<CharaMutations> logger
    )
        : base(apiContext, identityService)
    {
        this.unitRepository = unitRepository;
        this.logger = logger;
    }

    [GraphQLMutation("Reset a character for a particular player.")]
    public Expression<Func<ApiContext, DbPlayerCharaData>> ResetCharacter(
        ApiContext db,
        ResetCharacterArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(
            args.ViewerId,
            query => query.Include(x => x.CharaList)
        );

        DbPlayerCharaData? charaData = this.Player.CharaList.FirstOrDefault(x =>
            x.CharaId == args.CharaId
        );

        if (charaData is null)
            throw new ArgumentException("Player does not own specified character");

        this.logger.LogInformation("Resetting character {chara}", args.CharaId);

        this.Player.CharaList.Remove(charaData);
        this.Player.CharaList.Add(new DbPlayerCharaData(this.Player.ViewerId, args.CharaId));
        db.SaveChanges();

        return (ctx) =>
            ctx.PlayerCharaData.First(x =>
                x.ViewerId == this.Player.ViewerId && x.CharaId == args.CharaId
            );
    }

    [GraphQLArguments]
    public record ResetCharacterArgs(
        int ViewerId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] Charas CharaId
    );
}
