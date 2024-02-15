using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task ThreadSafeImport(LoadIndexData savefile);
    Task Import(LoadIndexData savefile);
    IQueryable<DbPlayer> Load();
    Task Reset();
    Task<DbPlayer> Create();
    Task<DbPlayer> Create(string deviceAccountId);
}
