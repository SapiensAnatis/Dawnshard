using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Enemy;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

public class QuestCompletionService(
    IRewardService rewardService,
    IMissionProgressionService missionProgressionService,
    ILogger<QuestCompletionService> logger,
    IUnitRepository unitRepository
) : IQuestCompletionService
{
    public async Task<(
        IEnumerable<AtgenScoringEnemyPointList> Enemies,
        int Points
    )> CompleteEnemyScoreMissions(DungeonSession session, PlayRecord record)
    {
        int scoringEnemyGroupId = int.Parse($"{session.QuestGid}01");

        if (IsEarnTicketQuest(session.QuestId))
            scoringEnemyGroupId++;

        Dictionary<int, QuestScoringEnemy> scoringEnemies = MasterAsset
            .QuestScoringEnemy.Enumerable.Where(x => x.ScoringEnemyGroupId == scoringEnemyGroupId)
            .ToDictionary(x => x.EnemyListId, x => x);

        if (scoringEnemies.Count == 0)
            return (Enumerable.Empty<AtgenScoringEnemyPointList>(), 0);

        // Assume all invasion events are single-area
        AtgenTreasureRecord treasureRecord = record.TreasureRecord.First();
        IEnumerable<AtgenEnemy> enemyList = session.EnemyList.GetValueOrDefault(
            treasureRecord.AreaIdx,
            []
        );

        IEnumerable<(int EnemyParam, int Count)> killedParamIds = enemyList
            .Zip(treasureRecord.EnemySmash)
            .Select(x => (param_id: x.First.ParamId, count: x.Second.Count));

        int totalPoints = 0;
        int totalEnemies = 0;
        Dictionary<int, AtgenScoringEnemyPointList> results = new();

        foreach ((int paramId, int count) in killedParamIds)
        {
            int enemyListId = GetEnemyListId(paramId);

            if (!scoringEnemies.TryGetValue(enemyListId, out QuestScoringEnemy? questScoringEnemy))
                continue;

            if (!results.TryGetValue(questScoringEnemy.Id, out AtgenScoringEnemyPointList? value))
            {
                value = new() { ScoringEnemyId = questScoringEnemy.Id, };
                results.Add(questScoringEnemy.Id, value);
            }

            int pointsGained = questScoringEnemy.Point * count;

            totalPoints += pointsGained;
            totalEnemies += count;

            value.Point += pointsGained;
            value.SmashCount += count;
        }

        int eventPointsId = int.Parse($"{session.QuestGid}01");

        await rewardService.GrantReward(
            new Entity(EntityTypes.EarnEventItem, eventPointsId, totalPoints)
        );

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.EarnEnemiesKilled,
            totalEnemies,
            totalEnemies,
            session.QuestGid
        );

        logger.LogDebug("Calculated enemy scoring {points}", totalPoints);

        return (results.Values, totalPoints);
    }

    public async Task<(
        IEnumerable<AtgenScoreMissionSuccessList> Missions,
        int Points,
        int BoostedPoints
    )> CompleteQuestScoreMissions(
        DungeonSession session,
        PlayRecord record,
        double abilityMultiplier
    )
    {
        List<AtgenScoreMissionSuccessList> missions = new();

        int questId = session.QuestId;

        if (
            !MasterAsset.QuestScoreMissionRewardInfo.TryGetValue(
                questId,
                out QuestScoreMissionRewardInfo? info
            )
        )
        {
            return (missions, 0, 0);
        }

        double multiplier = 100.0f;

        foreach (QuestMissionCondition condition in info.RewardConditions)
        {
            // The type is called AtgenScoreMission*Success*List, but a mission with correction_value == 0 means it was not completed (so greyed out)
            int percentageAdded = await IsQuestMissionCompleted(
                condition.QuestCompleteType,
                condition.QuestCompleteValue,
                record,
                session
            )
                ? condition.PercentageAdded
                : 0;

            multiplier += percentageAdded;
            missions.Add(
                new AtgenScoreMissionSuccessList(
                    condition.QuestCompleteType,
                    condition.QuestCompleteValue,
                    percentageAdded
                )
            );
        }

        logger.LogDebug("Completed mission score missions {@missions}", missions);

        int questScoreMissionId = MasterAsset.QuestRewardData[questId].QuestScoreMissionId;
        int baseQuantity = MasterAsset
            .QuestScoreMissionData[questScoreMissionId]
            .GetScore(record.Wave, session.QuestVariation);

        double obtainedAmount = baseQuantity * (multiplier / 100.0f);
        int rewardQuantity = (int)Math.Floor(obtainedAmount);

        EventKindType eventType = MasterAsset
            .EventData[MasterAsset.QuestData[questId].Gid]
            .EventKindType;

        await rewardService.GrantReward(
            new Entity(eventType.ToItemType(), (int)info.RewardEntityId, rewardQuantity)
        );

        int boostPoints = 0;

        if (abilityMultiplier > 1)
        {
            boostPoints = (int)Math.Floor(rewardQuantity * (abilityMultiplier - 1));

            logger.LogDebug(
                "Rewarding {boostPointQuantity} extra points due to abilities",
                boostPoints
            );

            await rewardService.GrantReward(
                new Entity(eventType.ToItemType(), (int)info.RewardEntityId, boostPoints)
            );
        }

        return (missions, rewardQuantity, boostPoints);
    }

    public async Task<QuestMissionStatus> CompleteQuestMissions(
        DungeonSession session,
        bool[] currentState,
        PlayRecord record
    )
    {
        List<AtgenMissionsClearSet> clearSet = new();

        bool[] newState = { currentState[0], currentState[1], currentState[2] };

        QuestRewardData rewardData = MasterAsset.QuestRewardData[session.QuestId];
        for (int i = 0; i < 3; i++)
        {
            (QuestCompleteType type, int value) = rewardData.Missions[i];

            if (!currentState[i] && await IsQuestMissionCompleted(type, value, record, session))
            {
                newState[i] = true;
                (EntityTypes entity, int id, int quantity) = rewardData.Entities[i];
                await rewardService.GrantReward(new Entity(entity, id, quantity));
                logger.LogDebug("Completed quest mission {missionId}", i);
                clearSet.Add(new AtgenMissionsClearSet(id, entity, quantity, i + 1));
            }
        }

        List<AtgenFirstClearSet> completeSet = new();

        if (currentState.Any(x => !x) && newState.All(x => x))
        {
            await rewardService.GrantReward(
                new Entity(
                    rewardData.MissionCompleteEntityType,
                    rewardData.MissionCompleteEntityId,
                    rewardData.MissionCompleteEntityQuantity
                )
            );
            logger.LogDebug("Granting bonus for completing all missions");
            completeSet.Add(
                new AtgenFirstClearSet(
                    rewardData.MissionCompleteEntityId,
                    rewardData.MissionCompleteEntityType,
                    rewardData.MissionCompleteEntityQuantity
                )
            );
        }

        return new(newState, clearSet, completeSet);
    }

    public async Task<bool> IsQuestMissionCompleted(
        QuestCompleteType type,
        int completionValue,
        PlayRecord record,
        DungeonSession session
    )
    {
        IEnumerable<PartySettingList> party = session.Party;

        return type switch
        {
            QuestCompleteType.None => false,
            QuestCompleteType.LimitFall => record.DownCount <= completionValue,
            QuestCompleteType.DefeatAllEnemies
                => record.TreasureRecord.All(x =>
                    x.Enemy == null
                    || !session.EnemyList[x.AreaIdx].Select(y => y.EnemyIdx).Except(x.Enemy).Any()
                ), // (Maybe)TODO
            QuestCompleteType.MaxTeamSize => party.Count() <= completionValue,
            QuestCompleteType.AdventurerElementRequired
                => party.All(x =>
                    MasterAsset.CharaData[x.CharaId].ElementalType == (UnitElement)completionValue
                ),
            QuestCompleteType.AdventurerElementNeeded
                => party.Any(x =>
                    MasterAsset.CharaData[x.CharaId].ElementalType == (UnitElement)completionValue
                ),
            QuestCompleteType.AdventurerElementLocked
                => party.All(x =>
                    MasterAsset.CharaData[x.CharaId].ElementalType != (UnitElement)completionValue
                ),
            QuestCompleteType.DragonElementRequired
                => await IsDragonConditionMet(type, completionValue, party),
            QuestCompleteType.DragonElementNeeded
                => await IsDragonConditionMet(type, completionValue, party),
            QuestCompleteType.DragonElementLocked
                => await IsDragonConditionMet(type, completionValue, party),
            QuestCompleteType.WeaponRequired
                => party.All(x => x.EquipWeaponBodyId == (WeaponBodies)completionValue),
            QuestCompleteType.MaxTraps => record.TrapCount <= completionValue,
            QuestCompleteType.MaxAfflicted => record.BadStatus <= completionValue,
            QuestCompleteType.TreasureChestCount
                => record.TreasureRecord.Count() >= completionValue,
            QuestCompleteType.AllTreasureChestsOpened => true, // TODO
            QuestCompleteType.NoContinues => record.PlayContinueCount == 0,
            QuestCompleteType.AdventurerRequired
                => party.Any(x => x.CharaId == (Charas)completionValue),
            QuestCompleteType.AllEnemyParts => true, // TODO
            QuestCompleteType.MaxTime => record.Time < completionValue,
            QuestCompleteType.MaxDamageTimes => record.DamageCount <= completionValue,
            QuestCompleteType.MinShapeshift => record.DragonTransformCount >= completionValue,
            QuestCompleteType.DefeatImperialCommander
                => record.TreasureRecord.Any(x =>
                    x.Enemy.Any(y =>
                        y == 500200001 /* Imperial Commander */
                    )
                ),
            QuestCompleteType.DefeatMinBandits
                => record.TreasureRecord.Sum(x =>
                    x.Enemy.Count(y =>
                        y == 500210001 /* Bandit */
                    )
                ) >= completionValue,
            QuestCompleteType.DefeatShadowKnight
                => record.TreasureRecord.Any(x =>
                    x.Enemy.Any(y =>
                        y == 500170001 /* Shadow Knight */
                    )
                ),
            QuestCompleteType.SaveMinHouses => record.VisitPrivateHouse >= completionValue,
            QuestCompleteType.MinGateHp => record.ProtectionDamage >= completionValue,
            QuestCompleteType.MinTimeRemaining => record.RemainingTime >= completionValue,
            QuestCompleteType.MinDrawbridgesLowered
                => record.LowerDrawbridgeCount >= completionValue,
            QuestCompleteType.FireEmblemAdventurerNeeded
                => party.Any(x =>
                    x.CharaId
                        is Charas.Alfonse
                            or Charas.Veronica
                            or Charas.Fjorm
                            or Charas.Marth
                            or Charas.Sharena
                            or Charas.Peony
                            or Charas.Tiki
                            or Charas.Chrom
                ),
            QuestCompleteType.MinStarsAdventurerNeeded
                => party.Any(x => MasterAsset.CharaData[x.CharaId].Rarity == completionValue),
            QuestCompleteType.MinAdventurersWithSameWeapon
                => party
                    .ToLookup(x => MasterAsset.CharaData[x.CharaId].WeaponType)
                    .Any(x => x.Count() >= completionValue),
            QuestCompleteType.MaxShapeshift => record.DragonTransformCount <= completionValue,
            QuestCompleteType.NoRevives => record.RebornCount == 0,
            QuestCompleteType.DefeatFormaChrom
                => record.TreasureRecord.Any(x =>
                    x.Enemy.Any(y =>
                        y == 601500002 /* Forma Chrom */
                    )
                ),
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid QuestCompleteType"
                )
        };
    }

    private async Task<bool> IsDragonConditionMet(
        QuestCompleteType type,
        int value,
        IEnumerable<PartySettingList> party
    )
    {
        IEnumerable<long> dragonKeyIds = party.Select(x => (long)x.EquipDragonKeyId).ToList();

        IEnumerable<Dragons> dragons = await unitRepository
            .Dragons.Where(x => dragonKeyIds.Contains(x.DragonKeyId))
            .Select(x => x.DragonId)
            .ToListAsync();

        return type switch
        {
            QuestCompleteType.DragonElementRequired
                => dragons.Any(x => MasterAsset.DragonData[x].ElementalType == (UnitElement)value),
            QuestCompleteType.DragonElementNeeded
                => dragons.All(x => MasterAsset.DragonData[x].ElementalType == (UnitElement)value),
            QuestCompleteType.DragonElementLocked
                => dragons.All(x => MasterAsset.DragonData[x].ElementalType != (UnitElement)value),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<IEnumerable<AtgenFirstClearSet>> GrantFirstClearRewards(int questId)
    {
        List<AtgenFirstClearSet> rewards = new();

        QuestRewardData rewardData = MasterAsset.QuestRewardData[questId];
        foreach (
            Entity rewardEntity in rewardData
                .FirstClearEntities.Where(x => x.Type != EntityTypes.None)
                .Select(x => new Entity(x.Type, x.Id, x.Quantity))
        )
        {
            await rewardService.GrantReward(rewardEntity);
            rewards.Add(
                new AtgenFirstClearSet(rewardEntity.Id, rewardEntity.Type, rewardEntity.Quantity)
            );
        }

        return rewards;
    }

    private static int GetEnemyListId(int enemyParamId)
    {
        EnemyParam enemyParam = MasterAsset.EnemyParam[enemyParamId];
        EnemyData enemyData = MasterAsset.EnemyData[enemyParam.DataId];
        return enemyData.BookId;
    }

    private static bool IsEarnTicketQuest(int questId) => questId % 1000 > 400;
}
