using System.Collections.Generic;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Talisman;

public partial class TalismanService(
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

        Log.SellingTalismans(logger, talismanIds);

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
            DeleteTalismanList = deletedTalismanIds.Select(x => new AtgenDeleteTalismanList(x)),
        };
    }

    public async Task SetLock(long talismanKeyId, bool locked)
    {
        DbTalisman talisman = await unitRepository.Talismans.SingleAsync(x =>
            x.TalismanKeyId == talismanKeyId
        );
        talisman.IsLock = locked;

        Log.ChangedLockStateOfTalismanTo(logger, talismanKeyId, locked);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Selling talismans {@talismanKeyIdList}")]
        public static partial void SellingTalismans(
            ILogger logger,
            IEnumerable<long> talismanKeyIdList
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Changed lock state of talisman {@talismanKeyId} to {isLocked}"
        )]
        public static partial void ChangedLockStateOfTalismanTo(
            ILogger logger,
            long talismanKeyId,
            bool isLocked
        );
    }
}
