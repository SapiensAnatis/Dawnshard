using System.Linq.Expressions;
using System.Text.Json.Serialization;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Missions.InitialProgress;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class MissionMutations : MutationBase
{
    private readonly IMissionService missionService;
    private readonly IMissionInitialProgressionService missionInitialProgressionService;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<MissionMutations> logger;

    public MissionMutations(
        IMissionService missionService,
        IMissionInitialProgressionService missionInitialProgressionService,
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<MissionMutations> logger
    )
        : base(apiContext, playerIdentityService)
    {
        this.missionService = missionService;
        this.missionInitialProgressionService = missionInitialProgressionService;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    [GraphQLMutation("Reset a mission to its initial progress")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> ResetMissionProgress(
        ApiContext db,
        MissionPlayerMutationArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(args.ViewerId);
        DbPlayerMission mission = GetMission(db, this.Player, args);

        await this.missionInitialProgressionService.GetInitialMissionProgress(mission);

        await db.SaveChangesAsync();

        return GetMissionExpression(this.Player, args);
    }

    [GraphQLMutation("Completes a mission")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> CompleteMission(
        ApiContext db,
        MissionPlayerMutationArgs args
    )
    {
        using IDisposable userImpersonation = this.StartUserImpersonation(args.ViewerId);
        DbPlayerMission mission = GetMission(db, this.Player, args);

        if (mission.State != MissionState.InProgress)
            throw new InvalidOperationException("Mission was already completed");

        int complete = Mission.From(args.MissionType, args.MissionId).CompleteValue;

        mission.Progress = complete;
        mission.State = MissionState.Completed;

        await db.SaveChangesAsync();

        return GetMissionExpression(this.Player, args);
    }

    [GraphQLMutation("Starts a mission")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> StartMission(
        ApiContext db,
        MissionPlayerMutationArgs args
    )
    {
        if (args.MissionType is MissionType.Drill or MissionType.MainStory)
        {
            throw new InvalidOperationException(
                "Starting missions that are part of a group is unsupported"
            );
        }

        using IDisposable userImpersonation = this.StartUserImpersonation(args.ViewerId);
        DbPlayerMission mission = GetMission(db, this.Player, args);

        if (mission != null)
            throw new InvalidOperationException("Mission is already started");

        await this.missionService.StartMission(args.MissionType, args.MissionId);

        await db.SaveChangesAsync();

        return GetMissionExpression(this.Player, args);
    }

    [GraphQLMutation("Reset a mission's progress for all players")]
    public async Task<Expression<Func<ApiContext, IEnumerable<DbPlayerMission>>>> ResetAllProgress(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        List<DbPlayerMission> affectedMissions = await db
            .PlayerMissions.Where(x =>
                x.Id == args.MissionId
                && x.Type == args.MissionType
                && x.State == MissionState.InProgress
            )
            .ToListAsync();

        long[] players = affectedMissions.Select(x => x.ViewerId).ToArray();

        foreach (DbPlayerMission mission in affectedMissions)
        {
            this.logger.LogInformation(
                "Recalculating progress for player {viewerId}",
                mission.ViewerId
            );

            using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
                viewer: mission.ViewerId
            );

            await this.missionInitialProgressionService.GetInitialMissionProgress(mission);
        }

        await db.SaveChangesAsync();

        return context =>
            context.PlayerMissions.Where(x =>
                players.Contains(x.ViewerId) && x.Id == args.MissionId && x.Type == args.MissionType
            );
    }

    [GraphQLArguments]
    public record MissionMutationArgs(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType MissionType,
        int MissionId
    );

    [GraphQLArguments]
    public record MissionPlayerMutationArgs(long ViewerId, MissionType MissionType, int MissionId)
        : MissionMutationArgs(MissionType, MissionId);

    private static Expression<Func<ApiContext, DbPlayerMission>> GetMissionExpression(
        DbPlayer player,
        MissionMutationArgs args
    ) => context => GetMission(context, player, args);

    private static DbPlayerMission GetMission(
        ApiContext context,
        DbPlayer player,
        MissionMutationArgs args
    ) =>
        context.PlayerMissions.FirstOrDefault(x =>
            x.Id == args.MissionId && x.Type == args.MissionType && x.ViewerId == player.ViewerId
        ) ?? throw new InvalidOperationException("No mission found.");
}
