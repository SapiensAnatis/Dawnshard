using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Talisman;

public class TalismanService(
    IUnitRepository unitRepository,
    IRewardService rewardService,
    ILogger<TalismanService> logger
) : ITalismanService
{
    public const int TalismanCoinReward = 10000; // Constant for all talismans
    public const int TalismanMaxCount = 500;

    public async Task<DeleteDataList> SellTalismans(IEnumerable<long> talismanIds)
    {
        List<long> deletedTalismanIds = new();

        List<DbTalisman> dbTalismans = await unitRepository
            .Talismans.Where(x => talismanIds.Contains(x.TalismanKeyId))
            .ToListAsync();

        logger.LogDebug("Selling talismans {@talismanKeyIdList}", talismanIds);

        foreach (DbTalisman talisman in dbTalismans)
        {
            if (talisman.IsLock)
            {
                throw new DragaliaException(
                    ResultCode.TalismanSellLocked,
                    "Tried to sell locked talisman"
                );
            }

            unitRepository.RemoveTalisman(talisman);
            await rewardService.GrantReward(new Entity(EntityTypes.Rupies, 0, TalismanCoinReward));
            deletedTalismanIds.Add(talisman.TalismanKeyId);
        }

        return new DeleteDataList
        {
            DeleteTalismanList = deletedTalismanIds.Select(x => new AtgenDeleteTalismanList(x))
        };
    }

    public async Task SetLock(long talismanKeyId, bool locked)
    {
        DbTalisman talisman = await unitRepository.Talismans.SingleAsync(x =>
            x.TalismanKeyId == talismanKeyId
        );
        talisman.IsLock = locked;

        logger.LogDebug(
            "Changed lock state of talisman {@talismanKeyId} to {isLocked}",
            talismanKeyId,
            locked
        );
    }
}
