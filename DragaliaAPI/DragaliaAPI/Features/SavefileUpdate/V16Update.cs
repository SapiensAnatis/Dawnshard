using System.Collections.Frozen;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Shared.MasterAsset;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Fixes mismatched event item IDs.
/// </summary>
/// <remarks>
/// It was previously possible for event items to be created with an ID that did not match their item type.
/// This updates rows where this mismatch occurs, but doesn't attempt to fix quantities.
/// </remarks>
[UsedImplicitly]
public class V16Update(IEventRepository eventRepository, ILogger<V16Update> logger)
    : ISavefileUpdate
{
    private static readonly FrozenDictionary<int, int> EventItemTypes;

    static V16Update()
    {
        EventItemTypes = MasterAsset
            .EventData.Enumerable.SelectMany(x => x.GetEventSpecificItemIds())
            .ToFrozenDictionary(x => x.Id, x => x.Type);
    }

    public int SavefileVersion => 16;

    public async Task Apply()
    {
        List<DbPlayerEventItem> items = await eventRepository.Items.ToListAsync();

        foreach (DbPlayerEventItem item in items)
        {
            int expectedType = EventItemTypes[item.Id];
            int actualType = item.Type;

            if (actualType != expectedType)
            {
                logger.LogInformation(
                    "Updating event item {item} type from {currentType} to {expectedType}",
                    item.Id,
                    item.Type,
                    expectedType
                );

                item.Type = expectedType;
            }
        }
    }
}
