using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task Create(string deviceAccountId);
    Task CreateBase(string deviceAccountId);
    Task Import(string deviceAccountId, LoadIndexData savefile);
    Task Clear(string deviceAccountId, bool recreate = true);
}
