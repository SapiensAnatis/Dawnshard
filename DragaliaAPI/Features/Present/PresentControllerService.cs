using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Present service to back <see cref="PresentController"/>.
/// </summary>
public class PresentControllerService(
    ILogger<PresentControllerService> logger,
    IPresentRepository presentRepository,
    IRewardService rewardService,
    IMapper mapper
) : IPresentControllerService
{
    private const int PresentPageSize = 100;

    public async Task<IEnumerable<PresentHistoryList>> GetPresentHistoryList(ulong presentId)
    {
        IQueryable<DbPlayerPresentHistory> presentsQuery = presentRepository.PresentHistory;

        if (presentId > 0)
        {
            presentsQuery = presentsQuery.Where(x => x.Id <= (long)presentId - PresentPageSize);
        }

        // It's a bit sus to page by ID without sorting by ID. But this is supposed to show in order of claimed date.
        // Theoretically, it should be sorted in the same way since the ID is auto-incrementing, so later date == higher ID.
        List<DbPlayerPresentHistory> list = await presentsQuery
            .OrderByDescending(x => x.Id)
            .Take(PresentPageSize)
            .ToListAsync();

        return list.Select(mapper.Map<DbPlayerPresentHistory, PresentHistoryList>)
            .OrderByDescending(x => x.id);
    }

    public async Task<IEnumerable<PresentDetailList>> GetPresentList(ulong presentId) =>
        await this.GetPresentList(presentId, false);

    public async Task<IEnumerable<PresentDetailList>> GetLimitPresentList(ulong presentId) =>
        await this.GetPresentList(presentId, true);

    private async Task<IEnumerable<PresentDetailList>> GetPresentList(ulong presentId, bool isLimit)
    {
        IQueryable<DbPlayerPresent> presentsQuery = presentRepository.Presents;

        presentsQuery = isLimit
            ? presentsQuery.Where(x => x.ReceiveLimitTime != null)
            : presentsQuery.Where(x => x.ReceiveLimitTime == null);

        if (presentId > 0)
        {
            presentsQuery = presentsQuery.Where(
                x => x.PresentId <= (long)presentId - PresentPageSize
            );
        }

        List<DbPlayerPresent> list = await presentsQuery
            .OrderByDescending(x => x.PresentId)
            .Take(PresentPageSize)
            .ToListAsync();

        return (list)
            .Select(mapper.Map<DbPlayerPresent, PresentDetailList>)
            .OrderBy(x => x.present_id);
    }

    public async Task<ClaimPresentResult> ReceivePresent(IEnumerable<ulong> ids, bool isLimit)
    {
        IQueryable<DbPlayerPresent> presentsQuery = presentRepository.Presents.Where(
            x => ids.Contains((ulong)x.PresentId)
        );

        presentsQuery = isLimit
            ? presentsQuery.Where(x => x.ReceiveLimitTime != null)
            : presentsQuery.Where(x => x.ReceiveLimitTime == null);

        List<DbPlayerPresent> presents = await presentsQuery.ToListAsync();

        List<long> receivedIds = new();
        List<long> notReceivedIds = new();
        List<long> removedIds = new();

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
            presentRepository.AddPlayerPresentHistory(
                mapper.Map<DbPlayerPresent, DbPlayerPresentHistory>(present)
            );
        }

        await presentRepository.DeletePlayerPresents(receivedIds.Concat(removedIds));

        return new(receivedIds, notReceivedIds, removedIds);
    }

    public record ClaimPresentResult(List<long> Received, List<long> Converted, List<long> Removed);
}
