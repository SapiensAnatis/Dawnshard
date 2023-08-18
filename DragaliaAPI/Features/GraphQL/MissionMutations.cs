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
    ILogger<MissionMutations> logger,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : MutationBase(apiContext, playerIdentityService)
{
    [GraphQLMutation("Reset a mission to its initial progress")]
    public Expression<Func<ApiContext, DbPlayerMission>> ResetMissionProgress(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        DbPlayer player = GetPlayer(args.ViewerId);

        DbPlayerMission mission =
            db.PlayerMissions.Find(player.AccountId, args.MissionType, args.MissionId)
            ?? throw new InvalidOperationException("Mission not found");

        Task task = missionInitialProgressionService.GetInitialMissionProgress(mission);
        task.Wait();

        db.SaveChanges();

        return ctx => ctx.PlayerMissions.Find(player.AccountId, args.MissionType, args.MissionId)!;
    }

    [GraphQLMutation("Completes a mission")]
    public Expression<Func<ApiContext, DbPlayerMission>> CompleteMission(
        ApiContext db,
        MissionMutationArgs args
    )
    {
        DbPlayer player = GetPlayer(args.ViewerId);

        DbPlayerMission mission =
            db.PlayerMissions.Find(player.AccountId, args.MissionType, args.MissionId)
            ?? throw new InvalidOperationException("Mission not found");

        if (mission.State != MissionState.InProgress)
            throw new InvalidOperationException("Mission was already completed");

        int complete = Mission.From(args.MissionType, args.MissionId).CompleteValue;

        mission.Progress = complete;
        mission.State = MissionState.Completed;

        db.SaveChanges();

        return ctx => ctx.PlayerMissions.Find(player.AccountId, args.MissionType, args.MissionId)!;
    }

    [GraphQLMutation("Starts a mission")]
    public Expression<Func<ApiContext, DbPlayerMission>> StartMission(
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

        DbPlayerMission? mission = db.PlayerMissions.Find(
            player.AccountId,
            args.MissionType,
            args.MissionId
        );

        if (mission != null)
            throw new InvalidOperationException("Mission is already started");

        Task task = missionService.StartMission(args.MissionType, args.MissionId);
        task.Wait();

        db.SaveChanges();

        return ctx => ctx.PlayerMissions.Find(player.AccountId, args.MissionType, args.MissionId)!;
    }

    [GraphQLArguments]
    public record MissionMutationArgs(
        long ViewerId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] MissionType MissionType,
        int MissionId
    );
}
