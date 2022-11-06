using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.Services;

public interface IDragonDataService
{
    IEnumerable<DataDragon> AllData { get; }

    DataDragon GetData(Dragons id);
    DataDragon GetData(int id);
    public IEnumerable<DataDragon> getByRarity(int rarity);
}
