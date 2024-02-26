using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Photon;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("matching")]
public class MatchingController : DragaliaControllerBase
{
    private readonly IMatchingService matchingService;
    private readonly ILogger<MatchingController> logger;

    public MatchingController(IMatchingService matchingService, ILogger<MatchingController> logger)
    {
        this.matchingService = matchingService;
        this.logger = logger;
    }

    [HttpPost("get_room_list")]
    public async Task<DragaliaResult> GetRoomList(MatchingGetRoomListRequest request)
    {
        return this.Ok(
            new MatchingGetRoomListResponse()
            {
                RoomList = await this.matchingService.GetRoomList()
            }
        );
    }

    [HttpPost("get_room_name")]
    public async Task<DragaliaResult> GetRoomName(MatchingGetRoomNameRequest request)
    {
        MatchingGetRoomNameResponse? data = await this.matchingService.GetRoomById(request.RoomId);

        if (data is null)
        {
            this.logger.LogDebug("Could not find room with id {id}", request.RoomId);
            return this.Code(ResultCode.MatchingRoomIdNotFound);
        }

        this.logger.LogDebug("Found room with id {id}: {@room}", request.RoomId, data);
        return this.Ok(data);
    }

    [HttpPost("get_room_list_by_quest_id")]
    public async Task<DragaliaResult> GetRoomListByQuestId(
        MatchingGetRoomListByQuestIdRequest request
    )
    {
        return this.Ok(
            new MatchingGetRoomListByQuestIdResponse()
            {
                RoomList = await this.matchingService.GetRoomList(request.QuestId)
            }
        );
    }

    [HttpPost("check_penalty_user")]
    public DragaliaResult CheckPenaltyUser(MatchingCheckPenaltyUserRequest request)
    {
        return this.Ok(new MatchingCheckPenaltyUserResponse() { Result = 1 });
    }
}
