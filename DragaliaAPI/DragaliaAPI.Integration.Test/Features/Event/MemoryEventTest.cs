namespace DragaliaAPI.Integration.Test.Features.Event;

public class MemoryEventTest : TestFixture
{
    public MemoryEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task MemoryEvent_Activate_CompletesParticipationMission()
    {
        int eventId = 20845; // Toll of the Deep

        MemoryEventActivateData result = (
            await this.Client.PostMsgpack<MemoryEventActivateData>(
                "memory_event/activate",
                new MemoryEventActivateRequest() { event_id = eventId }
            )
        ).data;

        result
            .update_data_list.mission_notice.memory_event_mission_notice.new_complete_mission_id_list.Should()
            .Contain(10220101); // Participate in the Event
    }

    [Fact]
    public async Task MemoryEvent_Activate_UnlocksNonObsoleteMissions()
    {
        int eventId = 20817; // The Miracle of Dragonyule

        int obsoleteMissionId1 = 10020301;
        int obsoleteMissionId2 = 10020701;

        await this.Client.PostMsgpack<MemoryEventActivateData>(
            "memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );

        MissionGetMissionListData missionListResponse = (
            await this.Client.PostMsgpack<MissionGetMissionListData>(
                "mission/get_mission_list",
                new MissionGetMissionListRequest() { }
            )
        ).data;

        missionListResponse
            .memory_event_mission_list.Should()
            .BeEquivalentTo(
                new MemoryEventMissionList[]
                {
                    new() { memory_event_mission_id = 10020101 },
                    new() { memory_event_mission_id = 10020102 },
                    new() { memory_event_mission_id = 10020103 },
                    new() { memory_event_mission_id = 10020104 },
                    new() { memory_event_mission_id = 10020201 },
                    new() { memory_event_mission_id = 10020302 },
                    new() { memory_event_mission_id = 10020303 },
                    new() { memory_event_mission_id = 10020304 },
                    new() { memory_event_mission_id = 10020305 },
                    new() { memory_event_mission_id = 10020401 },
                    new() { memory_event_mission_id = 10020502 },
                    new() { memory_event_mission_id = 10020503 },
                    new() { memory_event_mission_id = 10020504 },
                    new() { memory_event_mission_id = 10020505 },
                    new() { memory_event_mission_id = 10020601 },
                    new() { memory_event_mission_id = 10020602 },
                    new() { memory_event_mission_id = 10020603 },
                    new() { memory_event_mission_id = 10020604 },
                    new() { memory_event_mission_id = 10020702 },
                    new() { memory_event_mission_id = 10020703 },
                    new() { memory_event_mission_id = 10020704 },
                    new() { memory_event_mission_id = 10020801 },
                    new() { memory_event_mission_id = 10020901 },
                    new() { memory_event_mission_id = 10021001 },
                },
                opts => opts.Including(x => x.memory_event_mission_id)
            );

        this.ApiContext.PlayerMissions.Should().NotContain(x => x.Id == obsoleteMissionId1);
        this.ApiContext.PlayerMissions.Should().NotContain(x => x.Id == obsoleteMissionId2);
    }
}
