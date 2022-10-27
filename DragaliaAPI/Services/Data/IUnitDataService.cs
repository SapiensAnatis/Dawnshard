using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Services.Data.Models;

namespace DragaliaAPI.Services.Data;

public interface IUnitDataService
{
    IEnumerable<DataAdventurer> AllData { get; }

    DataAdventurer GetData(Charas id);
    DataAdventurer GetData(int id);
    public IEnumerable<DataAdventurer> getByRarity(int rarity);
}
