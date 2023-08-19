using System.Diagnostics;
using System.Linq;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionInitialProgressionService(
    ILogger<MissionInitialProgressionService> logger,
    IFortMissionProgressionService fortMissionProgressionService,
    IAbilityCrestRepository abilityCrestRepository,
    IQuestRepository questRepository,
    IUnitRepository unitRepository,
    IWeaponRepository weaponRepository,
    IStoryRepository storyRepository,
    IUserDataRepository userDataRepository,
    ITradeRepository tradeRepository,
    IPartyPowerRepository partyPowerRepository
) : IMissionInitialProgressionService
{
    public async Task GetInitialMissionProgress(DbPlayerMission mission)
    {
        Mission missionInfo = Mission.From(mission.Type, mission.Id);

        MissionProgressionInfo? progressionInfo =
            MasterAsset.MissionProgressionInfo.Enumerable.SingleOrDefault(
                x => x.MissionType == mission.Type && x.MissionId == mission.Id
            );

        if (progressionInfo == null)
            return;

        int amountToComplete = missionInfo.CompleteValue;
        int currentAmount = progressionInfo.CompleteType switch
        {
            MissionCompleteType.FortPlantLevelUp
                => await fortMissionProgressionService.GetMaxFortPlantLevel(
                    (FortPlants)progressionInfo.Parameter!
                ),
            MissionCompleteType.FortPlantBuilt
            or MissionCompleteType.FortPlantPlaced
                => await fortMissionProgressionService.GetFortPlantCount(
                    (FortPlants)progressionInfo.Parameter!
                ),
            MissionCompleteType.FortLevelUp
                => await fortMissionProgressionService.GetTotalFortLevel(),
            MissionCompleteType.QuestCleared
                => await GetQuestClearedCount(
                    progressionInfo.Parameter,
                    progressionInfo.Parameter2,
                    (QuestPlayModeTypes?)progressionInfo.Parameter3
                ),
            MissionCompleteType.QuestStoryCleared
                => await storyRepository.HasReadQuestStory(progressionInfo.Parameter!.Value)
                    ? 1
                    : 0,
            MissionCompleteType.EventGroupCleared
                => await GetQuestGroupClearedCount(
                    progressionInfo.Parameter,
                    (VariationTypes?)progressionInfo.Parameter2,
                    (QuestPlayModeTypes?)progressionInfo.Parameter3
                ),
            MissionCompleteType.WeaponEarned
                => await GetWeaponEarnedCount(
                    (WeaponBodies?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2,
                    progressionInfo.Parameter3,
                    (WeaponSeries?)progressionInfo.Parameter4
                ),
            MissionCompleteType.WeaponRefined
                => await GetWeaponRefinedCount(
                    (WeaponBodies?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2,
                    progressionInfo.Parameter3,
                    (WeaponSeries?)progressionInfo.Parameter4
                ),
            MissionCompleteType.AbilityCrestBuildupPlusCount
                => await GetWyrmprintBuildupCount(
                    (AbilityCrests?)progressionInfo.Parameter,
                    (PlusCountType?)progressionInfo.Parameter2
                ),
            MissionCompleteType.CharacterBuildupPlusCount
                => await GetCharacterBuildupCount(
                    (Charas?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2,
                    (PlusCountType?)progressionInfo.Parameter3
                ),
            MissionCompleteType.PlayerLevelUp
                => (await userDataRepository.GetUserDataAsync()).Level,
            MissionCompleteType.AbilityCrestTotalPlusCountUp
                => (
                    await abilityCrestRepository.AbilityCrests
                        .Where(
                            x =>
                                progressionInfo.Parameter == null
                                || x.AbilityCrestId == (AbilityCrests)progressionInfo.Parameter
                        )
                        .Select(x => new { x.AttackPlusCount, x.HpPlusCount })
                        .ToListAsync()
                )
                    .Select(x => (int?)Math.Min(x.AttackPlusCount, x.HpPlusCount))
                    .Max() ?? 0,
            MissionCompleteType.AbilityCrestLevelUp
                => (
                    await abilityCrestRepository.AbilityCrests
                        .Where(
                            x =>
                                progressionInfo.Parameter == null
                                || x.AbilityCrestId == (AbilityCrests)progressionInfo.Parameter
                        )
                        .Select(x => (int?)x.BuildupCount)
                        .ToListAsync()
                ).Sum() ?? 0,
            MissionCompleteType.CharacterLevelUp
                => await GetCharacterMaxLevel(
                    (Charas?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2
                ),
            MissionCompleteType.CharacterManaNodeUnlock
                => await GetCharacterManaNodeCount(
                    (Charas?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2
                ),
            MissionCompleteType.DragonLevelUp
                => await GetDragonMaxLevel(
                    (Dragons?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2
                ),
            MissionCompleteType.TreasureTrade
                => await GetTreasureTradeCount(
                    progressionInfo.Parameter,
                    (EntityTypes?)progressionInfo.Parameter2,
                    progressionInfo.Parameter3
                ),
            MissionCompleteType.DragonBondLevelUp
                => await GetDragonBondLevel(
                    (Dragons?)progressionInfo.Parameter,
                    (UnitElement?)progressionInfo.Parameter2
                ),
            MissionCompleteType.PartyPowerReached
                => await partyPowerRepository.GetMaxPartyPowerAsync(),
            MissionCompleteType.DragonGiftSent => 0, // Unsure about this
            MissionCompleteType.ItemSummon => 0, // TODO: As daily quests also use this, should we make it actually count the item summons?
            MissionCompleteType.AccountLinked => 0,
            MissionCompleteType.PartyOptimized => 0,
            MissionCompleteType.AbilityCrestTradeViewed => 0,
            MissionCompleteType.GuildCheckInRewardClaimed => amountToComplete, // TODO
            MissionCompleteType.UnimplementedAutoComplete => amountToComplete,
            _
                => throw new UnreachableException(
                    $"Invalid MissionProgressType {progressionInfo.CompleteType} in initial progress handling"
                )
        };

        if (currentAmount == 0)
            return;

        if (currentAmount >= amountToComplete)
        {
            mission.Progress = amountToComplete;
            mission.State = MissionState.Completed;
            logger.LogDebug(
                "Mission {missionId} ({missionType}) had all requirements met ({currentProgress}/{requiredProgress}), auto-completing.",
                mission.Id,
                mission.Type,
                currentAmount,
                amountToComplete
            );
        }
        else
        {
            mission.Progress = currentAmount;
            logger.LogDebug(
                "Mission {missionId} ({missionType}) had some requirements met ({currentProgress}/{requiredProgress}).",
                mission.Id,
                mission.Type,
                currentAmount,
                amountToComplete
            );
        }
    }

    private async Task<int> GetTreasureTradeCount(int? tradeId, EntityTypes? type, int? id)
    {
        List<int> trades = await tradeRepository.Trades
            .Where(x => x.Type == TradeType.Treasure && (tradeId == null || x.Id == tradeId))
            .Select(x => x.Id)
            .ToListAsync();

        return trades
            .Select(x => MasterAsset.TreasureTrade[x])
            .Count(
                x =>
                    (type == null || x.DestinationEntityType == type)
                    && (id == null || x.DestinationEntityId == id)
            );
    }

    private async Task<int> GetCharacterMaxLevel(Charas? charaId, UnitElement? element)
    {
        if (charaId != null)
        {
            return (await unitRepository.FindCharaAsync(charaId.Value))?.Level ?? 0;
        }

        if (element != null)
        {
            return (
                    await unitRepository.Charas
                        .Select(x => new { x.CharaId, x.Level })
                        .ToListAsync()
                )
                    .Where(x => MasterAsset.CharaData[x.CharaId].ElementalType == element)
                    .Select(x => (int?)x.Level)
                    .Max() ?? 0;
        }

        return await unitRepository.Charas.MaxAsync(x => x.Level);
    }

    private async Task<int> GetCharacterManaNodeCount(Charas? charaId, UnitElement? element)
    {
        if (charaId != null)
        {
            return (await unitRepository.FindCharaAsync(charaId.Value))?.ManaNodeUnlockCount ?? 0;
        }

        if (element != null)
        {
            return (await unitRepository.Charas.ToListAsync())
                    .Where(x => MasterAsset.CharaData[x.CharaId].ElementalType == element)
                    .Select(x => (int?)x.ManaNodeUnlockCount)
                    .Max() ?? 0;
        }

        return (await unitRepository.Charas.ToListAsync())
                .Select(x => (int?)x.ManaNodeUnlockCount)
                .Max() ?? 0;
    }

    private async Task<int> GetDragonMaxLevel(Dragons? dragonId, UnitElement? element)
    {
        if (dragonId != null)
        {
            return await unitRepository.Dragons
                    .Where(x => x.DragonId == dragonId)
                    .Select(x => (int?)x.Level)
                    .MaxAsync() ?? 0;
        }

        if (element != null)
        {
            return (
                    await unitRepository.Dragons
                        .Select(x => new { x.DragonId, x.Level })
                        .ToListAsync()
                )
                    .Where(x => MasterAsset.DragonData[x.DragonId].ElementalType == element)
                    .Select(x => (int?)x.Level)
                    .Max() ?? 0;
        }

        return await unitRepository.Dragons.MaxAsync(x => (int?)x.Level) ?? 0;
    }

    private async Task<int> GetDragonBondLevel(Dragons? dragonId, UnitElement? element)
    {
        if (dragonId != null)
        {
            DbPlayerDragonReliability? reliability =
                await unitRepository.FindDragonReliabilityAsync(dragonId.Value);

            return reliability?.Level ?? 0;
        }

        if (element != null)
        {
            return (
                    await unitRepository.DragonReliabilities
                        .Select(x => new { x.DragonId, x.Level })
                        .ToListAsync()
                )
                    .Where(x => MasterAsset.DragonData[x.DragonId].ElementalType == element)
                    .Select(x => (int?)x.Level)
                    .Max() ?? 0;
        }

        return await unitRepository.DragonReliabilities.MaxAsync(x => (int?)x.Level) ?? 0;
    }

    private async Task<int> GetQuestClearedCount(
        int? questId,
        int? questGroupId,
        QuestPlayModeTypes? playMode
    )
    {
        if (questId != null)
        {
            return await questRepository.Quests
                .Where(x => x.QuestId == questId)
                .Select(x => x.PlayCount)
                .FirstOrDefaultAsync();
        }

        List<int> validQuests = MasterAsset.QuestData.Enumerable
            .Where(
                x => x.Gid == questGroupId && (playMode == null || x.QuestPlayModeType == playMode)
            )
            .Select(x => x.Id)
            .ToList();

        return await questRepository.Quests
                .Where(x => validQuests.Contains(x.QuestId))
                .SumAsync(x => (int?)x.PlayCount) ?? 0;
    }

    private async Task<int> GetCharacterBuildupCount(
        Charas? charaId,
        UnitElement? element,
        PlusCountType? type
    )
    {
        Debug.Assert(type != null, "type != null");

        if (charaId != null)
        {
            DbPlayerCharaData? chara = await unitRepository.FindCharaAsync(charaId.Value);

            if (chara == null)
                return 0;

            return type switch
            {
                PlusCountType.Hp => chara.HpPlusCount,
                PlusCountType.Atk => chara.AttackPlusCount,
                _
                    => throw new DragaliaException(
                        ResultCode.CommonInvalidArgument,
                        $"Invalid PlusCountType for character in mission requirement, parameter: {type}"
                    )
            };
        }

        IQueryable<DbPlayerCharaData> charaQuery;

        if (element != null)
        {
            HashSet<Charas> validCharas = MasterAsset.CharaData.Enumerable
                .Where(x => x.ElementalType == element)
                .Select(x => x.Id)
                .ToHashSet();

            charaQuery = unitRepository.Charas.Where(x => validCharas.Contains(x.CharaId));
        }
        else
        {
            charaQuery = unitRepository.Charas;
        }

        return type switch
        {
            PlusCountType.Hp => await charaQuery.Select(x => (int?)x.HpPlusCount).MaxAsync() ?? 0,
            PlusCountType.Atk
                => await charaQuery.Select(x => (int?)x.AttackPlusCount).MaxAsync() ?? 0,
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    $"Invalid PlusCountType for character in mission requirement, parameter: {type}"
                )
        };
    }

    private async Task<int> GetWyrmprintBuildupCount(AbilityCrests? crestId, PlusCountType? type)
    {
        Debug.Assert(type != null, "type != null");

        if (crestId != null)
        {
            DbAbilityCrest? crest = await abilityCrestRepository.FindAsync(crestId.Value);

            if (crest == null)
                return 0;

            return type switch
            {
                PlusCountType.Hp => crest.HpPlusCount,
                PlusCountType.Atk => crest.AttackPlusCount,
                _
                    => throw new DragaliaException(
                        ResultCode.CommonInvalidArgument,
                        $"Invalid PlusCountType for wyrmprint in mission requirement, parameter: {type}"
                    ),
            };
        }

        return type switch
        {
            PlusCountType.Hp
                => await abilityCrestRepository.AbilityCrests
                    .Select(x => (int?)x.HpPlusCount)
                    .MaxAsync() ?? 0,
            PlusCountType.Atk
                => await abilityCrestRepository.AbilityCrests
                    .Select(x => (int?)x.AttackPlusCount)
                    .MaxAsync() ?? 0,
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    $"Invalid PlusCountType for wyrmprint in mission requirement, parameter: {type}"
                )
        };
    }

    private async Task<int> GetWeaponEarnedCount(
        WeaponBodies? bodyId,
        UnitElement? element,
        int? rarity,
        WeaponSeries? series
    )
    {
        if (bodyId != null)
        {
            return await weaponRepository.FindAsync(bodyId.Value) != null ? 1 : 0;
        }

        List<WeaponBodies> validWeaponBodies = MasterAsset.WeaponBody.Enumerable
            .Where(
                x =>
                    (element is null || x.ElementalType == element)
                    && (rarity is null || x.Rarity == rarity)
                    && (series is null || x.WeaponSeriesId == series)
            )
            .Select(x => x.Id)
            .ToList();

        return await weaponRepository.WeaponBodies.CountAsync(
            x => validWeaponBodies.Contains(x.WeaponBodyId)
        );
    }

    private async Task<int> GetWeaponRefinedCount(
        WeaponBodies? bodyId,
        UnitElement? element,
        int? rarity,
        WeaponSeries? series
    )
    {
        if (bodyId != null)
        {
            return (await weaponRepository.FindAsync(bodyId.Value))?.LimitOverCount ?? 0;
        }

        List<WeaponBodies> validWeaponBodies = MasterAsset.WeaponBody.Enumerable
            .Where(
                x =>
                    (element is null || x.ElementalType == element)
                    && (rarity is null || x.Rarity == rarity)
                    && (series is null || x.WeaponSeriesId == series)
            )
            .Select(x => x.Id)
            .ToList();

        return await weaponRepository.WeaponBodies
                .Where(x => validWeaponBodies.Contains(x.WeaponBodyId))
                .SumAsync(x => (int?)x.LimitOverCount) ?? 0;
    }

    private async Task<int> GetQuestGroupClearedCount(
        int? groupId,
        VariationTypes? type,
        QuestPlayModeTypes? playMode
    )
    {
        Debug.Assert(groupId != null, "groupId != null");

        HashSet<int> eventGroupPool = MasterAsset.QuestEventGroup.Enumerable
            .Where(x => x.BaseQuestGroupId == groupId)
            .Select(x => x.Id)
            .ToHashSet();

        List<int> questPool = MasterAsset.QuestData.Enumerable
            .Where(x => eventGroupPool.Contains(x.Gid))
            .Where(
                x =>
                    (type == null || x.VariationType == type)
                    && (playMode == null || x.QuestPlayModeType == playMode)
            )
            .Select(x => x.Id)
            .ToList();

        return await questRepository.Quests
                .Where(x => questPool.Contains(x.QuestId))
                .SumAsync(x => (int?)x.PlayCount) ?? 0;
    }
}
