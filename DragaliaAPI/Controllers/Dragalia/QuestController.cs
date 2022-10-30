using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("quest")]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
[ApiController]
public class QuestController : ControllerBase
{
    private readonly IApiRepository apiRepository;
    private readonly ISessionService sessionService;
    private const int ReadStoryState = 1;

    public QuestController(IApiRepository apiRepository, ISessionService sessionService)
    {
        this.apiRepository = apiRepository;
        this.sessionService = sessionService;
    }

    [HttpPost]
    [Route("read_story")]
    public async Task<DragaliaResult> ReadStory(
        [FromHeader(Name = "SID")] string sessionId,
        QuestReadStoryRequest request
    )
    {
        string deviceAccountId = await this.sessionService.GetDeviceAccountId_SessionId(sessionId);
        await apiRepository.UpdateQuestStory(
            deviceAccountId,
            request.quest_story_id,
            ReadStoryState
        );

        UserData userData = SavefileUserDataFactory.Create(
            await apiRepository.GetPlayerInfo(deviceAccountId).SingleAsync()
        );

        UpdateDataList updateData =
            new()
            {
                user_data = userData,
                quest_story_list = new List<QuestStory>()
                {
                    new(request.quest_story_id, ReadStoryState)
                }
            };

        QuestReadStoryData responseData =
            new(
                quest_story_reward_list: new()
                /*
                {
                    new(23, 0, 25, 0, 0),
                    new(1, (int)Charas.Ilia, 1, 5, 0)
                }*/,
                new(),
                updateData,
                new(
                    converted_entity_list: new List<BaseNewEntity>(),
                    new_get_entity_list: new List<BaseNewEntity>() /* { new(1, (int)Charas.Ilia) } */
                )
            );

        return this.Ok(new QuestReadStoryResponse(responseData));
    }
}
