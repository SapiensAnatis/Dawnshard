using AutoMapper;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Skip;

[Route("dungeon_skip")]
public class DungeonSkipController(
    IDungeonRecordService dungeonRecordService,
    IDungeonRecordHelperService dungeonRecordHelperService,
    IPartyRepository partyRepository,
    IOddsInfoService oddsInfoService,
    IPaymentService paymentService,
    IRewardService rewardService,
    IUpdateDataService updateDataService,
    IMapper mapper
) : DragaliaControllerBase
{
    [HttpPost("start")]
    public async Task<DragaliaResult> Start(DungeonSkipStartRequest request)
    {
        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: request.play_count)
        );

        IEnumerable<PartySettingList> party = (
            await partyRepository.GetPartyUnits(request.party_no).AsNoTracking().ToListAsync()
        ).Select(mapper.Map<PartySettingList>);

        IngameResultData ingameData = await this.GetIngameResultData(
            request.quest_id,
            request.play_count,
            request.support_viewer_id,
            party
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartData()
            {
                ingame_result_data = ingameData,
                update_data_list = updateDataList,
                entity_result = entityResult
            }
        );
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(DungeonSkipStartAssignUnitRequest request)
    {
        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: request.play_count)
        );

        IngameResultData ingameData = await this.GetIngameResultData(
            request.quest_id,
            request.play_count,
            request.support_viewer_id,
            request.request_party_setting_list
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartData()
            {
                ingame_result_data = ingameData,
                update_data_list = updateDataList,
                entity_result = entityResult
            }
        );
    }

    [HttpPost("start_multiple_quest")]
    public async Task<DragaliaResult> StartMultipleQuest(
        DungeonSkipStartMultipleQuestRequest request
    )
    {
        int totalSkipQuantity = request.request_quest_multiple_list.Sum(x => x.play_count);

        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: totalSkipQuantity)
        );

        IEnumerable<PartySettingList> party = (
            await partyRepository.GetPartyUnits(request.party_no).AsNoTracking().ToListAsync()
        ).Select(mapper.Map<PartySettingList>);

        List<IngameResultData> results = new(request.request_quest_multiple_list.Count());

        foreach (AtgenRequestQuestMultipleList quest in request.request_quest_multiple_list)
        {
            IngameResultData resultData = await this.GetIngameResultData(
                quest.quest_id,
                quest.play_count,
                request.support_viewer_id,
                party
            );
            results.Add(resultData);
        }

        IngameResultData combinedResults = CombineIngameResultData(results);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartMultipleQuestData()
            {
                ingame_result_data = combinedResults,
                entity_result = entityResult,
                update_data_list = updateDataList,
            }
        );
    }

    [HttpPost("start_multiple_quest_assign_unit")]
    public async Task<DragaliaResult> StartMultipleQuest(
        DungeonSkipStartMultipleQuestAssignUnitRequest request
    )
    {
        // Unsure what calls this endpoint -- can't repeat daily bonus skip or do it with assigned team

        int totalSkipQuantity = request.request_quest_multiple_list.Sum(x => x.play_count);

        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: totalSkipQuantity)
        );

        List<IngameResultData> results = new(request.request_quest_multiple_list.Count());

        foreach (AtgenRequestQuestMultipleList quest in request.request_quest_multiple_list)
        {
            IngameResultData resultData = await this.GetIngameResultData(
                quest.quest_id,
                quest.play_count,
                request.support_viewer_id,
                request.request_party_setting_list
            );
            results.Add(resultData);
        }

        IngameResultData combinedResults = CombineIngameResultData(results);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartMultipleQuestData()
            {
                ingame_result_data = combinedResults,
                entity_result = entityResult,
                update_data_list = updateDataList,
            }
        );
    }

    private async Task<IngameResultData> GetIngameResultData(
        int questId,
        int playCount,
        ulong? supportViewerId,
        IEnumerable<PartySettingList> party
    )
    {
        if (!MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData))
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Quest ID not recognised"
            );
        }

        DungeonSession session =
            new()
            {
                Party = party.Where(x => x.chara_id != 0),
                QuestData = questData,
                SupportViewerId = supportViewerId,
                IsHost = true,
                IsMulti = false,
                PlayCount = playCount,
            };

        session.EnemyList = questData.AreaInfo
            .Select((_, index) => oddsInfoService.GetOddsInfo(questData.Id, index))
            .ToDictionary(x => x.area_index, x => x.enemy.Repeat(playCount));

        PlayRecord playRecord =
            new()
            {
                is_clear = 1,
                time = -1,
                treasure_record = session.EnemyList.Select(
                    x =>
                        new AtgenTreasureRecord()
                        {
                            area_idx = x.Key,
                            enemy = x.Value.Select(y => 1),
                            drop_obj = new List<int>(), // TODO
                            enemy_smash = new List<AtgenEnemySmash>() // TODO
                        }
                )
            };

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            string.Empty,
            playRecord,
            session
        );

        (ingameResultData.helper_list, ingameResultData.helper_detail_list) =
            await dungeonRecordHelperService.ProcessHelperDataSolo(supportViewerId);

        return ingameResultData;
    }

    private static IngameResultData CombineIngameResultData(
        IEnumerable<IngameResultData> ingameResultDatas
    )
    {
        return ingameResultDatas.Aggregate(
            (acc, current) =>
            {
                // TODO: Combine other things
                acc.reward_record.take_coin += current.reward_record.take_coin;
                acc.reward_record.drop_all.AddRange(current.reward_record.drop_all);

                acc.grow_record.take_chara_exp += current.grow_record.take_chara_exp;
                acc.grow_record.take_player_exp += current.grow_record.take_player_exp;
                acc.grow_record.take_mana += current.grow_record.take_mana;

                acc.reward_record.quest_bonus_list = acc.reward_record.quest_bonus_list.Concat(
                    current.reward_record.quest_bonus_list
                );

                acc.reward_record.player_level_up_fstone += current
                    .reward_record
                    .player_level_up_fstone;

                return acc;
            }
        );
    }
}
