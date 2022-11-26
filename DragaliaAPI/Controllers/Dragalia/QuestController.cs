using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("quest")]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
[ApiController]
public class QuestController : DragaliaControllerBase
{
    private readonly IQuestRepository questRepository;
    private readonly IUpdateDataService updateDataService;
    private const int ReadStoryState = 1;

    public QuestController(IQuestRepository questRepository, IUpdateDataService updateDataService)
    {
        this.questRepository = questRepository;
        this.updateDataService = updateDataService;
    }

    [HttpPost]
    [Route("read_story")]
    public async Task<DragaliaResult> ReadStory(QuestReadStoryRequest request)
    {
        await this.questRepository.UpdateQuestStory(
            this.DeviceAccountId,
            request.quest_story_id,
            ReadStoryState
        );

        UpdateDataList updateData = this.updateDataService.GetUpdateDataList(this.DeviceAccountId);

        await this.questRepository.SaveChangesAsync();

        QuestReadStoryData responseData =
            new(
                updateData,
                new()
                {
                    new_get_entity_list = new List<AtgenDuplicateEntityList>() /* { new(1, (int)Charas.Ilia) } */
                },
                quest_story_reward_list: new List<AtgenQuestStoryRewardList>()
                /*
                {
                    new(23, 0, 25, 0, 0),
                    new(1, (int)Charas.Ilia, 1, 5, 0)
                }*/,
                new List<ConvertedEntityList>()
            );

        return this.Ok(responseData);
    }
}
