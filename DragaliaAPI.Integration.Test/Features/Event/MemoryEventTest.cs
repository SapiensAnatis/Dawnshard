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
            .update_data_list
            .mission_notice
            .memory_event_mission_notice
            .new_complete_mission_id_list
            .Should()
            .Contain(10220101); // Participate in the Event
    }
}
