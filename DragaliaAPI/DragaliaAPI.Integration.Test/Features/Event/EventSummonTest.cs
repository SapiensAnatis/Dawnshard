using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event.Summon;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    private IEnumerable<int> GetItemIds()
    {
        EventSummonOptions options = this
            .Services.GetRequiredService<IOptionsMonitor<EventSummonOptions>>()
            .CurrentValue;

        return options.EventSummons.Single(x => x.EventId == EventId).Items.Keys;
    }

    private async Task DepleteItems(params IEnumerable<int> itemIds)
    {
        DbPlayerEventSummonData summonData = await this
            .ApiContext.PlayerEventSummonData.Include(x => x.Items)
            .AsTracking()
            .SingleAsync(
                x => x.ViewerId == this.ViewerId && x.EventId == EventId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        foreach (int itemId in itemIds)
        {
            summonData.Items.Add(
                new DbPlayerEventSummonItem { ItemId = itemId, TimesSummoned = 9999 }
            );
        }

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
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

        foreach (AtgenBoxSummonDetail detail in getDataResponse.Data.BoxSummonData.BoxSummonDetail)
        {
            int drawnCount = drawCountsById.GetValueOrDefault(detail.Id);
            detail.Limit.Should().Be(detail.TotalCount - drawnCount);
        }
    }

    [Fact]
    public async Task Exec_StopByTargetEnabled_StopsAfterDrawingResetItem()
    {
        await this.SetSummonPoints(50);

        // Item 1 (Sophie's Conviction) is the only ResetItemFlag item in box 1.
        // Deplete every other item so the next draw is guaranteed to be item 1.
        await this.DepleteItems(this.GetItemIds().Except([1]));

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

        // Deplete every item except item 2 (Twinkling Sand, Box1Count=1), leaving a
        // single draw available so the box empties mid-request.
        await this.DepleteItems(this.GetItemIds().Except([2]));

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
    public async Task Exec_DrawsLastResetItem_ResetPossibleFlipsToTrue()
    {
        await this.SetSummonPoints(9999);

        // Deplete every item except item 1 (Sophie's Conviction, the only reset item),
        // leaving it as the sole remaining draw.
        await this.DepleteItems(this.GetItemIds().Except([1]));

        DragaliaResponse<EventSummonGetDataResponse> getDataBefore =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        getDataBefore.Data.BoxSummonData.ResetPossible.Should().BeFalse();

        DragaliaResponse<EventSummonExecResponse> response =
            await this.Client.PostMsgpack<EventSummonExecResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, 1, false),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonResult result = response.Data.BoxSummonResult;

        result.DrawDetails.Should().ContainSingle().Which.Id.Should().Be(1);
        result.ResetPossible.Should().BeTrue();
    }

    [Fact]
    public async Task Exec_BoxFullyDepleted_ReturnsError()
    {
        await this.SetSummonPoints(9999);
        await this.DepleteItems(this.GetItemIds());

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{Prefix}/exec",
                new EventSummonExecRequest(EventId, 10, false),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task Reset_ResetPossible_AdvancesBoxAndRefillsItems()
    {
        await this.DepleteItems(this.GetItemIds());

        DragaliaResponse<EventSummonGetDataResponse> beforeReset =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        beforeReset.Data.BoxSummonData.BoxSummonDetail.Single(x => x.Id == 1).Limit.Should().Be(0);

        await this.Client.PostMsgpack<EventSummonResetResponse>(
            $"{Prefix}/reset",
            new EventSummonResetRequest(EventId),
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<EventSummonGetDataResponse> afterReset =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonData data = afterReset.Data.BoxSummonData;

        data.BoxSummonSeq.Should().Be(2);
        data.BoxSummonDetail.Single(x => x.Id == 1).Limit.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Reset_ResetNotPossible_ReturnsError()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{Prefix}/reset",
                new EventSummonResetRequest(EventId),
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task Reset_AdvancesThroughAllBoxes_FinalBoxHasNoConvictions()
    {
        // Sophie's Conviction has Box1..Box4Count = 1 but FinalCount = 0. Four
        // resets advance box 1 -> box 5 (the final box), at which point no more
        // convictions should be obtainable.
        for (int i = 0; i < 4; i++)
        {
            await this.DepleteItems(1);

            await this.Client.PostMsgpack<EventSummonResetResponse>(
                $"{Prefix}/reset",
                new EventSummonResetRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

            this.ApiContext.ChangeTracker.Clear();
        }

        DragaliaResponse<EventSummonGetDataResponse> response =
            await this.Client.PostMsgpack<EventSummonGetDataResponse>(
                $"{Prefix}/get_data",
                new EventSummonGetDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        AtgenBoxSummonData data = response.Data.BoxSummonData;

        data.BoxSummonSeq.Should().Be(5);

        AtgenBoxSummonDetail convictions = data.BoxSummonDetail.Single(x => x.Id == 1);
        convictions.TotalCount.Should().Be(0);
        convictions.Limit.Should().Be(0);
    }
}
