using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Event.Summon;

public class EventSummonService(
    ApiContext apiContext,
    IOptionsMonitor<EventSummonOptions> eventSummonOptionsMonitor
)
{
    private const int MaxExecCount = 10;

    private readonly EventSummonOptions eventSummonOptions = eventSummonOptionsMonitor.CurrentValue;

    public async Task<AtgenBoxSummonData> GetBoxSummonData(int eventId)
    {
        var eventSummonInfo =
            await apiContext
                .PlayerEventSummonData.Where(x => x.EventId == eventId)
                .Select(x => new
                {
                    x.BoxNumber,
                    x.Points,
                    Items = x.Items.Select(y => new { y.ItemId, y.TimesSummoned })
                })
                .FirstOrDefaultAsync()
            ?? throw new DragaliaException(
                ResultCode.EntityRaidEventDataNotFound,
                "No event summon data found"
            );

        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemDict =
            this.eventSummonOptions.EventSummons.FirstOrDefault(x => x.EventId == eventId)?.Items
            ?? throw new InvalidOperationException(
                $"No item summon table configured for event {eventId}!"
            );

        EventSummonLogic.BoxSummonInfo data = EventSummonLogic.GetBoxSummonInfo(
            eventSummonInfo.BoxNumber,
            eventSummonInfo.Items.ToDictionary(x => x.ItemId, x => x.TimesSummoned),
            itemDict
        );

        return new()
        {
            EventId = eventId,
            EventPoint = 1000,
            MaxExecCount = MaxExecCount,
            RemainingQuantity = data.RemainingQuantity,
            ResetPossible = data.ResetPossible,
            BoxSummonDetail = data.DetailList,
            BoxSummonSeq = eventSummonInfo.BoxNumber,
        };
    }
}
