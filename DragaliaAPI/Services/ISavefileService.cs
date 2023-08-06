using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task Create();
    Task ThreadSafeImport(LoadIndexData savefile);
    Task Import(LoadIndexData savefile);
    IQueryable<DbPlayer> Load();
    Task Reset();
}
