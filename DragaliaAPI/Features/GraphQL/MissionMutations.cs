using System.Linq.Expressions;
using System.Text.Json.Serialization;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.GraphQL;

public class MissionMutations(
    IMissionService missionService,
    IMissionInitialProgressionService missionInitialProgressionService,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : MutationBase(apiContext, playerIdentityService)
{
    [GraphQLMutation("Reset a mission to its initial progress")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> ResetMissionProgress(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        DbPlayer player = GetPlayer(args.ViewerId);
        DbPlayerMission mission = GetMission(db, player, args);

        await missionInitialProgressionService.GetInitialMissionProgress(mission);

        await db.SaveChangesAsync();

        return this.GetMissionExpression(player, args);
    }

    [GraphQLMutation("Completes a mission")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> CompleteMission(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        DbPlayer player = GetPlayer(args.ViewerId);
        DbPlayerMission mission = GetMission(db, player, args);

        if (mission.State != MissionState.InProgress)
            throw new InvalidOperationException("Mission was already completed");

        int complete = Mission.From(args.MissionType, args.MissionId).CompleteValue;

        mission.Progress = complete;
        mission.State = MissionState.Completed;

        await db.SaveChangesAsync();

        return this.GetMissionExpression(player, args);
    }

    [GraphQLMutation("Starts a mission")]
    public async Task<Expression<Func<ApiContext, DbPlayerMission>>> StartMission(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        if (args.MissionType is MissionType.Drill or MissionType.MainStory)
        {
            throw new InvalidOperationException(
                "Starting missions that are part of a group is unsupported"
            );
        }

        DbPlayer player = GetPlayer(args.ViewerId);
        DbPlayerMission mission = GetMission(db, player, args);

        if (mission != null)
            throw new InvalidOperationException("Mission is already started");

        await missionService.StartMission(args.MissionType, args.MissionId);

        await db.SaveChangesAsync();

        return this.GetMissionExpression(player, args);
    }

    [GraphQLArguments]
    public record MissionMutationArgs(
        long ViewerId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType MissionType,
        int MissionId
    );

    private Expression<Func<ApiContext, DbPlayerMission>> GetMissionExpression(
        DbPlayer player,
        MissionMutationArgs args
    ) => context => GetMission(context, player, args);

    private DbPlayerMission GetMission(
        ApiContext context,
        DbPlayer player,
        MissionMutationArgs args
    ) =>
        context.PlayerMissions.FirstOrDefault(
            x =>
                x.Id == args.MissionId
                && x.Type == args.MissionType
                && x.DeviceAccountId == player.AccountId
        ) ?? throw new InvalidOperationException("No mission found.");
}
