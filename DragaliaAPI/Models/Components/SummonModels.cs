using MessagePack;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Models.Components;

/// <param name="required_count_to_next">Count until next pity rate increase</param>
/// <seealso cref="SummonOddsData"/>
[MessagePackObject(true)]
public record ExtSummonOddsData(
    int? required_count_to_next,
    SummonOdds normal,
    SummonOdds guarantee
) : SummonOddsData(normal, guarantee);

[MessagePackObject(true)]
public record SummonOddsData(SummonOdds normal, SummonOdds guarantee);

[MessagePackObject(true)]
public record SummonOdds(
    List<Rarity> rarity_list,
    List<RarityGroup> rarity_group_list,
    OddsData unit
);

[MessagePackObject(true)]
public record Rarity(int rarity, string total_rate);

[MessagePackObject(true)]
public record RarityGroup(
    bool pickup,
    int rarity,
    string total_rate,
    string chara_rate,
    string dragon_rate
);

[MessagePackObject(true)]
public record OddsData(List<RarityOddsList> chara_odds_list, List<RarityOddsList> dragon_odds_list);

[MessagePackObject(true)]
public record RarityGroupsList(bool pickup, int rarity, List<Odds> unit_list);

[MessagePackObject(true)]
public record RarityOddsList(bool pickup, int rarity, List<Odds> unit_list);

[MessagePackObject(true)]
public record Odds(int id, string rate);

//TODO: Probably taken from an inventory table
[MessagePackObject(true)]
public record SummonTicket(int key_id, int summon_ticket_id, int quantity, long use_limit_time);

//TODO: Don't know the actual members of the record for prize summons
[MessagePackObject(true)]
public record SummonPrize(int material_id, int quantity);

[MessagePackObject(true)]
public record SummonReward(int entity_type, int id, int rarity, bool is_new, int dew_point)
    : SimpleSummonReward(entity_type, id, rarity);

[MessagePackObject(true)]
public record TradableEntity(int trade_id, int entity_type, int entity_id)
    : BaseNewEntity(entity_type, entity_id);

[MessagePackObject(true)]
public record SummonableEntity(int entity_type, int entity_id, int rarity, int quantity)
    : BaseNewEntity(entity_type, entity_id);

[MessagePackObject(true)]
public record QuestReward(
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count
) : BaseNewEntity(entity_type, entity_id);

[MessagePackObject(true)]
public record BaseNewEntity(int entity_type, int entity_id);

[MessagePackObject(true)]
public record SimpleSummonReward(int entity_type, int id, int rarity) : BaseReward(entity_type, id);

[MessagePackObject(true)]
public record BaseReward(int entity_type, int id);

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
