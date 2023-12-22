using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Services.Game;

public class QuestTreasureService(
    IRewardService rewardService,
    IUserDataRepository userDataRepository,
    IPlayerIdentityService playerIdentityService,
    IUpdateDataService updateDataService,
    ApiContext apiContext
) : IQuestTreasureService
{
    public async Task<QuestOpenTreasureData> DoOpenTreasure(QuestOpenTreasureRequest request)
    {
        QuestTreasureData questTreasureData = MasterAsset.QuestTreasureData[
            request.quest_treasure_id
        ];

        List<AtgenBuildEventRewardEntityList> rewards = new();

        if (questTreasureData.EntityType != EntityTypes.None)
        {
            await rewardService.GrantReward(
                new Entity(
                    questTreasureData.EntityType,
                    questTreasureData.EntityId,
                    questTreasureData.EntityQuantity
                )
            );

            rewards.Add(
                new AtgenBuildEventRewardEntityList(
                    questTreasureData.EntityType,
                    questTreasureData.EntityId,
                    questTreasureData.EntityQuantity
                )
            );
        }

        if (questTreasureData.AddMaxDragonStorage != 0)
        {
            DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();
            userData.MaxDragonQuantity += questTreasureData.AddMaxDragonStorage;
        }

        apiContext.QuestTreasureList.Add(
            new DbQuestTreasureList()
            {
                ViewerId = playerIdentityService.ViewerId,
                QuestTreasureId = questTreasureData.Id
            }
        );

        IEnumerable<AtgenBuildEventRewardEntityList> quest_treasure_reward_list = rewards;

        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list =
            new List<AtgenDuplicateEntityList>();
        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        return new QuestOpenTreasureData()
        {
            update_data_list = updateDataList,
            entity_result = entityResult,
            quest_treasure_reward_list = quest_treasure_reward_list,
            duplicate_entity_list = duplicate_entity_list,
            add_max_dragon_quantity = questTreasureData.AddMaxDragonStorage,
            add_max_weapon_quantity = 0,
            add_max_amulet_quantity = 0
        };
    }
}
