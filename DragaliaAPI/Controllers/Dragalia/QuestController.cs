using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("quest")]
[Consumes("application/x-msgpack")]
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
        await apiRepository.UpdateQuestStory(deviceAccountId, request.questStoryId, ReadStoryState);

        UserData userData = SavefileUserDataFactory.Create(
            await apiRepository.GetPlayerInfo(deviceAccountId).SingleAsync()
        );
        UpdateDataList updateData =
            new()
            {
                user_data = userData,
                quest_story_list = new List<QuestStory>()
                {
                    new(request.questStoryId, ReadStoryState)
                }
            };
        QuestReadStoryData responseData = new(new(), new(), updateData, EntityResultStatic.Empty);

        return this.Ok(new QuestReadStoryResponse(responseData));
    }
}
