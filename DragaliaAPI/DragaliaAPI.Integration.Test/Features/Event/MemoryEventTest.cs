namespace DragaliaAPI.Integration.Test.Features.Event;

public class MemoryEventTest : TestFixture
{
    public MemoryEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task MemoryEvent_Activate_CompletesParticipationMission()
    {
        int eventId = 20845; // Toll of the Deep

        MemoryEventActivateResponse result = (
            await this.Client.PostMsgpack<MemoryEventActivateResponse>(
                "memory_event/activate",
                new MemoryEventActivateRequest() { EventId = eventId }
            )
        ).Data;

        result
            .UpdateDataList.MissionNotice.MemoryEventMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(10220101); // Participate in the Event
    }

    [Fact]
    public async Task MemoryEvent_Activate_UnlocksNonObsoleteMissions()
    {
        int eventId = 20817; // The Miracle of Dragonyule

        int obsoleteMissionId1 = 10020301;
        int obsoleteMissionId2 = 10020701;

        await this.Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        MissionGetMissionListResponse missionListResponse = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>("mission/get_mission_list")
        ).Data;

        missionListResponse
            .MemoryEventMissionList.Should()
            .BeEquivalentTo(
                new MemoryEventMissionList[]
                {
                    new() { MemoryEventMissionId = 10020101 },
                    new() { MemoryEventMissionId = 10020102 },
                    new() { MemoryEventMissionId = 10020103 },
                    new() { MemoryEventMissionId = 10020104 },
                    new() { MemoryEventMissionId = 10020201 },
                    new() { MemoryEventMissionId = 10020302 },
                    new() { MemoryEventMissionId = 10020303 },
                    new() { MemoryEventMissionId = 10020304 },
                    new() { MemoryEventMissionId = 10020305 },
                    new() { MemoryEventMissionId = 10020401 },
                    new() { MemoryEventMissionId = 10020502 },
                    new() { MemoryEventMissionId = 10020503 },
                    new() { MemoryEventMissionId = 10020504 },
                    new() { MemoryEventMissionId = 10020505 },
                    new() { MemoryEventMissionId = 10020601 },
                    new() { MemoryEventMissionId = 10020602 },
                    new() { MemoryEventMissionId = 10020603 },
                    new() { MemoryEventMissionId = 10020604 },
                    new() { MemoryEventMissionId = 10020702 },
                    new() { MemoryEventMissionId = 10020703 },
                    new() { MemoryEventMissionId = 10020704 },
                    new() { MemoryEventMissionId = 10020801 },
                    new() { MemoryEventMissionId = 10020901 },
                    new() { MemoryEventMissionId = 10021001 },
                },
                opts => opts.Including(x => x.MemoryEventMissionId)
            );

        this.ApiContext.PlayerMissions.Should().NotContain(x => x.Id == obsoleteMissionId1);
        this.ApiContext.PlayerMissions.Should().NotContain(x => x.Id == obsoleteMissionId2);
    }
}
