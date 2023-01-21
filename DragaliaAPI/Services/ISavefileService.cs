using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task Create(string deviceAccountId);
    Task CreateBase(string deviceAccountId);
    Task ThreadSafeImport(string deviceAccountId, LoadIndexData savefile);
    Task Import(string deviceAccountId, LoadIndexData savefile);
    IQueryable<DbPlayer> Load(string deviceAccountId);
    Task Reset(string deviceAccountId);
}
