using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class EventQuestResetTest : TestFixture
{
    public EventQuestResetTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    private const int CombatEventId = 22213;
    private const int CombatEventQuestId = 222130103;

    [Fact]
    public async Task Activate_FirstEntry_ResetsStaleQuestRecords()
    {
        ApiContext.PlayerQuests.Add(
            new DbQuest
            {
                ViewerId = ViewerId,
                QuestId = CombatEventQuestId,
                State = 3,
                IsMissionClear1 = true,
                IsMissionClear2 = true,
                IsMissionClear3 = true,
                PlayCount = 5,
                DailyPlayCount = 5,
                WeeklyPlayCount = 5,
                BestClearTime = 12.5f,
            }
        );

        await ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(CombatEventId),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbQuest reset = await ApiContext
            .PlayerQuests.AsNoTracking()
            .SingleAsync(
                x => x.ViewerId == ViewerId && x.QuestId == CombatEventQuestId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        reset.State.Should().Be(0);
        reset.IsMissionClear1.Should().BeFalse();
        reset.IsMissionClear2.Should().BeFalse();
        reset.IsMissionClear3.Should().BeFalse();
        reset.PlayCount.Should().Be(0);
        reset.DailyPlayCount.Should().Be(0);
        reset.WeeklyPlayCount.Should().Be(0);
        reset.BestClearTime.Should().Be(-1.0f);
    }

    [Fact]
    public async Task Activate_SubsequentEntry_PreservesQuestRecords()
    {
        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(CombatEventId),
            cancellationToken: TestContext.Current.CancellationToken
        );

        ApiContext.PlayerQuests.RemoveRange(
            ApiContext.PlayerQuests.Where(x =>
                x.ViewerId == ViewerId && x.QuestId == CombatEventQuestId
            )
        );
        ApiContext.PlayerQuests.Add(
            new DbQuest
            {
                ViewerId = ViewerId,
                QuestId = CombatEventQuestId,
                State = 3,
                IsMissionClear1 = true,
                PlayCount = 2,
                BestClearTime = 9.5f,
            }
        );

        await ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(CombatEventId),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbQuest preserved = await ApiContext
            .PlayerQuests.AsNoTracking()
            .SingleAsync(
                x => x.ViewerId == ViewerId && x.QuestId == CombatEventQuestId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        preserved.State.Should().Be(3);
        preserved.IsMissionClear1.Should().BeTrue();
        preserved.PlayCount.Should().Be(2);
        preserved.BestClearTime.Should().Be(9.5f);
    }

    [Fact]
    public async Task Activate_FirstEntry_DoesNotResetUnrelatedQuestRecords()
    {
        const int unrelatedQuestId = 100010101;

        ApiContext.PlayerQuests.Add(
            new DbQuest
            {
                ViewerId = ViewerId,
                QuestId = unrelatedQuestId,
                State = 3,
                IsMissionClear1 = true,
                PlayCount = 7,
                BestClearTime = 4.2f,
            }
        );

        await ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(CombatEventId),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbQuest unrelated = await ApiContext
            .PlayerQuests.AsNoTracking()
            .SingleAsync(
                x => x.ViewerId == ViewerId && x.QuestId == unrelatedQuestId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        unrelated.State.Should().Be(3);
        unrelated.IsMissionClear1.Should().BeTrue();
        unrelated.PlayCount.Should().Be(7);
        unrelated.BestClearTime.Should().Be(4.2f);
    }
}
