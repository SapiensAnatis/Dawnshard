using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.TimeAttack.Validation;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;
using DragaliaAPI.Shared.PlayerDetails;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.TimeAttack;

public class TimeAttackService(
    ITimeAttackCacheService timeAttackCacheService,
    ITimeAttackRepository timeAttackRepository,
    IOptionsMonitor<TimeAttackOptions> options,
    IQuestRepository questRepository,
    IRewardService rewardService,
    IPlayerIdentityService playerIdentityService,
    ILogger<TimeAttackService> logger
) : ITimeAttackService
{
    public bool GetIsRankedQuest(int questId) =>
        MasterAsset.RankingData.TryGetValue(questId, out RankingData? rankingData)
        && rankingData.GroupId == options.CurrentValue.GroupId;

    public IEnumerable<RankingTierReward> GetRewards()
    {
        IEnumerable<RankingTierReward> applicableRewards =
            MasterAsset.RankingTierReward.Enumerable.Where(x =>
                x.GroupId == options.CurrentValue.GroupId
            );

        return applicableRewards;
    }

    public async Task<bool> SetupRankedClear(int questId, PartyInfo partyInfo)
    {
        PartyInfoValidator validator = new(questId);
        ValidationResult result = validator.Validate(partyInfo);

        if (!result.IsValid)
        {
            logger.LogInformation("Time attack validation failed: {@result}", result);
            return false;
        }

        await timeAttackCacheService.Set(questId, partyInfo);

        return true;
    }

    public async Task RegisterRankedClear(string gameId, float clearTime)
    {
        if (await timeAttackCacheService.Get() is not { } entry)
        {
            logger.LogWarning("Unable to retrieve cache entry for time attack clear");
            return;
        }

        List<DbTimeAttackClearUnit> clearUnits = entry
            .PartyInfo.PartyUnitList.Select(x => MapTimeAttackUnit(x, gameId))
            .ToList();

        await timeAttackRepository.CreateOrUpdateClear(
            new DbTimeAttackClear()
            {
                GameId = gameId,
                QuestId = entry.QuestId,
                Time = clearTime,
                Players = new()
                {
                    new()
                    {
                        GameId = gameId,
                        ViewerId = playerIdentityService.ViewerId,
                        PartyInfo = JsonSerializer.Serialize(entry.PartyInfo),
                        Units = clearUnits
                    },
                }
            }
        );

        logger.LogDebug(
            "Registered time attack clear for room {room} and quest {questId}",
            gameId,
            entry.QuestId
        );
    }

    public async Task<IEnumerable<RankingTierReward>> ReceiveTierReward(int questId)
    {
        float bestClearTime = await questRepository
            .Quests.Where(x => x.QuestId == questId)
            .Select(x => x.BestClearTime)
            .FirstOrDefaultAsync();

        logger.LogDebug("Found clear time of {time} s for quest {quest}", bestClearTime, questId);

        if (bestClearTime <= 0)
            return Enumerable.Empty<RankingTierReward>();

        IEnumerable<int> receivedRewards = await timeAttackRepository
            .ReceivedRewards.Select(x => x.RewardId)
            .ToListAsync();

        IEnumerable<RankingTierReward> rewardsToReceive = this.GetRewards()
            .Where(x => x.QuestId == questId)
            .Where(x => x.ClearTimeUpper > bestClearTime)
            .ExceptBy(receivedRewards, x => x.Id)
            .ToList();

        await rewardService.GrantRewards(
            rewardsToReceive.Select(x => new Entity(
                x.RankingRewardEntityType,
                x.RankingRewardEntityId,
                x.RankingRewardEntityQuantity
            ))
        );

        timeAttackRepository.AddRewards(
            rewardsToReceive.Select(x => new DbReceivedRankingTierReward()
            {
                ViewerId = playerIdentityService.ViewerId,
                QuestId = questId,
                RewardId = x.Id
            })
        );

        return rewardsToReceive;
    }

    private DbTimeAttackClearUnit MapTimeAttackUnit(PartyUnitList x, string roomId)
    {
        DbTimeAttackClearUnit unit =
            new()
            {
                UnitNo = x.Position,
                ViewerId = playerIdentityService.ViewerId,
                GameId = roomId
            };

        if (x.CharaData is not null)
            unit.CharaId = x.CharaData.CharaId;

        if (x.DragonData is not null)
            unit.EquippedDragonEntityId = x.DragonData.DragonId;

        if (x.WeaponBodyData is not null)
            unit.EquipWeaponBodyId = x.WeaponBodyData.WeaponBodyId;

        if (x.EditSkill1CharaData is not null)
            unit.EditSkill1CharaId = x.EditSkill1CharaData.CharaId;

        if (x.EditSkill2CharaData is not null)
            unit.EditSkill2CharaId = x.EditSkill2CharaData.CharaId;

        if (x.TalismanData is not null)
        {
            unit.EquippedTalismanEntityId = x.TalismanData.TalismanId;
            unit.TalismanAbility1 = x.TalismanData.TalismanAbilityId1;
            unit.TalismanAbility2 = x.TalismanData.TalismanAbilityId2;
        }

        List<AbilityCrests> crests = x
            .CrestSlotType1CrestList.Concat(x.CrestSlotType2CrestList)
            .Concat(x.CrestSlotType3CrestList)
            .Select(x => x.AbilityCrestId)
            .ToList();

        unit.EquipCrestSlotType1CrestId1 = crests.ElementAtOrDefault(0);
        unit.EquipCrestSlotType1CrestId2 = crests.ElementAtOrDefault(1);
        unit.EquipCrestSlotType1CrestId3 = crests.ElementAtOrDefault(2);

        unit.EquipCrestSlotType2CrestId1 = crests.ElementAtOrDefault(3);
        unit.EquipCrestSlotType2CrestId2 = crests.ElementAtOrDefault(4);

        unit.EquipCrestSlotType3CrestId1 = crests.ElementAtOrDefault(5);
        unit.EquipCrestSlotType3CrestId2 = crests.ElementAtOrDefault(6);

        return unit;
    }
}
