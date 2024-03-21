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
    public async Task<QuestOpenTreasureResponse> DoOpenTreasure(
        QuestOpenTreasureRequest request,
        CancellationToken cancellationToken
    )
    {
        QuestTreasureData questTreasureData = MasterAsset.QuestTreasureData[
            request.QuestTreasureId
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

        IEnumerable<AtgenBuildEventRewardEntityList> questTreasureRewardList = rewards;

        IEnumerable<AtgenDuplicateEntityList> duplicateEntityList =
            new List<AtgenDuplicateEntityList>();
        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new QuestOpenTreasureResponse()
        {
            UpdateDataList = updateDataList,
            EntityResult = entityResult,
            QuestTreasureRewardList = questTreasureRewardList,
            DuplicateEntityList = duplicateEntityList,
            AddMaxDragonQuantity = questTreasureData.AddMaxDragonStorage,
            AddMaxWeaponQuantity = 0,
            AddMaxAmuletQuantity = 0
        };
    }
}
