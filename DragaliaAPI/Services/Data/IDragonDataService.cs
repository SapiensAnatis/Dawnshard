using DragaliaAPI.Models.Data;
using DragaliaAPI.Services.Data.Models;

namespace DragaliaAPI.Services.Data;

public interface IDragonDataService
{
    IEnumerable<DataDragon> AllData { get; }

    DataDragon GetData(Dragons id);
    DataDragon GetData(int id);
}
