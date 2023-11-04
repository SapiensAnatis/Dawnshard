using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Shared.MasterAsset.Extensions;

public static class QuestDataExtensions
{
    /// <summary>
    /// Get a boolean indicating whether the quest data is an event boss battle.
    /// </summary>
    /// <param name="questData">The quest data.</param>
    /// <returns>A boolean indicating whether the quest is an event boss battle.</returns>
    public static bool IsEventBossBattle(this QuestData questData)
    {
        if (!MasterAsset.EventData.TryGetValue(questData.Gid, out EventData? eventData))
            return false;

        int idSuffix = questData.Id % 1000;

        return eventData.EventKindType switch
        {
            EventKindType.Build => idSuffix is 301 or 302 or 303,
            EventKindType.Raid => idSuffix is 201 or 202 or 203,
            _ => false
        };
    }
}
