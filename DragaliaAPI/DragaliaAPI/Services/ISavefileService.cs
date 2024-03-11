using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task ThreadSafeImport(LoadIndexResponse savefile);
    Task Import(LoadIndexResponse savefile);
    Task Reset();
    Task<DbPlayer> Create();
    Task<DbPlayer> Create(string deviceAccountId);
}
