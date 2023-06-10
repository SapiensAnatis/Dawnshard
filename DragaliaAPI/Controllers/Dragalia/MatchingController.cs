using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("matching")]
[AllowAnonymous]
public class MatchingController : DragaliaControllerBase
{
    private const int VolksWrathExpertCoOp = 219010102;
    private const int RoomId = 4201337;
    private const string Japan = "jp";

    private static readonly RoomList TestRoomData =
        new()
        {
            cluster_name = Japan,
            region = Japan,
            host_name = "DreadfullyDistinct",
            language = "en_us",
            leader_chara_id = Charas.Marth,
            leader_chara_level = 100,
            leader_chara_rarity = 5,
            host_level = 100,
            host_viewer_id = 1,
            member_num = 1,
            quest_id = VolksWrathExpertCoOp,
            quest_type = QuestTypes.Dungeon,
            room_id = RoomId,
            entry_conditions = new()
            {
                objective_text_id = 0,
                required_party_power = 0,
                unaccepted_element_type_list = new List<int>(), // seem to be needed otherwise UI breaks
                unaccepted_weapon_type_list = new List<int>(),
            },
            room_name = "5966a844-c879-4698-9069-d7f39d4e7277",
            status = RoomStatuses.Available,
            room_member_list = new List<AtgenRoomMemberList>() { new() { viewer_id = 2, } },
            entry_type = 1,
            start_entry_time = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1),
            entry_guild_id = default,
            compatible_id = 36,
        };
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
            new MatchingGetRoomListData() { room_list = await this.matchingService.GetRoomList() }
        );
    }

    [HttpPost("get_room_name")]
    public async Task<DragaliaResult> GetRoomName(MatchingGetRoomNameRequest request)
    {
        MatchingGetRoomNameData? data = await this.matchingService.GetRoomById(request.room_id);

        if (data is null)
        {
            this.logger.LogDebug("Could not find room with id {id}", request.room_id);
            return this.Code(Models.ResultCode.MatchingRoomIdNotFound);
        }

        this.logger.LogDebug("Found room with id {id}: {@room}", request.room_id, data);
        return this.Ok(data);
    }
}
