using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.Common;

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
public record RarityOddsList(bool pickup, int rarity, List<Odds> unit_list);

[MessagePackObject(true)]
public record Odds(int id, string rate);

[MessagePackObject(true)]
public record SummonEntity(int entity_type, int id, int rarity) : Entity(entity_type, id);

[MessagePackObject(true)]
public record Entity(int entity_type, int id);
