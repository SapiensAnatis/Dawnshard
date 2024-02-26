using DragaliaAPI.Photon.Shared.Models;

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
        this.MockPhotonStateApi.Setup(x => x.GetAllGames())
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

        MatchingGetRoomListResponse data = (
            await this.Client.PostMsgpack<MatchingGetRoomListResponse>(
                $"{EndpointGroup}/get_room_list",
                new MatchingGetRoomListRequest() { CompatibleId = 36 }
            )
        ).Data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomListResponse()
                {
                    RoomList = new List<RoomList>()
                    {
                        new()
                        {
                            RoomId = 911948,
                            RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                            Region = "jp",
                            ClusterName = "jp",
                            Language = "en_us",
                            Status = RoomStatuses.Available,
                            EntryType = 1,
                            EntryGuildId = 0,
                            HostViewerId = (ulong)ViewerId,
                            HostName = "Euden",
                            HostLevel = 250,
                            LeaderCharaId = Charas.ThePrince,
                            LeaderCharaRarity = 4,
                            LeaderCharaLevel = 1,
                            QuestId = 204550501,
                            QuestType = QuestTypes.Dungeon,
                            RoomMemberList = new List<AtgenRoomMemberList>()
                            {
                                new() { ViewerId = (ulong)ViewerId, }
                            },
                            StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                            EntryConditions = new()
                            {
                                UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                                UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                                RequiredPartyPower = 100,
                                ObjectiveTextId = 2
                            },
                            CompatibleId = 36,
                            MemberNum = 1,
                        }
                    }
                }
            );
    }

    [Fact]
    public async Task GetRoomListByQuestId_ReturnsRoomList()
    {
        this.MockPhotonStateApi.Setup(x => x.GetByQuestId(204550501))
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

        MatchingGetRoomListByQuestIdResponse data = (
            await this.Client.PostMsgpack<MatchingGetRoomListByQuestIdResponse>(
                $"{EndpointGroup}/get_room_list_by_quest_id",
                new MatchingGetRoomListByQuestIdRequest() { CompatibleId = 36, QuestId = 204550501 }
            )
        ).Data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomListByQuestIdResponse()
                {
                    RoomList = new List<RoomList>()
                    {
                        new()
                        {
                            RoomId = 911948,
                            RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                            Region = "jp",
                            ClusterName = "jp",
                            Language = "en_us",
                            Status = RoomStatuses.Available,
                            EntryType = 1,
                            EntryGuildId = 0,
                            HostViewerId = (ulong)ViewerId,
                            HostName = "Euden",
                            HostLevel = 250,
                            LeaderCharaId = Charas.ThePrince,
                            LeaderCharaRarity = 4,
                            LeaderCharaLevel = 1,
                            QuestId = 204550501,
                            QuestType = QuestTypes.Dungeon,
                            RoomMemberList = new List<AtgenRoomMemberList>()
                            {
                                new() { ViewerId = (ulong)ViewerId, }
                            },
                            StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                            EntryConditions = new()
                            {
                                UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                                UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                                RequiredPartyPower = 100,
                                ObjectiveTextId = 2
                            },
                            CompatibleId = 36,
                            MemberNum = 1,
                        }
                    }
                }
            );
    }

    [Fact]
    public async Task GetRoomName_ReturnsRoom()
    {
        this.MockPhotonStateApi.Setup(x => x.GetGameById(911948))
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

        MatchingGetRoomNameResponse data = (
            await this.Client.PostMsgpack<MatchingGetRoomNameResponse>(
                $"{EndpointGroup}/get_room_name",
                new MatchingGetRoomNameRequest() { RoomId = 911948 }
            )
        ).Data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomNameResponse()
                {
                    RoomData = new()
                    {
                        RoomId = 911948,
                        RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        Region = "jp",
                        ClusterName = "jp",
                        Language = "en_us",
                        Status = RoomStatuses.Available,
                        EntryType = 1,
                        EntryGuildId = 0,
                        HostViewerId = (ulong)ViewerId,
                        HostName = "Euden",
                        HostLevel = 250,
                        LeaderCharaId = Charas.ThePrince,
                        LeaderCharaRarity = 4,
                        LeaderCharaLevel = 1,
                        QuestId = 204550501,
                        QuestType = QuestTypes.Dungeon,
                        RoomMemberList = new List<AtgenRoomMemberList>()
                        {
                            new() { ViewerId = (ulong)ViewerId, }
                        },
                        StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        EntryConditions = new()
                        {
                            UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                            UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                            RequiredPartyPower = 100,
                            ObjectiveTextId = 2
                        },
                        CompatibleId = 36,
                        MemberNum = 1,
                    },
                    ClusterName = "jp",
                    IsFriend = false,
                    QuestId = 204550501,
                    RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953"
                }
            );
    }

    [Fact]
    public async Task GetRoomName_UnrecognizedViewerId_UsesDefault()
    {
        this.MockPhotonStateApi.Setup(x => x.GetGameById(911948))
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

        MatchingGetRoomNameResponse data = (
            await this.Client.PostMsgpack<MatchingGetRoomNameResponse>(
                $"{EndpointGroup}/get_room_name",
                new MatchingGetRoomNameRequest() { RoomId = 911948 }
            )
        ).Data;

        data.Should()
            .BeEquivalentTo(
                new MatchingGetRoomNameResponse()
                {
                    RoomData = new()
                    {
                        RoomId = 911948,
                        RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953",
                        Region = "jp",
                        ClusterName = "jp",
                        Language = "en_us",
                        Status = RoomStatuses.Available,
                        EntryType = 1,
                        EntryGuildId = 0,
                        HostViewerId = 40000,
                        HostName = "Euden",
                        HostLevel = 1,
                        LeaderCharaId = Charas.ThePrince,
                        LeaderCharaRarity = 4,
                        LeaderCharaLevel = 1,
                        QuestId = 204550501,
                        QuestType = QuestTypes.Dungeon,
                        RoomMemberList = new List<AtgenRoomMemberList>()
                        {
                            new() { ViewerId = 40000, }
                        },
                        StartEntryTime = DateTimeOffset.FromUnixTimeSeconds(1662160789),
                        EntryConditions = new()
                        {
                            UnacceptedElementTypeList = new List<int>() { 1, 2, 3, 4, 5 },
                            UnacceptedWeaponTypeList = new List<int>() { 1, 2 },
                            RequiredPartyPower = 100,
                            ObjectiveTextId = 2
                        },
                        CompatibleId = 36,
                        MemberNum = 1,
                    },
                    ClusterName = "jp",
                    IsFriend = false,
                    QuestId = 204550501,
                    RoomName = "7942ce2a-c0ac-4e41-8472-0cf5918f3953"
                }
            );
    }
}
