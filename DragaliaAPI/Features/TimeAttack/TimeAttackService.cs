using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.TimeAttack.Validation;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
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
    public bool GetIsRankedQuest(int questId)
    {
        return MasterAsset.RankingData.TryGetValue(questId, out _);
    }

    public IEnumerable<RankingTierReward> GetRewards()
    {
        IEnumerable<RankingTierReward> applicableRewards =
            MasterAsset.RankingTierReward.Enumerable.Where(
                x => x.GroupId == options.CurrentValue.GroupId
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

        List<DbTimeAttackClearUnit> clearUnits = entry.PartyInfo.party_unit_list
            .Select(x => MapTimeAttackUnit(x, gameId))
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
                        DeviceAccountId = playerIdentityService.AccountId,
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
        float bestClearTime = await questRepository.Quests
            .Where(x => x.QuestId == questId)
            .Select(x => x.BestClearTime)
            .FirstOrDefaultAsync();

        logger.LogDebug("Found clear time of {time} s for quest {quest}", bestClearTime, questId);

        if (bestClearTime <= 0)
            return Enumerable.Empty<RankingTierReward>();

        IEnumerable<int> receivedRewards = await timeAttackRepository.ReceivedRewards
            .Select(x => x.RewardId)
            .ToListAsync();

        IEnumerable<RankingTierReward> rewardsToReceive = this.GetRewards()
            .Where(x => x.QuestId == questId)
            .Where(x => x.ClearTimeUpper > bestClearTime)
            .ExceptBy(receivedRewards, x => x.Id)
            .ToList();

        await rewardService.GrantRewards(
            rewardsToReceive.Select(
                x =>
                    new Entity(
                        x.RankingRewardEntityType,
                        x.RankingRewardEntityId,
                        x.RankingRewardEntityQuantity
                    )
            )
        );

        timeAttackRepository.AddRewards(
            rewardsToReceive.Select(
                x =>
                    new DbReceivedRankingTierReward()
                    {
                        DeviceAccountId = playerIdentityService.AccountId,
                        QuestId = questId,
                        RewardId = x.Id
                    }
            )
        );

        return rewardsToReceive;
    }

    private DbTimeAttackClearUnit MapTimeAttackUnit(PartyUnitList x, string roomId)
    {
        DbTimeAttackClearUnit unit =
            new()
            {
                UnitNo = x.position,
                DeviceAccountId = playerIdentityService.AccountId,
                GameId = roomId
            };

        if (x.chara_data is not null)
            unit.CharaId = x.chara_data.chara_id;

        if (x.dragon_data is not null)
            unit.EquippedDragonEntityId = x.dragon_data.dragon_id;

        if (x.weapon_body_data is not null)
            unit.EquipWeaponBodyId = x.weapon_body_data.weapon_body_id;

        if (x.edit_skill_1_chara_data is not null)
            unit.EditSkill1CharaId = x.edit_skill_1_chara_data.chara_id;

        if (x.edit_skill_2_chara_data is not null)
            unit.EditSkill2CharaId = x.edit_skill_2_chara_data.chara_id;

        if (x.talisman_data is not null)
        {
            unit.EquippedTalismanEntityId = x.talisman_data.talisman_id;
            unit.TalismanAbility1 = x.talisman_data.talisman_ability_id_1;
            unit.TalismanAbility2 = x.talisman_data.talisman_ability_id_2;
        }

        List<AbilityCrests> crests = x.crest_slot_type_1_crest_list
            .Concat(x.crest_slot_type_2_crest_list)
            .Concat(x.crest_slot_type_3_crest_list)
            .Select(x => x.ability_crest_id)
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
