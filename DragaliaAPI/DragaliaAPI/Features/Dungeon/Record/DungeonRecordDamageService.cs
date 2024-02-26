using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordDamageService : IDungeonRecordDamageService
{
    public Task<EventDamageRanking> GetEventDamageRanking(PlayRecord playRecord, int eventId)
    {
        EventDamageRanking damageRanking =
            new()
            {
                EventId = eventId,
                OwnDamageRankingList = new List<AtgenOwnDamageRankingList>()
                {
                    // TODO: track in database to determine if it's a new personal best
                    new AtgenOwnDamageRankingList()
                    {
                        CharaId = 0,
                        Rank = 0,
                        DamageValue = playRecord?.TotalPlayDamage ?? 0,
                        IsNew = false,
                    }
                }
            };

        return Task.FromResult(damageRanking);
    }
}
