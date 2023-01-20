using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("castle_story")]
public class CastleStoryController : DragaliaControllerBase
{
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<CastleStoryController> logger;

    private const int StoryWyrmiteReward = 50;

    public CastleStoryController(
        IStoryRepository storyRepository,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        ILogger<CastleStoryController> logger
    )
    {
        this.storyRepository = storyRepository;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("read")]
    public async Task<DragaliaResult> Read(CastleStoryReadRequest request)
    {
        List<AtgenBuildEventRewardEntityList> rewardList = new(); // wtf is this type

        if (
            (
                await this.storyRepository
                    .GetStoryList(this.DeviceAccountId)
                    .SingleOrDefaultAsync(
                        x =>
                            x.StoryType == StoryTypes.Castle && x.StoryId == request.castle_story_id
                    )
            )?.State != 1
        )
        {
            int lookingGlassCount =
                (
                    await this.inventoryRepository.GetMaterial(
                        this.DeviceAccountId,
                        Materials.LookingGlass
                    )
                )?.Quantity ?? 0;

            if (lookingGlassCount < 1)
            {
                throw new DragaliaException(
                    ResultCode.CastleStoryOutOfPeriod,
                    "Insufficient looking glasses"
                );
            }

            await this.inventoryRepository.AddMaterialQuantity(
                DeviceAccountId,
                Materials.LookingGlass,
                -1
            );

            this.logger.LogInformation(
                "Reading previously unread story id {id}",
                request.castle_story_id
            );

            await this.storyRepository.UpdateStory(
                DeviceAccountId,
                StoryTypes.Castle,
                request.castle_story_id,
                1
            );

            // TODO when giftbox is added: give rewards properly
            // TODO: give different wyrmite rewards for chara/dragon stories
            await this.userDataRepository.GiveWyrmite(this.DeviceAccountId, StoryWyrmiteReward);
            rewardList.Add(
                new()
                {
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = StoryWyrmiteReward
                }
            );
        }

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.storyRepository.SaveChangesAsync();

        return this.Ok(
            new CastleStoryReadData()
            {
                castle_story_reward_list = rewardList,
                update_data_list = updateDataList
            }
        );
    }
}
