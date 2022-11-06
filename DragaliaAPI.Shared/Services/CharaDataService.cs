using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using System.Reflection;
using System.Text.Json;

namespace DragaliaAPI.Shared.Services;

// Tables to use for this CargoQuery:
// Adventurers.IdLong, Adventurers.FullName, Adventurers.WeaponTypeId, Adventurers.Rarity, Adventurers.MaxLimitBreakCount, Adventurers.ElementalType, Adventurers.MinHp3, Adventurers.MinHp4, Adventurers.MinHp5, Adventurers.MaxHp, Adventurers.AddMaxHp1, Adventurers.PlusHp1, Adventurers.PlusHp2, Adventurers.PlusHp3, Adventurers.PlusHp4, Adventurers.PlusHp5, Adventurers.McFullBonusHp5, Adventurers.MinAtk3, Adventurers.MinAtk4, Adventurers.MinAtk5, Adventurers.MaxAtk, Adventurers.AddMaxAtk1, Adventurers.PlusAtk0, Adventurers.PlusAtk1, Adventurers.PlusAtk2, Adventurers.PlusAtk3, Adventurers.PlusAtk3, Adventurers.PlusAtk4, Adventurers.PlusAtk5, Adventurers.McFullBonusAtk5, Adventurers.MinDef, Adventurers.DefCoef, Adventurers.Skill1ID, Adventurers.Skill2ID, Adventurers.HoldEditSkillCost, Adventurers.EditSkillId, Adventurers.EditSkillLevelNum, Adventurers.EditSkillCost, Adventurers.EditSkillRelationId, Adventurers.Abilities11, Adventurers.Abilities12, Adventurers.Abilities13, Adventurers.Abilities14, Adventurers.Abilities21, Adventurers.Abilities22, Adventurers.Abilities23, Adventurers.Abilities24, Adventurers.Abilities31, Adventurers.Abilities32, Adventurers.Abilities33, Adventurers.Abilities34, Adventurers.ExAbilityData1, Adventurers.ExAbilityData2, Adventurers.ExAbilityData3, Adventurers.ExAbilityData4, Adventurers.ExAbilityData5, Adventurers.ExAbility2Data1, Adventurers.ExAbility2Data2, Adventurers.ExAbility2Data3, Adventurers.ExAbility2Data4, Adventurers.ExAbility2Data5, Adventurers.Availability, Adventurers.LimitBreakMaterialId,

public class CharaDataService : ICharaDataService
{
    private const string _filename = "adventurers.json";
    private const string _folder = "Resources";

    private readonly Dictionary<int, DataAdventurer> _dictionary;

    public IEnumerable<DataAdventurer> AllData => _dictionary.Values;

    public CharaDataService()
    {
        string json = File.ReadAllText(
            Path.Join(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                _folder,
                _filename
            )
        );
        List<DataAdventurer> deserialized =
            JsonSerializer.Deserialize<List<DataAdventurer>>(json)
            ?? throw new JsonException("Deserialization failure");

        _dictionary = deserialized
            .Select(x => new KeyValuePair<int, DataAdventurer>(x.IdLong, x))
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public DataAdventurer GetData(Charas id)
    {
        return _dictionary[(int)id];
    }

    public DataAdventurer GetData(int id)
    {
        return _dictionary[id];
    }

    public IEnumerable<DataAdventurer> getByRarity(int rarity) =>
        _dictionary.Values.Where(x => x.Rarity == rarity);
}
