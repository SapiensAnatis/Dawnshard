using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("matching")]
public class MatchingController : DragaliaControllerBase
{
    private const int VolksWrathExpertCoOp = 219010102;
    private const int RoomId = 4201337;
    private const string ClusterName = "en";

    private enum QuestType
    {
        NONE,
        DUNGEON,
        STORY,
        TREASURE,
        WALL
    }

    private enum RoomStatus
    {
        None,
        NoData,
        Available,
        Full,
        InvalidCompatibleId,
        Error
    }

    private static readonly RoomList TestRoomData =
        new()
        {
            cluster_name = ClusterName,
            host_name = "DreadfullyDistinct",
            language = "en",
            region = "en",
            leader_chara_id = (int)Charas.Marth,
            leader_chara_level = 100,
            leader_chara_rarity = 5,
            host_level = 100,
            host_viewer_id = 1,
            member_num = 1,
            quest_id = VolksWrathExpertCoOp,
            quest_type = (int)QuestType.DUNGEON,
            room_id = RoomId,
            entry_conditions = new()
            {
                objective_free_text = "To try and break the server",
                required_party_power = 0,
                unaccepted_element_type_list = new List<int>(), // seem to be needed otherwise UI breaks
                unaccepted_weapon_type_list = new List<int>(),
            },
            room_name = "Static room name",
            status = (int)RoomStatus.Available,
            room_member_list = new List<AtgenRoomMemberList>() { new() { viewer_id = 2, } },
            entry_type = default,
            start_entry_time = default,
            entry_guild_id = default,
            compatible_id = default,
        };

    [HttpPost("get_room_name")]
    public async Task<DragaliaResult> GetRoomName(MatchingGetRoomNameRequest request)
    {
        return this.Ok(
            new MatchingGetRoomNameData()
            {
                cluster_name = ClusterName,
                is_friend = 1,
                quest_id = VolksWrathExpertCoOp,
                room_name = "/matching/get_room_name",
                room_data = TestRoomData
            }
        );
    }

    [HttpPost("get_room_list")]
    public async Task<DragaliaResult> GetRoomList(MatchingGetRoomListRequest request)
    {
        return this.Ok(
            new MatchingGetRoomListData() { room_list = new List<RoomList>() { TestRoomData } }
        );
    }
}
