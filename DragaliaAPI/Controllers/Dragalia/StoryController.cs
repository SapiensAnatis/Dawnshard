using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("story")]
public class StoryController : DragaliaControllerBase
{
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<StoryController> logger;
    private const int StoryWyrmiteReward = 25;

    public StoryController(
        IStoryRepository storyRepository,
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        ILogger<StoryController> logger
    )
    {
        this.storyRepository = storyRepository;
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("read")]
    public async Task<DragaliaResult> Read(StoryReadRequest request)
    {
        List<AtgenBuildEventRewardEntityList> rewardList = new(); // wtf is this type

        if (
            (
                await this.storyRepository
                    .GetStoryList(this.DeviceAccountId)
                    .SingleOrDefaultAsync(
                        x => x.StoryType == StoryTypes.Chara && x.StoryId == request.unit_story_id
                    )
            )?.State != 1
        )
        {
            this.logger.LogInformation(
                "Reading previously unread story id {id}",
                request.unit_story_id
            );

            await this.storyRepository.UpdateStory(
                DeviceAccountId,
                StoryTypes.Chara,
                request.unit_story_id,
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
            new StoryReadData()
            {
                unit_story_reward_list = rewardList,
                update_data_list = updateDataList
            }
        );
    }
}
