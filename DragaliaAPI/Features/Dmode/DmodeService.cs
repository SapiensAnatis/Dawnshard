using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dmode;

public class DmodeService(
    IDmodeRepository dmodeRepository,
    IDateTimeProvider dateTimeProvider,
    IDmodeCacheService dmodeCacheService,
    ILogger<DmodeService> logger,
    IPaymentService paymentService,
    IRewardService rewardService
) : IDmodeService
{
    private const int MaxRecoveryUses = 10;
    private const int MaxSkipUses = 3;

    public async Task<DmodeInfo> GetInfo()
    {
        DbPlayerDmodeInfo? info = await dmodeRepository.Info.SingleOrDefaultAsync();
        if (info == null)
        {
            return new DmodeInfo();
        }

        int maxFloor = await dmodeRepository.GetTotalMaxFloorAsync();

        return new DmodeInfo(
            maxFloor,
            info.RecoveryCount,
            info.RecoveryTime,
            info.FloorSkipCount,
            info.FloorSkipTime,
            info.Point1Quantity,
            info.Point2Quantity,
            true
        );
    }

    public async Task<DmodeDungeonInfo> GetDungeonInfo()
    {
        DbPlayerDmodeDungeon? dungeon = await dmodeRepository.Dungeon.SingleOrDefaultAsync();
        if (dungeon == null)
        {
            return new DmodeDungeonInfo();
        }

        // Sanity checks so people dont get stuck
        if (
            dungeon.State
            is not (DungeonState.Waiting or DungeonState.WaitingSkip or DungeonState.Halting)
        )
        {
            DmodePlayRecord? record = null;
            try
            {
                if (await dmodeCacheService.DoesPlayRecordExist())
                {
                    record = await dmodeCacheService.LoadPlayRecord();
                    await dmodeCacheService.LoadFloorInfo(record.floor_key);
                }

                await dmodeCacheService.LoadIngameInfo();
            }
            catch (DragaliaException ex)
            {
                // Cache data has errors, resetting dungeon info
                logger.LogWarning(
                    "Dmode cache sanity check failed (no floor or ingame info), resetting state. Exception: {ex}",
                    ex
                );

                if (record != null)
                    await dmodeCacheService.DeleteFloorInfo(record.floor_key);

                await dmodeCacheService.DeletePlayRecord();
                await dmodeCacheService.DeleteIngameInfo();

                dungeon.Clear();
            }
        }

        return new DmodeDungeonInfo(
            dungeon.CharaId,
            dungeon.Floor,
            dungeon.QuestTime,
            dungeon.DungeonScore,
            dungeon.IsPlayEnd,
            dungeon.State
        );
    }

    public async Task<IEnumerable<DmodeServitorPassiveList>> GetServitorPassiveList()
    {
        return (await dmodeRepository.ServitorPassives.ToListAsync()).Select(
            x => new DmodeServitorPassiveList(x.PassiveId, x.Level)
        );
    }

    public async Task<DmodeExpedition> GetExpedition()
    {
        DbPlayerDmodeExpedition? expedition =
            await dmodeRepository.Expedition.SingleOrDefaultAsync();

        if (expedition == null)
        {
            return new DmodeExpedition();
        }

        return new DmodeExpedition(
            expedition.CharaId1,
            expedition.CharaId2,
            expedition.CharaId3,
            expedition.CharaId4,
            expedition.StartTime,
            expedition.TargetFloor,
            expedition.State
        );
    }

    public async Task<IEnumerable<DmodeCharaList>> GetCharaList()
    {
        return (await dmodeRepository.GetCharasAsync()).Select(
            x =>
                new DmodeCharaList(
                    x.CharaId,
                    x.MaxFloor,
                    x.SelectedServitorId,
                    x.SelectEditSkillCharaId1,
                    x.SelectEditSkillCharaId2,
                    x.SelectEditSkillCharaId3,
                    x.MaxScore
                )
        );
    }

    public async Task UseRecovery()
    {
        DbPlayerDmodeInfo info = await dmodeRepository.GetInfoAsync();
        if (info.RecoveryCount >= MaxRecoveryUses)
        {
            throw new DragaliaException(
                ResultCode.DmodeDungeonRecoverError,
                "Daily limit exceeded"
            );
        }

        info.RecoveryCount++;
        info.RecoveryTime = dateTimeProvider.UtcNow;
    }

    public async Task UseSkip()
    {
        DbPlayerDmodeInfo info = await dmodeRepository.GetInfoAsync();
        if (info.FloorSkipCount >= MaxSkipUses)
        {
            throw new DragaliaException(
                ResultCode.DmodeDungeonSkipDailyCountError,
                "Daily limit exceeded"
            );
        }

        info.FloorSkipCount++;
        info.FloorSkipTime = dateTimeProvider.UtcNow;
    }

    public async Task<IEnumerable<DmodeServitorPassiveList>> BuildupServitorPassive(
        IEnumerable<DmodeServitorPassiveList> buildupList
    )
    {
        Dictionary<DmodeServitorPassiveType, DbPlayerDmodeServitorPassive> currentPassives = (
            await dmodeRepository.ServitorPassives.ToListAsync()
        ).ToDictionary(x => x.PassiveId, x => x);

        logger.LogDebug("Building up servitors: {@servitorBuildups}", buildupList);

        foreach (DmodeServitorPassiveList passiveList in buildupList.OrderBy(x => x.passive_level))
        {
            if (!currentPassives.ContainsKey(passiveList.passive_no))
            {
                currentPassives[passiveList.passive_no] = dmodeRepository.AddServitorPassive(
                    passiveList.passive_no,
                    0
                );
            }

            DbPlayerDmodeServitorPassive passive = currentPassives[passiveList.passive_no];
            ImmutableDictionary<int, DmodeServitorPassiveLevel> levels = DmodeHelper.PassiveLevels[
                passive.PassiveId
            ];

            DmodeServitorPassiveLevel level = levels[passiveList.passive_level];
            foreach ((EntityTypes type, int id, int quantity) in level.NeededMaterials)
            {
                if (type == EntityTypes.None)
                    continue;

                await paymentService.ProcessPayment(new Entity(type, id, quantity));
            }

            passive.Level = passiveList.passive_level;
        }

        return currentPassives.Values.Select(
            x => new DmodeServitorPassiveList(x.PassiveId, x.Level)
        );
    }

    public async Task<DmodeExpedition> StartExpedition(int targetFloor, IEnumerable<Charas> charas)
    {
        logger.LogDebug(
            "Starting dmode expedition to floor {targetFloor} with characters {@charas}",
            targetFloor,
            charas
        );

        DbPlayerDmodeExpedition expedition = await dmodeRepository.GetExpeditionAsync();

        expedition.Clear();

        Charas[] charaArr = charas.ToArray();

        expedition.CharaId1 = charaArr[0];
        expedition.CharaId2 = charaArr[1];
        expedition.CharaId3 = charaArr[2];
        expedition.CharaId4 = charaArr[3];

        expedition.StartTime = dateTimeProvider.UtcNow;
        expedition.TargetFloor = targetFloor;

        expedition.State = ExpeditionState.Playing;

        return new DmodeExpedition(
            expedition.CharaId1,
            expedition.CharaId2,
            expedition.CharaId3,
            expedition.CharaId4,
            expedition.StartTime,
            expedition.TargetFloor,
            expedition.State
        );
    }

    public async Task<(
        DmodeExpedition Expedition,
        DmodeIngameResult IngameResult
    )> FinishExpedition(bool forceFinish)
    {
        DbPlayerDmodeExpedition expedition = await dmodeRepository.GetExpeditionAsync();

        expedition.State = ExpeditionState.Waiting;

        DmodeExpedition dmodeExpedition =
            new(
                expedition.CharaId1,
                expedition.CharaId2,
                expedition.CharaId3,
                expedition.CharaId4,
                expedition.StartTime,
                expedition.TargetFloor,
                expedition.State
            );

        logger.LogDebug(
            "Finishing expedition {@expedition}. Force-finished: {isForceFinish}",
            dmodeExpedition,
            forceFinish
        );

        DmodeIngameResult dmodeResult = new();
        dmodeResult.chara_id_list = new[]
        {
            expedition.CharaId1,
            expedition.CharaId2,
            expedition.CharaId3,
            expedition.CharaId4
        };

        List<AtgenRewardTalismanList> talismans = new();

        dmodeResult.reward_talisman_list = talismans;

        // No rewards for force-finishing
        if (forceFinish)
            return (dmodeExpedition, dmodeResult);

        DmodeExpeditionFloor floor = MasterAsset.DmodeExpeditionFloor[expedition.TargetFloor];
        DateTimeOffset endTime = expedition.StartTime.AddSeconds(floor.NeedTime);
        if (endTime > dateTimeProvider.UtcNow)
        {
            throw new DragaliaException(
                ResultCode.DmodeExpeditionStateError,
                "Not enough time has passed"
            );
        }

        // Dawn Amber Intake Boost works on Expedition rewards. (wiki)
        Dictionary<DmodeServitorPassiveType, int> passives = (
            await dmodeRepository.ServitorPassives.ToListAsync()
        ).ToDictionary(x => x.PassiveId, x => x.Level);

        double pointMultiplier1 = 1d;
        double pointMultiplier2 = 1d;

        if (passives.TryGetValue(DmodeServitorPassiveType.DmodePoint1, out int dmodePoint1Level))
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.DmodePoint1
            ][dmodePoint1Level];
            pointMultiplier1 += level.UpValue / 100;
        }

        if (passives.TryGetValue(DmodeServitorPassiveType.DmodePoint2, out int dmodePoint2Level))
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.DmodePoint2
            ][dmodePoint2Level];
            pointMultiplier2 += level.UpValue / 100;
        }

        dmodeResult.floor_num = floor.FloorNum;
        dmodeResult.take_dmode_point_1 = (int)
            Math.Ceiling(floor.RewardDmodePoint1 * pointMultiplier1);
        dmodeResult.take_dmode_point_2 = (int)
            Math.Ceiling(floor.RewardDmodePoint2 * pointMultiplier2);

        await rewardService.GrantReward(
            new Entity(
                EntityTypes.DmodePoint,
                (int)DmodePoint.Point1,
                dmodeResult.take_dmode_point_1
            )
        );
        await rewardService.GrantReward(
            new Entity(
                EntityTypes.DmodePoint,
                (int)DmodePoint.Point2,
                dmodeResult.take_dmode_point_2
            )
        );

        Random rdm = Random.Shared;

        foreach (Charas charaId in dmodeResult.chara_id_list.Where(x => x != Charas.Empty))
        {
            AtgenRewardTalismanList talisman = TalismanHelper.GenerateTalisman(rdm, charaId, 0);

            await rewardService.GrantTalisman(
                talisman.talisman_id,
                talisman.talisman_ability_id_1,
                talisman.talisman_ability_id_2,
                talisman.talisman_ability_id_3,
                talisman.additional_hp,
                talisman.additional_attack
            );

            talismans.Add(talisman);
        }

        dmodeResult.reward_talisman_list = talismans;

        return (dmodeExpedition, dmodeResult);
    }
}
