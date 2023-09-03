using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class MatchingTest : TestFixture
{
    private const string EndpointGroup = "/matching";

    public MatchingTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockPhotonStateApi.Reset();
    }

    [Fact]
    public async Task GetRoomList_ReturnsRoomList()
    {
        this.MockPhotonStateApi
            .Setup(x => x.GetAllGames())
            .ReturnsAsync(
                new List<ApiGame>()
                {
                    new()
                    {
                        RoomId = 911948,
                        Name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        QuestId = 204550501,
                        Players = new List<Player>()
                        {
                            new()
                            {
                                ActorNr = 1,
                                ViewerId = ViewerId,
                                PartyNoList = new List<int>() { 1 }
                            }
                        },
                        StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        EntryConditions = new()
                        {
                            UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                            UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                            RequiredPartyPower = 100,
                            ObjectiveTextId = 2,
                        },
                        MatchingCompatibleId = 36,
                    }
                }
            );

        MatchingGetRoomListData data = (
            await this.Client.PostMsgpack<MatchingGetRoomListData>(
                $"{EndpointGroup}/get_room_list",
                new MatchingGetRoomListRequest() { compatible_id = 36 }
            )
        ).data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomListData()
                {
                    room_list = new List<RoomList>()
                    {
                        new()
                        {
                            room_id = 911948,
                            room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                            region = "jp",
                            cluster_name = "jp",
                            language = "en_us",
                            status = RoomStatuses.Available,
                            entry_type = 1,
                            entry_guild_id = 0,
                            host_viewer_id = (ulong)ViewerId,
                            host_name = "Euden",
                            host_level = 250,
                            leader_chara_id = Charas.ThePrince,
                            leader_chara_rarity = 4,
                            leader_chara_level = 1,
                            quest_id = 204550501,
                            quest_type = QuestTypes.Dungeon,
                            room_member_list = new List<AtgenRoomMemberList>()
                            {
                                new() { viewer_id = (ulong)ViewerId, }
                            },
                            start_entry_time = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                            entry_conditions = new()
                            {
                                unaccepted_element_type_list = new List<int>() { 1, 2, 3, 4, 5 },
                                unaccepted_weapon_type_list = new List<int>() { 1, 2 },
                                required_party_power = 100,
                                objective_text_id = 2
                            },
                            compatible_id = 36,
                            member_num = 1,
                        }
                    }
                }
            );
    }

    [Fact]
    public async Task GetRoomListByQuestId_ReturnsRoomList()
    {
        this.MockPhotonStateApi
            .Setup(x => x.GetByQuestId(204550501))
            .ReturnsAsync(
                new List<ApiGame>()
                {
                    new()
                    {
                        RoomId = 911948,
                        Name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        QuestId = 204550501,
                        Players = new List<Player>()
                        {
                            new()
                            {
                                ActorNr = 1,
                                ViewerId = ViewerId,
                                PartyNoList = new List<int>() { 1 }
                            }
                        },
                        StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        EntryConditions = new()
                        {
                            UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                            UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                            RequiredPartyPower = 100,
                            ObjectiveTextId = 2,
                        },
                        MatchingCompatibleId = 36,
                    }
                }
            );

        MatchingGetRoomListByQuestIdData data = (
            await this.Client.PostMsgpack<MatchingGetRoomListByQuestIdData>(
                $"{EndpointGroup}/get_room_list_by_quest_id",
                new MatchingGetRoomListByQuestIdRequest()
                {
                    compatible_id = 36,
                    quest_id = 204550501
                }
            )
        ).data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomListByQuestIdData()
                {
                    room_list = new List<RoomList>()
                    {
                        new()
                        {
                            room_id = 911948,
                            room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                            region = "jp",
                            cluster_name = "jp",
                            language = "en_us",
                            status = RoomStatuses.Available,
                            entry_type = 1,
                            entry_guild_id = 0,
                            host_viewer_id = (ulong)ViewerId,
                            host_name = "Euden",
                            host_level = 250,
                            leader_chara_id = Charas.ThePrince,
                            leader_chara_rarity = 4,
                            leader_chara_level = 1,
                            quest_id = 204550501,
                            quest_type = QuestTypes.Dungeon,
                            room_member_list = new List<AtgenRoomMemberList>()
                            {
                                new() { viewer_id = (ulong)ViewerId, }
                            },
                            start_entry_time = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                            entry_conditions = new()
                            {
                                unaccepted_element_type_list = new List<int>() { 1, 2, 3, 4, 5 },
                                unaccepted_weapon_type_list = new List<int>() { 1, 2 },
                                required_party_power = 100,
                                objective_text_id = 2
                            },
                            compatible_id = 36,
                            member_num = 1,
                        }
                    }
                }
            );
    }

    [Fact]
    public async Task GetRoomName_ReturnsRoom()
    {
        this.MockPhotonStateApi
            .Setup(x => x.GetGameById(911948))
            .ReturnsAsync(
                new ApiGame()
                {
                    RoomId = 911948,
                    Name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                    QuestId = 204550501,
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ActorNr = 1,
                            ViewerId = ViewerId,
                            PartyNoList = new List<int>() { 1 }
                        }
                    },
                    StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                        RequiredPartyPower = 100,
                        ObjectiveTextId = 2,
                    },
                    MatchingCompatibleId = 36,
                }
            );

        MatchingGetRoomNameData data = (
            await this.Client.PostMsgpack<MatchingGetRoomNameData>(
                $"{EndpointGroup}/get_room_name",
                new MatchingGetRoomNameRequest() { room_id = 911948 }
            )
        ).data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomNameData()
                {
                    room_data = new()
                    {
                        room_id = 911948,
                        room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        region = "jp",
                        cluster_name = "jp",
                        language = "en_us",
                        status = RoomStatuses.Available,
                        entry_type = 1,
                        entry_guild_id = 0,
                        host_viewer_id = (ulong)ViewerId,
                        host_name = "Euden",
                        host_level = 250,
                        leader_chara_id = Charas.ThePrince,
                        leader_chara_rarity = 4,
                        leader_chara_level = 1,
                        quest_id = 204550501,
                        quest_type = QuestTypes.Dungeon,
                        room_member_list = new List<AtgenRoomMemberList>()
                        {
                            new() { viewer_id = (ulong)ViewerId, }
                        },
                        start_entry_time = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        entry_conditions = new()
                        {
                            unaccepted_element_type_list = new List<int>() { 1, 2, 3, 4, 5 },
                            unaccepted_weapon_type_list = new List<int>() { 1, 2 },
                            required_party_power = 100,
                            objective_text_id = 2
                        },
                        compatible_id = 36,
                        member_num = 1,
                    },
                    cluster_name = "jp",
                    is_friend = 0,
                    quest_id = 204550501,
                    room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953"
                }
            );
    }

    [Fact]
    public async Task GetRoomName_UnrecognizedViewerId_UsesDefault()
    {
        this.MockPhotonStateApi
            .Setup(x => x.GetGameById(911948))
            .ReturnsAsync(
                new ApiGame()
                {
                    RoomId = 911948,
                    Name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                    QuestId = 204550501,
                    Players = new List<Player>()
                    {
                        new()
                        {
                            ActorNr = 1,
                            ViewerId = 40000,
                            PartyNoList = new List<int>() { 1 }
                        }
                    },
                    StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                    EntryConditions = new()
                    {
                        UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                        UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                        RequiredPartyPower = 100,
                        ObjectiveTextId = 2,
                    },
                    MatchingCompatibleId = 36,
                }
            );

        MatchingGetRoomNameData data = (
            await this.Client.PostMsgpack<MatchingGetRoomNameData>(
                $"{EndpointGroup}/get_room_name",
                new MatchingGetRoomNameRequest() { room_id = 911948 }
            )
        ).data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomNameData()
                {
                    room_data = new()
                    {
                        room_id = 911948,
                        room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        region = "jp",
                        cluster_name = "jp",
                        language = "en_us",
                        status = RoomStatuses.Available,
                        entry_type = 1,
                        entry_guild_id = 0,
                        host_viewer_id = 40000,
                        host_name = "Euden",
                        host_level = 1,
                        leader_chara_id = Charas.ThePrince,
                        leader_chara_rarity = 4,
                        leader_chara_level = 1,
                        quest_id = 204550501,
                        quest_type = QuestTypes.Dungeon,
                        room_member_list = new List<AtgenRoomMemberList>()
                        {
                            new() { viewer_id = 40000, }
                        },
                        start_entry_time = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        entry_conditions = new()
                        {
                            unaccepted_element_type_list = new List<int>() { 1, 2, 3, 4, 5 },
                            unaccepted_weapon_type_list = new List<int>() { 1, 2 },
                            required_party_power = 100,
                            objective_text_id = 2
                        },
                        compatible_id = 36,
                        member_num = 1,
                    },
                    cluster_name = "jp",
                    is_friend = 0,
                    quest_id = 204550501,
                    room_name = "7942ce2a-c0ac-4e41-8472-0cf5918f3953"
                }
            );
    }
}
