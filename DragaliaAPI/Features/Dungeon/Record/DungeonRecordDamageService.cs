using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordDamageService : IDungeonRecordDamageService
{
    public Task<EventDamageRanking> GetEventDamageRanking(PlayRecord playRecord, int eventId)
    {
        EventDamageRanking damageRanking =
            new()
            {
                event_id = eventId,
                own_damage_ranking_list = new List<AtgenOwnDamageRankingList>()
                {
                    // TODO: track in database to determine if it's a new personal best
                    new AtgenOwnDamageRankingList()
                    {
                        chara_id = 0,
                        rank = 0,
                        damage_value = playRecord?.total_play_damage ?? 0,
                        is_new = false,
                    }
                }
            };

        return Task.FromResult(damageRanking);
    }
}
