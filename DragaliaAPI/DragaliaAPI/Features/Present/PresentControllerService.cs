using System.Linq.Expressions;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Present service to back <see cref="PresentController"/>.
/// </summary>
public class PresentControllerService(
    IRewardService rewardService,
    ApiContext apiContext,
    ILogger<PresentControllerService> logger
) : IPresentControllerService
{
    private const int PresentPageSize = 100;

    public async Task<IEnumerable<PresentDetailList>> GetPresentList(ulong presentId) =>
        await this.GetPresentList(presentId, false);

    public async Task<IEnumerable<PresentDetailList>> GetLimitPresentList(ulong presentId) =>
        await this.GetPresentList(presentId, true);

    private async Task<IEnumerable<PresentDetailList>> GetPresentList(ulong presentId, bool isLimit)
    {
        IQueryable<DbPlayerPresent> presentsQuery = apiContext.PlayerPresents;

        presentsQuery = isLimit
            ? presentsQuery.Where(x => x.ReceiveLimitTime != null)
            : presentsQuery.Where(x => x.ReceiveLimitTime == null);

        if (presentId > 0)
        {
            // The client uses keyset pagination for the present list, and will send the present ID of the last present
            // that it viewed if it knows there are more than 100 presents.
            presentsQuery = presentsQuery.Where(x => (ulong)x.PresentId < presentId);
        }

        List<PresentDetailList> list = await presentsQuery
            .OrderByDescending(x => x.PresentId)
            .Take(PresentPageSize)
            .ProjectToPresentDetailList()
            .ToListAsync();

        return list;
    }

    public async Task<IEnumerable<PresentHistoryList>> GetPresentHistoryList(ulong presentId)
    {
        IQueryable<DbPlayerPresentHistory> presentsQuery = apiContext.PlayerPresentHistory;

        if (presentId > 0)
        {
            presentsQuery = presentsQuery.Where(x => (ulong)x.Id < presentId);
        }

        List<PresentHistoryList> list = await presentsQuery
            .OrderByDescending(x => x.Id)
            .Take(PresentPageSize)
            .ProjectToPresentHistoryList()
            .ToListAsync();

        return list;
    }

    public async Task<ClaimPresentResult> ReceivePresent(IEnumerable<ulong> ids, bool isLimit)
    {
        IQueryable<DbPlayerPresent> presentsQuery = apiContext.PlayerPresents.Where(x =>
            ids.Contains((ulong)x.PresentId)
        );

        presentsQuery = isLimit
            ? presentsQuery.Where(x => x.ReceiveLimitTime != null)
            : presentsQuery.Where(x => x.ReceiveLimitTime == null);

        List<DbPlayerPresent> presents = await presentsQuery.ToListAsync();

        List<long> receivedIds = [];
        List<long> notReceivedIds = [];
        List<long> removedIds = [];

        foreach (DbPlayerPresent present in presents)
        {
            RewardGrantResult result = await rewardService.GrantReward(
                new(
                    present.EntityType,
                    present.EntityId,
                    present.EntityQuantity,
                    present.EntityLimitBreakCount,
                    0,
                    1
                )
            );

            switch (result)
            {
                case RewardGrantResult.Added:
                    receivedIds.Add(present.PresentId);
                    break;
                case RewardGrantResult.Converted:
                    receivedIds.Add(present.PresentId);
                    break;
                case RewardGrantResult.Limit:
                    notReceivedIds.Add(present.PresentId);
                    break;
                case RewardGrantResult.Discarded:
                    removedIds.Add(present.PresentId);
                    break;
            }

            logger.LogDebug("Claimed present {@present}", present);

            if (result is RewardGrantResult.Added or RewardGrantResult.Converted)
            {
                apiContext.PlayerPresents.Remove(present);
                apiContext.PlayerPresentHistory.Add(present.MapToPresentHistory());
            }
        }

        return new(receivedIds, notReceivedIds, removedIds);
    }

    public record ClaimPresentResult(List<long> Received, List<long> Converted, List<long> Removed);
}

file static class MappingExtensions
{
    public static IQueryable<PresentDetailList> ProjectToPresentDetailList(
        this IQueryable<DbPlayerPresent> presents
    ) =>
        presents.Select(x => new PresentDetailList()
        {
            PresentId = (ulong)x.PresentId,
            MasterId = (int)x.MasterId,
            State = (int)x.State,
            EntityType = x.EntityType,
            EntityId = x.EntityId,
            EntityQuantity = x.EntityQuantity,
            EntityLevel = x.EntityLevel,
            EntityLimitBreakCount = x.EntityLimitBreakCount,
            EntityStatusPlusCount = x.EntityStatusPlusCount,
            MessageId = x.MessageId,
            MessageParamValue1 = x.MessageParamValue1,
            MessageParamValue2 = x.MessageParamValue2,
            MessageParamValue3 = x.MessageParamValue3,
            MessageParamValue4 = x.MessageParamValue4,
            ReceiveLimitTime = x.ReceiveLimitTime ?? DateTimeOffset.UnixEpoch,
            CreateTime = x.CreateTime,
        });

    public static IQueryable<PresentHistoryList> ProjectToPresentHistoryList(
        this IQueryable<DbPlayerPresentHistory> presentHistories
    ) =>
        presentHistories.Select(x => new PresentHistoryList()
        {
            Id = (ulong)x.Id,
            EntityType = x.EntityType,
            EntityId = x.EntityId,
            EntityQuantity = x.EntityQuantity,
            EntityLevel = x.EntityLevel,
            EntityLimitBreakCount = x.EntityLimitBreakCount,
            EntityStatusPlusCount = x.EntityStatusPlusCount,
            MessageId = x.MessageId,
            MessageParamValue1 = x.MessageParamValue1,
            MessageParamValue2 = x.MessageParamValue2,
            MessageParamValue3 = x.MessageParamValue3,
            MessageParamValue4 = x.MessageParamValue4,
            CreateTime = x.CreateTime,
        });

    public static DbPlayerPresentHistory MapToPresentHistory(this DbPlayerPresent present) =>
        new()
        {
            Id = present.PresentId,
            EntityType = present.EntityType,
            EntityId = present.EntityId,
            EntityQuantity = present.EntityQuantity,
            EntityLevel = present.EntityLevel,
            EntityLimitBreakCount = present.EntityLimitBreakCount,
            EntityStatusPlusCount = present.EntityStatusPlusCount,
            MessageId = present.MessageId,
            MessageParamValue1 = present.MessageParamValue1,
            MessageParamValue2 = present.MessageParamValue2,
            MessageParamValue3 = present.MessageParamValue3,
            MessageParamValue4 = present.MessageParamValue4,
            CreateTime = present.CreateTime,
            Owner = present.Owner,
            ViewerId = present.ViewerId,
        };
}
