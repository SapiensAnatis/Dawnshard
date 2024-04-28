using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordDamageService
{
    Task<EventDamageRanking> GetEventDamageRanking(PlayRecord playRecord, int eventId);
}
