using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public sealed class EventSummonTest : TestFixture, IAsyncLifetime
{
    private const int EventId = 20429;
    private const string Prefix = "event_summon";
    private const int InitialBox1Quantity = 535;

    public EventSummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    public async ValueTask InitializeAsync()
    {
        await this.Client.PostMsgpack<RaidEventEntryResponse>(
            "raid_event/entry",
            new RaidEventEntryRequest(EventId)
        );
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private async Task SetSummonPoints(int quantity)
    {
        int rows = await this
            .ApiContext.PlayerEventItems.Where(x =>
                x.ViewerId == this.ViewerId
                && x.EventId == EventId
                && x.Type == (int)RaidEventItemType.SummonPoint
            )
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.Quantity, quantity),
                cancellationToken: TestContext.Current.CancellationToken
            );

        if (rows != 1)
        {
            throw new InvalidOperationException("Failed to update summon points");
        }
    }

    [Fact]
    public async Task GetData_ReturnsBoxSummonData()
    {
        DragaliaResponse<EventSummonGetDataResponse> response =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonData data = response.Data.BoxSummonData;

        data.EventId.Should().Be(EventId);
        data.BoxSummonSeq.Should().Be(1);
        data.RemainingQuantity.Should().Be(535);
        data.MaxExecCount.Should().Be(100);
        data.ResetPossible.Should().BeFalse();
        data.BoxSummonDetail.Should().HaveCount(31);
    }

    [Fact]
    public async Task GetData_EventNotRunning_ReturnsError()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(20401),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.EventOutThePeriod);
    }

    [Fact]
    public async Task Exec_SufficientPoints_DrawsItemsAndDepletesCurrency()
    {
        int numSummons = 10;

        await this.SetSummonPoints(50);

        DragaliaResponse<EventSummonExecResponse> response =
            await this.Client.PostMsgpack<EventSummonExecResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, numSummons, false),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonResult result = response.Data.BoxSummonResult;

        result.EventId.Should().Be(EventId);
        result.BoxSummonSeq.Should().Be(1);
        result.RemainingQuantity.Should().Be(InitialBox1Quantity - numSummons);
        result.EventPoint.Should().Be(50 - numSummons);
        result.DrawDetails.Should().HaveCount(numSummons);

        Dictionary<int, int> drawCountsById = result
            .DrawDetails.GroupBy(d => d.Id)
            .ToDictionary(g => g.Key, g => g.Count());

        DragaliaResponse<EventSummonGetDataResponse> getDataResponse =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        List<AtgenBoxSummonDetail> afterDetails =
            getDataResponse.Data.BoxSummonData.BoxSummonDetail.ToList();

        for (int i = 0; i < afterDetails.Count; i++)
        {
            int itemKey = i + 1;
            int drawnCount = drawCountsById.GetValueOrDefault(itemKey);
            AtgenBoxSummonDetail detail = afterDetails[i];

            detail.Limit.Should().Be(detail.TotalCount - drawnCount);
        }
    }

    [Fact]
    public async Task Exec_StopByTargetEnabled_StopsAfterDrawingResetItem()
    {
        await this.SetSummonPoints(50);

        DbPlayerEventSummonData summonData = await this
            .ApiContext.PlayerEventSummonData.Include(x => x.Items)
            .AsTracking()
            .SingleAsync(
                x => x.ViewerId == this.ViewerId && x.EventId == EventId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        // Item 1 (Sophie's Conviction) is the only ResetItemFlag item in box 1.
        // Deplete every other item so the next draw is guaranteed to be item 1.
        for (int itemId = 2; itemId <= 31; itemId++)
        {
            summonData.Items.Add(
                new DbPlayerEventSummonItem { ItemId = itemId, TimesSummoned = 9999 }
            );
        }

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<EventSummonExecResponse> response =
            await this.Client.PostMsgpack<EventSummonExecResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, 10, true),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonResult result = response.Data.BoxSummonResult;

        result.IsStoppedByTarget.Should().BeTrue();
        result.DrawDetails.Should().ContainSingle().Which.Id.Should().Be(1);
        result.EventPoint.Should().Be(49);
    }

    [Fact]
    public async Task Exec_BoxRunsOut_StopsAfterFinalDraw()
    {
        await this.SetSummonPoints(9999);

        DbPlayerEventSummonData summonData = await this
            .ApiContext.PlayerEventSummonData.Include(x => x.Items)
            .AsTracking()
            .SingleAsync(
                x => x.ViewerId == this.ViewerId && x.EventId == EventId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        // Deplete every item except item 2 (Twinkling Sand, Box1Count=1), leaving a
        // single draw available so the box empties mid-request.
        for (int itemId = 1; itemId <= 31; itemId++)
        {
            if (itemId == 2)
            {
                continue;
            }

            summonData.Items.Add(
                new DbPlayerEventSummonItem { ItemId = itemId, TimesSummoned = 9999 }
            );
        }

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<EventSummonExecResponse> response =
            await this.Client.PostMsgpack<EventSummonExecResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, 10, false),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonResult result = response.Data.BoxSummonResult;

        result.DrawDetails.Should().ContainSingle().Which.Id.Should().Be(2);
        result.RemainingQuantity.Should().Be(0);
        result.ResetPossible.Should().BeTrue();
        result.IsStoppedByTarget.Should().BeFalse();
    }

    [Fact]
    public async Task Exec_InsufficientPoints_ReturnsError()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, 10, false),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.SummonPointShort);
    }
}
