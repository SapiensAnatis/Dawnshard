using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.Summon;

public class SummonHistoryFactory
{
    /// UNKNOWN: params: summon_point_id(why?)
    /// <summary>
    /// A summon history entry
    /// </summary>
    /// <param name="key_id">Unique Id of the summon</param>
    /// <param name="summon_id">Banner Id</param>
    /// <param name="summon_exec_type">Distinguishing 1x from 10x</param>
    /// <param name="exec_date">Summon date</param>
    /// <param name="payment_type">Summon currency type</param>
    /// <param name="summon_prize_rank">Summon prize rank obtained for this summon</param>
    /// <param name="summon_point_id">Summon point entry id (unknown why needed since <see cref="summon_point"/> exists)</param>
    /// <param name="summon_point">Amount of summon points received</param>
    /// <param name="get_dew_point_quantity">Amount of Dew points received</param>
    [MessagePackObject(true)]
    public record SummonHistory(
        long key_id,
        int summon_id,
        SummonExecTypes summon_exec_type,
        [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
            DateTimeOffset exec_date,
        PaymentTypes payment_type,
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int entity_level,
        int entity_rarity,
        int entity_limit_break_count,
        int entity_hp_plus_count,
        int entity_attack_plus_count,
        SummonPrizeRanks summon_prize_rank,
        int summon_point_id,
        int summon_point,
        int get_dew_point_quantity
    );

    public static SummonHistory Create(DbPlayerSummonHistory dbEntry)
    {
        return new SummonHistory(
            key_id: dbEntry.SummonId,
            summon_id: dbEntry.BannerId,
            summon_exec_type: dbEntry.SummonExecType,
            exec_date: dbEntry.ExecDate,
            payment_type: dbEntry.PaymentType,
            entity_type: dbEntry.EntityType,
            entity_id: dbEntry.EntityId,
            entity_quantity: dbEntry.EntityQuantity,
            entity_level: dbEntry.EntityLevel,
            entity_rarity: dbEntry.EntityRarity,
            entity_limit_break_count: dbEntry.EntityLimitBreakCount,
            entity_hp_plus_count: dbEntry.EntityHpPlusCount,
            entity_attack_plus_count: dbEntry.EntityAtkPlusCount,
            summon_prize_rank: dbEntry.SummonPrizeRank,
            summon_point_id: dbEntry.BannerId,
            summon_point: dbEntry.SummonPointGet,
            get_dew_point_quantity: dbEntry.DewPointGet
        );
    }
}
