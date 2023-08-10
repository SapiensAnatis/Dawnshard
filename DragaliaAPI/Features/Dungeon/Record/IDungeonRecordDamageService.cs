using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordDamageService
{
    Task<EventDamageRanking> GetEventDamageRanking(PlayRecord playRecord, int eventId);
}
