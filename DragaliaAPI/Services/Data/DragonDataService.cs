using DragaliaAPI.Models.Data;
using DragaliaAPI.Services.Data.Models;
using System.Text.Json;

namespace DragaliaAPI.Services.Data;

// Tables to use for this CargoQuery:
// Dragons.Id, Dragons.FullName, Dragons.Rarity, Dragons.ElementalType, Dragons.MinHp, Dragons.MaxHp, Dragons.AddMaxHp1, Dragons.MinAtk, Dragons.MaxAtk, Dragons.AddMaxAtk1, Dragons.SkillID, Dragons.Skill2ID, Dragons.Abilities11, Dragons.Abilities12, Dragons.Abilities13, Dragons.Abilities14, Dragons.Abilities15, Dragons.Abilities16, Dragons.Abilities21, Dragons.Abilities22, Dragons.Abilities23, Dragons.Abilities24, Dragons.Abilities25, Dragons.Abilities26, Dragons.DmodePassiveAbilityId, Dragons.MaxLimitBreakCount, Dragons.LimitBreakMaterialId, Dragons.FavoriteType, Dragons.SellCoin, Dragons.SellDewPoint, Dragons.Availability, Dragons.DraconicEssenceLocation,
public class DragonDataService : IDragonDataService
{
    private const string _filename = "dragons.json";
    private const string _folder = "Resources";

    private readonly Dictionary<int, DataDragon> _dictionary;

    public IEnumerable<DataDragon> AllData
    {
        get => _dictionary.Values;
    }

    public DragonDataService()
    {
        string json = File.ReadAllText(Path.Join(_folder, _filename));
        List<DataDragon> deserialized =
            JsonSerializer.Deserialize<List<DataDragon>>(json)
            ?? throw new JsonException("Deserialization failure");

        _dictionary = deserialized
            .Select(x => new KeyValuePair<int, DataDragon>(x.Id, x))
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public DataDragon GetData(Dragons id) => _dictionary[(int)id];

    public DataDragon GetData(int id) => _dictionary[id];
}
